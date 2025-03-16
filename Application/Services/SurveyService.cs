
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SurveyApp.Application.DTOs;
using SurveyApp.Application.Ports;
using SurveyApp.Domain.Entities;

namespace SurveyApp.Application.Services
{
    public class SurveyService : ISurveyService
    {
        private readonly ISurveyRepository _surveyRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger<SurveyService> _logger;
        private const string BASE_SURVEY_URL = "https://yoursurveyapp.com/survey/";

        public SurveyService(
            ISurveyRepository surveyRepository, 
            IEmailService emailService,
            ILogger<SurveyService> logger)
        {
            _surveyRepository = surveyRepository ?? throw new ArgumentNullException(nameof(surveyRepository));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<SurveyDto> GetSurveyByIdAsync(Guid id)
        {
            var survey = await _surveyRepository.GetByIdAsync(id);
            
            if (survey == null)
            {
                _logger.LogWarning("Survey with ID {SurveyId} not found", id);
                throw new KeyNotFoundException($"Survey with ID {id} not found.");
            }
            
            return MapToDto(survey);
        }

        public async Task<List<SurveyDto>> GetAllSurveysAsync()
        {
            var surveys = await _surveyRepository.GetAllAsync();
            return surveys.Select(MapToDto).ToList();
        }

        public async Task<SurveyDto> CreateSurveyAsync(CreateSurveyDto createSurveyDto)
        {
            if (createSurveyDto == null)
            {
                throw new ArgumentNullException(nameof(createSurveyDto));
            }
            
            ValidateSurveyDto(createSurveyDto);

            var survey = new Survey(createSurveyDto.Title, createSurveyDto.Description);

            foreach (var questionDto in createSurveyDto.Questions)
            {
                var questionType = Enum.Parse<QuestionType>(questionDto.Type, true);
                var question = new Question(questionDto.Title, questionType, questionDto.Required);

                if (!string.IsNullOrEmpty(questionDto.Description))
                {
                    question.UpdateDescription(questionDto.Description);
                }

                if (questionDto.Options != null && questionDto.Options.Any())
                {
                    question.SetOptions(questionDto.Options);
                }

                survey.AddQuestion(question);
            }

            if (createSurveyDto.DeliveryConfig != null)
            {
                var deliveryConfig = MapToDeliveryConfig(createSurveyDto.DeliveryConfig);
                survey.SetDeliveryConfig(deliveryConfig);
            }

            var createdSurvey = await _surveyRepository.CreateAsync(survey);
            
            // Auto-send for scheduled monthly surveys
            if (ShouldSendSurveyAutomatically(createSurveyDto.DeliveryConfig))
            {
                try
                {
                    await SendSurveyEmailsAsync(createdSurvey.Id);
                    _logger.LogInformation("Automatically sent monthly survey {SurveyId}", createdSurvey.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to automatically send monthly survey {SurveyId}", createdSurvey.Id);
                    // Continue with survey creation even if email sending fails
                }
            }
            
            return MapToDto(createdSurvey);
        }

        public async Task UpdateSurveyAsync(Guid id, CreateSurveyDto updateSurveyDto)
        {
            if (updateSurveyDto == null)
            {
                throw new ArgumentNullException(nameof(updateSurveyDto));
            }
            
            ValidateSurveyDto(updateSurveyDto);
            
            var survey = await _surveyRepository.GetByIdAsync(id);
            
            if (survey == null)
            {
                _logger.LogWarning("Survey with ID {SurveyId} not found during update", id);
                throw new KeyNotFoundException($"Survey with ID {id} not found.");
            }

            survey.UpdateTitle(updateSurveyDto.Title);
            survey.UpdateDescription(updateSurveyDto.Description);
            
            // Clear existing questions and add new ones
            survey.Questions.Clear();
            foreach (var questionDto in updateSurveyDto.Questions)
            {
                var questionType = Enum.Parse<QuestionType>(questionDto.Type, true);
                var question = new Question(questionDto.Title, questionType, questionDto.Required);

                if (!string.IsNullOrEmpty(questionDto.Description))
                {
                    question.UpdateDescription(questionDto.Description);
                }

                if (questionDto.Options != null && questionDto.Options.Any())
                {
                    question.SetOptions(questionDto.Options);
                }

                survey.AddQuestion(question);
            }

            if (updateSurveyDto.DeliveryConfig != null)
            {
                var deliveryConfig = MapToDeliveryConfig(updateSurveyDto.DeliveryConfig);
                survey.SetDeliveryConfig(deliveryConfig);
            }

            await _surveyRepository.UpdateAsync(survey);
            _logger.LogInformation("Updated survey {SurveyId}", id);
        }

        public async Task DeleteSurveyAsync(Guid id)
        {
            if (!await _surveyRepository.ExistsAsync(id))
            {
                _logger.LogWarning("Survey with ID {SurveyId} not found during deletion", id);
                throw new KeyNotFoundException($"Survey with ID {id} not found.");
            }
            
            await _surveyRepository.DeleteAsync(id);
            _logger.LogInformation("Deleted survey {SurveyId}", id);
        }

        public async Task SendSurveyEmailsAsync(Guid id)
        {
            var survey = await _surveyRepository.GetByIdAsync(id);
            
            if (survey == null)
            {
                _logger.LogWarning("Survey with ID {SurveyId} not found when sending emails", id);
                throw new KeyNotFoundException($"Survey with ID {id} not found.");
            }

            if (survey.DeliveryConfig.EmailAddresses.Count == 0)
            {
                _logger.LogWarning("No email recipients specified for survey {SurveyId}", id);
                throw new InvalidOperationException("No email recipients specified.");
            }

            var invalidEmails = survey.DeliveryConfig.EmailAddresses
                .Where(email => !IsValidEmail(email))
                .ToList();
                
            if (invalidEmails.Any())
            {
                throw new InvalidOperationException($"The following email addresses are invalid: {string.Join(", ", invalidEmails)}");
            }

            var surveyLink = $"{BASE_SURVEY_URL}{survey.Id}";
            int successCount = 0;
            
            foreach (var email in survey.DeliveryConfig.EmailAddresses)
            {
                try
                {
                    await _emailService.SendSurveyInvitationAsync(email, survey.Title, surveyLink);
                    successCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send survey {SurveyId} to email {Email}", id, email);
                    // Continue with other emails even if one fails
                }
            }
            
            if (successCount == 0 && survey.DeliveryConfig.EmailAddresses.Any())
            {
                throw new InvalidOperationException("Failed to send all emails. Please check your email configuration.");
            }
            
            _logger.LogInformation("Sent {SuccessCount} of {TotalCount} emails for survey {SurveyId}", 
                successCount, survey.DeliveryConfig.EmailAddresses.Count, id);
        }

        public async Task<bool> SendSurveyOnTicketClosedAsync(string customerEmail, Guid? specificSurveyId = null)
        {
            if (string.IsNullOrWhiteSpace(customerEmail))
            {
                throw new ArgumentException("Customer email is required");
            }
                
            if (!IsValidEmail(customerEmail))
            {
                throw new ArgumentException($"Invalid email format: {customerEmail}");
            }
                
            List<Survey> surveysToSend;
            
            if (specificSurveyId.HasValue)
            {
                // If a specific survey is requested, send only that one
                var survey = await _surveyRepository.GetByIdAsync(specificSurveyId.Value);
                if (survey == null)
                {
                    _logger.LogWarning("Survey with ID {SurveyId} not found for ticket-closed event", specificSurveyId);
                    throw new KeyNotFoundException($"Survey with ID {specificSurveyId} not found.");
                }
                    
                surveysToSend = new List<Survey> { survey };
            }
            else
            {
                // Get all surveys configured for automatic sending with ticket-closed trigger
                var allSurveys = await _surveyRepository.GetAllAsync();
                surveysToSend = allSurveys
                    .Where(s => s.DeliveryConfig.Type == DeliveryType.Triggered && 
                           s.DeliveryConfig.Trigger.Type == "ticket-closed" &&
                           s.DeliveryConfig.Trigger.SendAutomatically)
                    .ToList();
            }
            
            if (!surveysToSend.Any())
            {
                _logger.LogInformation("No eligible surveys found for ticket-closed event to {Email}", customerEmail);
                return false;
            }
                
            // Send each relevant survey to the customer
            int successCount = 0;
            foreach (var survey in surveysToSend)
            {
                try
                {
                    var surveyLink = $"{BASE_SURVEY_URL}{survey.Id}";
                    await _emailService.SendSurveyInvitationAsync(customerEmail, survey.Title, surveyLink);
                    successCount++;
                    _logger.LogInformation("Sent ticket-closed survey {SurveyId} to {Email}", survey.Id, customerEmail);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send ticket-closed survey {SurveyId} to {Email}", survey.Id, customerEmail);
                    // Continue with other surveys even if one fails
                }
            }
            
            return successCount > 0;
        }

        public async Task<bool> SendTestSurveyEmailAsync(string email, Guid surveyId)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email address is required");
            }
                
            if (!IsValidEmail(email))
            {
                throw new ArgumentException($"Invalid email format: {email}");
            }
                
            var survey = await _surveyRepository.GetByIdAsync(surveyId);
            
            if (survey == null)
            {
                _logger.LogWarning("Survey with ID {SurveyId} not found when sending test email", surveyId);
                throw new KeyNotFoundException($"Survey with ID {surveyId} not found.");
            }
            
            try
            {    
                var surveyLink = $"{BASE_SURVEY_URL}{survey.Id}";
                await _emailService.SendSurveyInvitationAsync(email, survey.Title, surveyLink);
                _logger.LogInformation("Sent test email for survey {SurveyId} to {Email}", surveyId, email);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send test email for survey {SurveyId} to {Email}", surveyId, email);
                return false;
            }
        }

        // Helper methods for mapping between DTOs and domain entities
        private SurveyDto MapToDto(Survey survey)
        {
            if (survey == null) return null;
            
            return new SurveyDto
            {
                Id = survey.Id,
                Title = survey.Title,
                Description = survey.Description,
                CreatedAt = survey.CreatedAt,
                Responses = survey.Responses,
                CompletionRate = survey.CompletionRate,
                Questions = survey.Questions.Select(q => new QuestionDto
                {
                    Id = q.Id,
                    Type = q.Type.ToString(),
                    Title = q.Title,
                    Description = q.Description,
                    Required = q.Required,
                    Options = q.Options
                }).ToList(),
                DeliveryConfig = new DeliveryConfigDto
                {
                    Type = survey.DeliveryConfig.Type.ToString(),
                    EmailAddresses = survey.DeliveryConfig.EmailAddresses,
                    Schedule = new ScheduleDto
                    {
                        Frequency = survey.DeliveryConfig.Schedule.Frequency,
                        DayOfMonth = survey.DeliveryConfig.Schedule.DayOfMonth,
                        DayOfWeek = survey.DeliveryConfig.Schedule.DayOfWeek,
                        Time = survey.DeliveryConfig.Schedule.Time,
                        StartDate = survey.DeliveryConfig.Schedule.StartDate
                    },
                    Trigger = new TriggerDto
                    {
                        Type = survey.DeliveryConfig.Trigger.Type,
                        DelayHours = survey.DeliveryConfig.Trigger.DelayHours,
                        SendAutomatically = survey.DeliveryConfig.Trigger.SendAutomatically
                    }
                }
            };
        }

        private DeliveryConfig MapToDeliveryConfig(DeliveryConfigDto dto)
        {
            if (dto == null) return new DeliveryConfig();
            
            var deliveryConfig = new DeliveryConfig();
            
            try
            {
                deliveryConfig.SetType(Enum.Parse<DeliveryType>(dto.Type, true));
            }
            catch (Exception)
            {
                deliveryConfig.SetType(DeliveryType.Manual);
            }

            if (dto.EmailAddresses != null)
            {
                foreach (var email in dto.EmailAddresses.Where(e => !string.IsNullOrWhiteSpace(e)))
                {
                    deliveryConfig.AddEmailAddress(email.Trim());
                }
            }

            if (dto.Schedule != null)
            {
                deliveryConfig.SetSchedule(new Schedule
                {
                    Frequency = dto.Schedule.Frequency,
                    DayOfMonth = dto.Schedule.DayOfMonth ?? 1,
                    DayOfWeek = dto.Schedule.DayOfWeek,
                    Time = dto.Schedule.Time,
                    StartDate = dto.Schedule.StartDate
                });
            }

            if (dto.Trigger != null)
            {
                deliveryConfig.SetTrigger(new Trigger
                {
                    Type = dto.Trigger.Type,
                    DelayHours = dto.Trigger.DelayHours,
                    SendAutomatically = dto.Trigger.SendAutomatically
                });
            }

            return deliveryConfig;
        }
        
        // Helper method to validate survey data
        private void ValidateSurveyDto(CreateSurveyDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title))
            {
                throw new ArgumentException("Survey title is required");
            }
            
            if (dto.Questions == null || !dto.Questions.Any())
            {
                throw new ArgumentException("At least one question is required");
            }
            
            foreach (var question in dto.Questions)
            {
                if (string.IsNullOrWhiteSpace(question.Title))
                {
                    throw new ArgumentException("Question title is required for all questions");
                }
                
                if (question.Type.Equals("SingleChoice", StringComparison.OrdinalIgnoreCase) || 
                    question.Type.Equals("MultipleChoice", StringComparison.OrdinalIgnoreCase))
                {
                    if (question.Options == null || question.Options.Count < 2)
                    {
                        throw new ArgumentException($"Question '{question.Title}' must have at least 2 options");
                    }
                }
            }
        }
        
        // Helper method to check email validity
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;
                
            try
            {
                var mailAddress = new MailAddress(email);
                return mailAddress.Address == email;
            }
            catch
            {
                return false;
            }
        }
        
        // Helper method to check if survey should be sent automatically
        private bool ShouldSendSurveyAutomatically(DeliveryConfigDto config)
        {
            return config?.Type == "Scheduled" && 
                   config.Schedule?.Frequency == "monthly" &&
                   config.EmailAddresses != null && 
                   config.EmailAddresses.Count > 0;
        }
    }
}
