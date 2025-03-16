
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SurveyApp.Application.DTOs;
using SurveyApp.Application.Ports;
using SurveyApp.Domain.Entities;

namespace SurveyApp.Application.Services
{
    public class SurveyService : ISurveyService
    {
        private readonly ISurveyRepository _surveyRepository;
        private readonly IEmailService _emailService;

        public SurveyService(ISurveyRepository surveyRepository, IEmailService emailService)
        {
            _surveyRepository = surveyRepository;
            _emailService = emailService;
        }

        public async Task<SurveyDto> GetSurveyByIdAsync(Guid id)
        {
            var survey = await _surveyRepository.GetByIdAsync(id);
            return MapToDto(survey);
        }

        public async Task<List<SurveyDto>> GetAllSurveysAsync()
        {
            var surveys = await _surveyRepository.GetAllAsync();
            return surveys.Select(MapToDto).ToList();
        }

        public async Task<SurveyDto> CreateSurveyAsync(CreateSurveyDto createSurveyDto)
        {
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
            
            // Si la encuesta está configurada para envío automático mensual, programar envío
            if (createSurveyDto.DeliveryConfig?.Type == "Scheduled" && 
                createSurveyDto.DeliveryConfig?.Schedule?.Frequency == "monthly" &&
                createSurveyDto.DeliveryConfig?.EmailAddresses.Count > 0)
            {
                await SendSurveyEmailsAsync(createdSurvey.Id);
            }
            
            return MapToDto(createdSurvey);
        }

        public async Task UpdateSurveyAsync(Guid id, CreateSurveyDto updateSurveyDto)
        {
            var survey = await _surveyRepository.GetByIdAsync(id);
            
            if (survey == null)
            {
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
        }

        public async Task DeleteSurveyAsync(Guid id)
        {
            await _surveyRepository.DeleteAsync(id);
        }

        public async Task SendSurveyEmailsAsync(Guid id)
        {
            var survey = await _surveyRepository.GetByIdAsync(id);
            
            if (survey == null)
            {
                throw new KeyNotFoundException($"Survey with ID {id} not found.");
            }

            if (survey.DeliveryConfig.EmailAddresses.Count == 0)
            {
                throw new InvalidOperationException("No email recipients specified.");
            }

            var surveyLink = $"https://yoursurveyapp.com/survey/{survey.Id}";
            
            foreach (var email in survey.DeliveryConfig.EmailAddresses)
            {
                await _emailService.SendSurveyInvitationAsync(email, survey.Title, surveyLink);
            }
        }

        // Nuevo método para enviar encuestas automáticamente cuando se cierra un ticket
        public async Task SendSurveyOnTicketClosedAsync(string customerEmail, Guid? specificSurveyId = null)
        {
            if (string.IsNullOrEmpty(customerEmail))
                throw new ArgumentException("El correo del cliente es requerido");
                
            List<Survey> surveysToSend;
            
            if (specificSurveyId.HasValue)
            {
                // Si se especifica una encuesta, enviar solo esa
                var survey = await _surveyRepository.GetByIdAsync(specificSurveyId.Value);
                if (survey == null)
                    throw new KeyNotFoundException($"No se encontró la encuesta con ID {specificSurveyId}");
                    
                surveysToSend = new List<Survey> { survey };
            }
            else
            {
                // Obtener todas las encuestas configuradas para envío automático con trigger "ticket-closed"
                var allSurveys = await _surveyRepository.GetAllAsync();
                surveysToSend = allSurveys
                    .Where(s => s.DeliveryConfig.Type == DeliveryType.Triggered && 
                           s.DeliveryConfig.Trigger.Type == "ticket-closed" &&
                           s.DeliveryConfig.Trigger.SendAutomatically)
                    .ToList();
            }
            
            if (!surveysToSend.Any())
                return;
                
            // Enviar cada encuesta relevante al cliente
            foreach (var survey in surveysToSend)
            {
                var surveyLink = $"https://yoursurveyapp.com/survey/{survey.Id}";
                await _emailService.SendSurveyInvitationAsync(customerEmail, survey.Title, surveyLink);
            }
        }

        // Helper methods for mapping between DTOs and domain entities
        private SurveyDto MapToDto(Survey survey)
        {
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
            var deliveryConfig = new DeliveryConfig();
            deliveryConfig.SetType(Enum.Parse<DeliveryType>(dto.Type, true));

            foreach (var email in dto.EmailAddresses)
            {
                deliveryConfig.AddEmailAddress(email);
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
    }
}
