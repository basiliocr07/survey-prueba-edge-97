
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
            
            _logger.LogInformation("SurveyService initialized with repository and email service");
        }

        public async Task<SurveyDto> GetSurveyByIdAsync(Guid id)
        {
            _logger.LogInformation("GetSurveyByIdAsync called with ID: {SurveyId}", id);
            
            var survey = await _surveyRepository.GetByIdAsync(id);
            
            if (survey == null)
            {
                _logger.LogWarning("Survey with ID {SurveyId} not found", id);
                throw new KeyNotFoundException($"Survey with ID {id} not found.");
            }
            
            _logger.LogInformation("Survey found: {SurveyTitle}", survey.Title);
            return MapToDto(survey);
        }

        public async Task<List<SurveyDto>> GetAllSurveysAsync()
        {
            _logger.LogInformation("GetAllSurveysAsync called");
            
            var surveys = await _surveyRepository.GetAllAsync();
            
            _logger.LogInformation("Retrieved {Count} surveys", surveys.Count);
            return surveys.Select(MapToDto).ToList();
        }

        public async Task<SurveyDto> CreateSurveyAsync(CreateSurveyDto createSurveyDto)
        {
            _logger.LogInformation("CreateSurveyAsync called with title: {Title}", createSurveyDto?.Title);
            
            if (createSurveyDto == null)
            {
                _logger.LogError("CreateSurveyDto is null");
                throw new ArgumentNullException(nameof(createSurveyDto));
            }
            
            try
            {
                ValidateSurveyDto(createSurveyDto);
                
                _logger.LogInformation("Creating survey with {QuestionCount} questions", 
                    createSurveyDto.Questions?.Count ?? 0);

                var survey = new Survey(createSurveyDto.Title, createSurveyDto.Description);

                foreach (var questionDto in createSurveyDto.Questions)
                {
                    _logger.LogDebug("Processing question: {QuestionTitle}, Type: {QuestionType}", 
                        questionDto.Title, questionDto.Type);
                        
                    var questionType = Enum.Parse<QuestionType>(questionDto.Type, true);
                    var question = new Question(questionDto.Title, questionType, questionDto.Required);

                    if (!string.IsNullOrEmpty(questionDto.Description))
                    {
                        question.UpdateDescription(questionDto.Description);
                    }

                    if (questionDto.Options != null && questionDto.Options.Any())
                    {
                        _logger.LogDebug("Setting {OptionCount} options for question", questionDto.Options.Count);
                        question.SetOptions(questionDto.Options);
                    }

                    survey.AddQuestion(question);
                }

                if (createSurveyDto.DeliveryConfig != null)
                {
                    _logger.LogInformation("Setting delivery config, Type: {DeliveryType}", 
                        createSurveyDto.DeliveryConfig.Type);
                        
                    var deliveryConfig = MapToDeliveryConfig(createSurveyDto.DeliveryConfig);
                    survey.SetDeliveryConfig(deliveryConfig);
                }

                var createdSurvey = await _surveyRepository.CreateAsync(survey);
                _logger.LogInformation("Survey created successfully with ID: {SurveyId}", createdSurvey.Id);
                
                // Auto-send for scheduled monthly surveys
                if (ShouldSendSurveyAutomatically(createSurveyDto.DeliveryConfig))
                {
                    try
                    {
                        _logger.LogInformation("Attempting to automatically send monthly survey {SurveyId}", 
                            createdSurvey.Id);
                            
                        await SendSurveyEmailsAsync(createdSurvey.Id);
                        _logger.LogInformation("Automatically sent monthly survey {SurveyId}", createdSurvey.Id);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to automatically send monthly survey {SurveyId}: {ErrorMessage}", 
                            createdSurvey.Id, ex.Message);
                        // Continue with survey creation even if email sending fails
                    }
                }
                
                return MapToDto(createdSurvey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating survey: {ErrorMessage}", ex.Message);
                throw;
            }
        }

        public async Task UpdateSurveyAsync(Guid id, CreateSurveyDto updateSurveyDto)
        {
            _logger.LogInformation("UpdateSurveyAsync called for ID: {SurveyId}", id);
            
            if (updateSurveyDto == null)
            {
                _logger.LogError("UpdateSurveyDto is null");
                throw new ArgumentNullException(nameof(updateSurveyDto));
            }
            
            try
            {
                ValidateSurveyDto(updateSurveyDto);
                
                var survey = await _surveyRepository.GetByIdAsync(id);
                
                if (survey == null)
                {
                    _logger.LogWarning("Survey with ID {SurveyId} not found during update", id);
                    throw new KeyNotFoundException($"Survey with ID {id} not found.");
                }

                _logger.LogInformation("Updating survey: {SurveyTitle} with {QuestionCount} questions", 
                    updateSurveyDto.Title, updateSurveyDto.Questions?.Count ?? 0);
                    
                survey.UpdateTitle(updateSurveyDto.Title);
                survey.UpdateDescription(updateSurveyDto.Description);
                
                // Clear existing questions and add new ones
                survey.Questions.Clear();
                foreach (var questionDto in updateSurveyDto.Questions)
                {
                    _logger.LogDebug("Processing question: {QuestionTitle}, Type: {QuestionType}", 
                        questionDto.Title, questionDto.Type);
                        
                    var questionType = Enum.Parse<QuestionType>(questionDto.Type, true);
                    var question = new Question(questionDto.Title, questionType, questionDto.Required);

                    if (!string.IsNullOrEmpty(questionDto.Description))
                    {
                        question.UpdateDescription(questionDto.Description);
                    }

                    if (questionDto.Options != null && questionDto.Options.Any())
                    {
                        _logger.LogDebug("Setting {OptionCount} options for question", questionDto.Options.Count);
                        question.SetOptions(questionDto.Options);
                    }

                    survey.AddQuestion(question);
                }

                if (updateSurveyDto.DeliveryConfig != null)
                {
                    _logger.LogInformation("Updating delivery config, Type: {DeliveryType}", 
                        updateSurveyDto.DeliveryConfig.Type);
                        
                    var deliveryConfig = MapToDeliveryConfig(updateSurveyDto.DeliveryConfig);
                    survey.SetDeliveryConfig(deliveryConfig);
                }

                await _surveyRepository.UpdateAsync(survey);
                _logger.LogInformation("Successfully updated survey {SurveyId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating survey {SurveyId}: {ErrorMessage}", id, ex.Message);
                throw;
            }
        }

        public async Task DeleteSurveyAsync(Guid id)
        {
            _logger.LogInformation("DeleteSurveyAsync called for ID: {SurveyId}", id);
            
            try
            {
                if (!await _surveyRepository.ExistsAsync(id))
                {
                    _logger.LogWarning("Survey with ID {SurveyId} not found during deletion", id);
                    throw new KeyNotFoundException($"Survey with ID {id} not found.");
                }
                
                await _surveyRepository.DeleteAsync(id);
                _logger.LogInformation("Successfully deleted survey {SurveyId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting survey {SurveyId}: {ErrorMessage}", id, ex.Message);
                throw;
            }
        }

        public async Task SendSurveyEmailsAsync(Guid id)
        {
            _logger.LogInformation("SendSurveyEmailsAsync called for ID: {SurveyId}", id);
            
            try
            {
                var survey = await _surveyRepository.GetByIdAsync(id);
                
                if (survey == null)
                {
                    _logger.LogWarning("Survey with ID {SurveyId} not found when sending emails", id);
                    throw new KeyNotFoundException($"Survey with ID {id} not found.");
                }

                if (survey.DeliveryConfig.EmailAddresses == null)
                {
                    _logger.LogError("EmailAddresses list is null for survey {SurveyId}", id);
                    throw new InvalidOperationException("Email addresses list is null.");
                }

                if (survey.DeliveryConfig.EmailAddresses.Count == 0)
                {
                    _logger.LogWarning("No email recipients specified for survey {SurveyId}", id);
                    throw new InvalidOperationException("No email recipients specified.");
                }

                _logger.LogInformation("Preparing to send survey {SurveyId} to {RecipientCount} recipients", 
                    id, survey.DeliveryConfig.EmailAddresses.Count);
                    
                var invalidEmails = survey.DeliveryConfig.EmailAddresses
                    .Where(email => !IsValidEmail(email))
                    .ToList();
                    
                if (invalidEmails.Any())
                {
                    _logger.LogWarning("Invalid email addresses found: {InvalidEmails}", 
                        string.Join(", ", invalidEmails));
                    throw new InvalidOperationException($"The following email addresses are invalid: {string.Join(", ", invalidEmails)}");
                }

                var surveyLink = $"{BASE_SURVEY_URL}{survey.Id}";
                int successCount = 0;
                
                foreach (var email in survey.DeliveryConfig.EmailAddresses)
                {
                    try
                    {
                        _logger.LogInformation("Sending survey {SurveyId} to {Email}", id, email);
                        await _emailService.SendSurveyInvitationAsync(email, survey.Title, surveyLink);
                        successCount++;
                        _logger.LogInformation("Successfully sent survey invitation to {Email}", email);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send survey {SurveyId} to email {Email}: {ErrorMessage}", 
                            id, email, ex.Message);
                        // Continue with other emails even if one fails
                    }
                }
                
                if (successCount == 0 && survey.DeliveryConfig.EmailAddresses.Any())
                {
                    _logger.LogError("Failed to send all {Count} emails for survey {SurveyId}", 
                        survey.DeliveryConfig.EmailAddresses.Count, id);
                    throw new InvalidOperationException("Failed to send all emails. Please check your email configuration.");
                }
                
                _logger.LogInformation("Sent {SuccessCount} of {TotalCount} emails for survey {SurveyId}", 
                    successCount, survey.DeliveryConfig.EmailAddresses.Count, id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SendSurveyEmailsAsync for survey {SurveyId}: {ErrorMessage}", 
                    id, ex.Message);
                throw;
            }
        }

        public async Task<bool> SendSurveyOnTicketClosedAsync(string customerEmail, Guid? specificSurveyId = null)
        {
            _logger.LogInformation("SendSurveyOnTicketClosedAsync called for email: {Email}, SpecificSurveyId: {SurveyId}", 
                customerEmail, specificSurveyId);
            
            try
            {
                if (string.IsNullOrWhiteSpace(customerEmail))
                {
                    _logger.LogError("Customer email is null or empty");
                    throw new ArgumentException("Customer email is required");
                }
                    
                if (!IsValidEmail(customerEmail))
                {
                    _logger.LogError("Invalid email format: {Email}", customerEmail);
                    throw new ArgumentException($"Invalid email format: {customerEmail}");
                }
                    
                List<Survey> surveysToSend;
                
                if (specificSurveyId.HasValue)
                {
                    // If a specific survey is requested, send only that one
                    _logger.LogInformation("Looking for specific survey: {SurveyId}", specificSurveyId.Value);
                    var survey = await _surveyRepository.GetByIdAsync(specificSurveyId.Value);
                    
                    if (survey == null)
                    {
                        _logger.LogWarning("Survey with ID {SurveyId} not found for ticket-closed event", specificSurveyId);
                        throw new KeyNotFoundException($"Survey with ID {specificSurveyId} not found.");
                    }
                        
                    surveysToSend = new List<Survey> { survey };
                    _logger.LogInformation("Using specific survey: {SurveyTitle}", survey.Title);
                }
                else
                {
                    // Get all surveys configured for automatic sending with ticket-closed trigger
                    _logger.LogInformation("Looking for surveys with ticket-closed trigger and automatic sending");
                    var allSurveys = await _surveyRepository.GetAllAsync();
                    surveysToSend = allSurveys
                        .Where(s => s.DeliveryConfig.Type == DeliveryType.Triggered && 
                               s.DeliveryConfig.Trigger.Type == "ticket-closed" &&
                               s.DeliveryConfig.Trigger.SendAutomatically)
                        .ToList();
                        
                    _logger.LogInformation("Found {Count} eligible surveys for ticket-closed event", surveysToSend.Count);
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
                        _logger.LogInformation("Sending ticket-closed survey {SurveyId}: {SurveyTitle} to {Email}", 
                            survey.Id, survey.Title, customerEmail);
                            
                        var surveyLink = $"{BASE_SURVEY_URL}{survey.Id}";
                        await _emailService.SendSurveyInvitationAsync(customerEmail, survey.Title, surveyLink);
                        successCount++;
                        _logger.LogInformation("Sent ticket-closed survey {SurveyId} to {Email}", survey.Id, customerEmail);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send ticket-closed survey {SurveyId} to {Email}: {ErrorMessage}", 
                            survey.Id, customerEmail, ex.Message);
                        // Continue with other surveys even if one fails
                    }
                }
                
                _logger.LogInformation("Successfully sent {SuccessCount} of {TotalCount} ticket-closed surveys to {Email}", 
                    successCount, surveysToSend.Count, customerEmail);
                return successCount > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SendSurveyOnTicketClosedAsync: {ErrorMessage}", ex.Message);
                throw;
            }
        }

        public async Task<bool> SendTestSurveyEmailAsync(string email, Guid surveyId)
        {
            _logger.LogInformation("SendTestSurveyEmailAsync called for email: {Email}, surveyId: {SurveyId}", 
                email, surveyId);
            
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    _logger.LogError("Email address is null or empty");
                    throw new ArgumentException("Email address is required");
                }
                    
                if (!IsValidEmail(email))
                {
                    _logger.LogError("Invalid email format: {Email}", email);
                    throw new ArgumentException($"Invalid email format: {email}");
                }
                    
                var survey = await _surveyRepository.GetByIdAsync(surveyId);
                
                if (survey == null)
                {
                    _logger.LogWarning("Survey with ID {SurveyId} not found when sending test email", surveyId);
                    throw new KeyNotFoundException($"Survey with ID {surveyId} not found.");
                }
                
                _logger.LogInformation("Sending test email for survey {SurveyId}: {SurveyTitle} to {Email}", 
                    surveyId, survey.Title, email);
                
                try
                {    
                    var surveyLink = $"{BASE_SURVEY_URL}{survey.Id}";
                    await _emailService.SendSurveyInvitationAsync(email, survey.Title, surveyLink);
                    _logger.LogInformation("Successfully sent test email for survey {SurveyId} to {Email}", surveyId, email);
                    return true;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send test email for survey {SurveyId} to {Email}: {ErrorMessage}", 
                        surveyId, email, ex.Message);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SendTestSurveyEmailAsync: {ErrorMessage}", ex.Message);
                throw;
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
