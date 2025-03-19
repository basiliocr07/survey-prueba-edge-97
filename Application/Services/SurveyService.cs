using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SurveyApp.Application.DTOs;
using SurveyApp.Application.Ports;
using SurveyApp.Domain.Entities;

namespace SurveyApp.Application.Services
{
    public partial class SurveyService : ISurveyService
    {
        private readonly ISurveyRepository _surveyRepository;
        private readonly ISurveyResponseRepository _surveyResponseRepository;
        private readonly IEmailService _emailService;

        public SurveyService(
            ISurveyRepository surveyRepository, 
            ISurveyResponseRepository surveyResponseRepository, 
            IEmailService emailService)
        {
            _surveyRepository = surveyRepository;
            _surveyResponseRepository = surveyResponseRepository;
            _emailService = emailService;
        }

        // Implementación del método CreateSurveyAsync
        public async Task<SurveyDto> CreateSurveyAsync(CreateSurveyDto surveyDto)
        {
            // Crear la entidad de encuesta con los datos del DTO
            var survey = new Survey
            {
                Id = Guid.NewGuid(),
                Title = surveyDto.Title,
                Description = surveyDto.Description,
                CreatedAt = DateTime.UtcNow,
                Status = "Active",
                Responses = 0
            };

            // Agregar preguntas a la encuesta
            if (surveyDto.Questions != null)
            {
                foreach (var questionDto in surveyDto.Questions)
                {
                    var question = new Question
                    {
                        Id = Guid.NewGuid(),
                        Title = questionDto.Title,
                        Description = questionDto.Description,
                        Type = questionDto.Type,
                        Required = questionDto.Required,
                        Options = questionDto.Options
                    };

                    survey.AddQuestion(question);
                }
            }

            // Agregar configuración de entrega si existe
            if (surveyDto.DeliveryConfig != null)
            {
                var deliveryConfig = new DeliveryConfig
                {
                    Type = surveyDto.DeliveryConfig.Type,
                    EmailAddresses = surveyDto.DeliveryConfig.EmailAddresses
                };

                if (surveyDto.DeliveryConfig.Schedule != null)
                {
                    deliveryConfig.Schedule = new Schedule
                    {
                        Frequency = surveyDto.DeliveryConfig.Schedule.Frequency,
                        DayOfMonth = surveyDto.DeliveryConfig.Schedule.DayOfMonth,
                        DayOfWeek = surveyDto.DeliveryConfig.Schedule.DayOfWeek,
                        Time = surveyDto.DeliveryConfig.Schedule.Time,
                        StartDate = surveyDto.DeliveryConfig.Schedule.StartDate
                    };
                }

                if (surveyDto.DeliveryConfig.Trigger != null)
                {
                    deliveryConfig.Trigger = new Trigger
                    {
                        Type = surveyDto.DeliveryConfig.Trigger.Type,
                        DelayHours = surveyDto.DeliveryConfig.Trigger.DelayHours,
                        SendAutomatically = surveyDto.DeliveryConfig.Trigger.SendAutomatically
                    };
                }

                survey.SetDeliveryConfig(deliveryConfig);
            }

            // Guardar la encuesta en el repositorio
            var createdSurvey = await _surveyRepository.CreateAsync(survey);

            // Mapear la entidad a DTO para retornar
            return MapToSurveyDto(createdSurvey);
        }

        // Implementa este método para actualizar el estado de una encuesta
        public async Task UpdateSurveyStatusAsync(Guid id, string status)
        {
            var survey = await _surveyRepository.GetByIdAsync(id);
            
            if (survey == null)
                throw new KeyNotFoundException($"Encuesta con ID {id} no encontrada.");
                
            survey.SetStatus(status);
            await _surveyRepository.UpdateAsync(survey);
        }

        // Implementa los métodos de respuesta de encuestas
        public async Task<SurveyResponseDto> SubmitSurveyResponseAsync(CreateSurveyResponseDto createResponseDto)
        {
            // Verificar que la encuesta existe
            var survey = await _surveyRepository.GetByIdAsync(createResponseDto.SurveyId);
            if (survey == null)
                throw new KeyNotFoundException($"Encuesta con ID {createResponseDto.SurveyId} no encontrada.");

            // Crear nueva respuesta
            var surveyResponse = new SurveyResponse(
                createResponseDto.SurveyId, 
                createResponseDto.RespondentName, 
                createResponseDto.RespondentEmail,
                createResponseDto.RespondentPhone,
                createResponseDto.RespondentCompany
            );
            
            // Establecer la información del cliente
            surveyResponse.SetClientInfo(createResponseDto.IsExistingClient, createResponseDto.ExistingClientId);

            // Procesar respuestas
            foreach (var answerEntry in createResponseDto.Answers)
            {
                var questionId = Guid.Parse(answerEntry.Key);
                var question = survey.Questions.FirstOrDefault(q => q.Id == questionId);
                
                if (question == null)
                    continue;

                if (question.Type == "multiple-choice" && answerEntry.Value is List<string> multipleAnswers)
                {
                    var questionResponse = new QuestionResponse(questionId, question.Title, question.Type, multipleAnswers);
                    surveyResponse.AddAnswer(questionResponse);
                }
                else
                {
                    var answerValue = answerEntry.Value?.ToString() ?? string.Empty;
                    var questionResponse = new QuestionResponse(questionId, question.Title, question.Type, answerValue);
                    surveyResponse.AddAnswer(questionResponse);
                }
            }

            // Guardar la respuesta
            var createdResponse = await _surveyResponseRepository.CreateAsync(surveyResponse);

            // Incrementar el contador de respuestas en la encuesta
            survey.IncrementResponses();
            await _surveyRepository.UpdateAsync(survey);

            // Mapear a DTO para devolver
            return MapToSurveyResponseDto(createdResponse);
        }

        public async Task<List<SurveyResponseDto>> GetSurveyResponsesAsync(Guid surveyId)
        {
            // Verificar que la encuesta existe
            var survey = await _surveyRepository.GetByIdAsync(surveyId);
            if (survey == null)
                throw new KeyNotFoundException($"Encuesta con ID {surveyId} no encontrada.");

            // Obtener respuestas
            var responses = await _surveyResponseRepository.GetBySurveyIdAsync(surveyId);

            // Mapear a DTOs
            return responses.Select(MapToSurveyResponseDto).ToList();
        }

        // Nuevo método para obtener respuestas recientes
        public async Task<List<RecentResponseDto>> GetRecentResponsesAsync(int count)
        {
            var recentResponses = await _surveyResponseRepository.GetRecentResponsesAsync(count);
            var result = new List<RecentResponseDto>();
            
            foreach (var response in recentResponses)
            {
                // Obtener el título de la encuesta
                var survey = await _surveyRepository.GetByIdAsync(response.SurveyId);
                string surveyTitle = survey?.Title ?? "Encuesta sin título";
                
                result.Add(new RecentResponseDto
                {
                    Id = response.Id,
                    SurveyId = response.SurveyId,
                    SurveyTitle = surveyTitle,
                    RespondentName = response.RespondentName,
                    SubmittedAt = response.SubmittedAt
                });
            }
            
            return result;
        }

        private SurveyResponseDto MapToSurveyResponseDto(SurveyResponse response)
        {
            return new SurveyResponseDto
            {
                Id = response.Id,
                SurveyId = response.SurveyId,
                RespondentName = response.RespondentName,
                RespondentEmail = response.RespondentEmail,
                RespondentPhone = response.RespondentPhone,
                RespondentCompany = response.RespondentCompany,
                SubmittedAt = response.SubmittedAt,
                IsExistingClient = response.IsExistingClient,
                ExistingClientId = response.ExistingClientId,
                Answers = response.Answers.Select(a => new QuestionResponseDto
                {
                    QuestionId = a.QuestionId,
                    QuestionTitle = a.QuestionTitle,
                    QuestionType = a.QuestionType,
                    Answer = a.Answer,
                    MultipleAnswers = a.MultipleAnswers,
                    IsValid = a.IsValid
                }).ToList()
            };
        }

        // Método auxiliar para mapear Survey a SurveyDto
        private SurveyDto MapToSurveyDto(Survey survey)
        {
            return new SurveyDto
            {
                Id = survey.Id,
                Title = survey.Title,
                Description = survey.Description,
                CreatedAt = survey.CreatedAt,
                Responses = survey.Responses,
                CompletionRate = survey.CompletionRate,
                Questions = survey.Questions?.Select(q => new QuestionDto
                {
                    Id = q.Id,
                    Title = q.Title,
                    Description = q.Description,
                    Type = q.Type,
                    Required = q.Required,
                    Options = q.Options
                }).ToList(),
                DeliveryConfig = survey.DeliveryConfig != null ? new DeliveryConfigDto
                {
                    Type = survey.DeliveryConfig.Type,
                    EmailAddresses = survey.DeliveryConfig.EmailAddresses,
                    Schedule = survey.DeliveryConfig.Schedule != null ? new ScheduleDto
                    {
                        Frequency = survey.DeliveryConfig.Schedule.Frequency,
                        DayOfMonth = survey.DeliveryConfig.Schedule.DayOfMonth,
                        DayOfWeek = survey.DeliveryConfig.Schedule.DayOfWeek,
                        Time = survey.DeliveryConfig.Schedule.Time,
                        StartDate = survey.DeliveryConfig.Schedule.StartDate
                    } : null,
                    Trigger = survey.DeliveryConfig.Trigger != null ? new TriggerDto
                    {
                        Type = survey.DeliveryConfig.Trigger.Type,
                        DelayHours = survey.DeliveryConfig.Trigger.DelayHours,
                        SendAutomatically = survey.DeliveyConfig.Trigger.SendAutomatically
                    } : null
                } : null
            };
        }

        // Otras implementaciones requeridas por la interfaz ISurveyService deben ser agregadas aquí
        public async Task<SurveyDto> GetSurveyByIdAsync(Guid id)
        {
            var survey = await _surveyRepository.GetByIdAsync(id);
            if (survey == null)
                return null;

            return MapToSurveyDto(survey);
        }

        public async Task<List<SurveyDto>> GetAllSurveysAsync()
        {
            var surveys = await _surveyRepository.GetAllAsync();
            return surveys.Select(MapToSurveyDto).ToList();
        }

        public async Task UpdateSurveyAsync(Guid id, UpdateSurveyDto surveyDto)
        {
            var survey = await _surveyRepository.GetByIdAsync(id);
            if (survey == null)
                throw new KeyNotFoundException($"Survey with ID {id} not found");

            // Actualizar propiedades básicas
            survey.Title = surveyDto.Title;
            survey.Description = surveyDto.Description;
            
            // Actualizar configuración de entrega si existe
            if (surveyDto.DeliveryConfig != null)
            {
                var deliveryConfig = new DeliveryConfig
                {
                    Type = surveyDto.DeliveryConfig.Type,
                    EmailAddresses = surveyDto.DeliveryConfig.EmailAddresses
                };

                if (surveyDto.DeliveryConfig.Schedule != null)
                {
                    deliveryConfig.Schedule = new Schedule
                    {
                        Frequency = surveyDto.DeliveryConfig.Schedule.Frequency,
                        DayOfMonth = surveyDto.DeliveryConfig.Schedule.DayOfMonth,
                        DayOfWeek = surveyDto.DeliveryConfig.Schedule.DayOfWeek,
                        Time = surveyDto.DeliveryConfig.Schedule.Time,
                        StartDate = surveyDto.DeliveryConfig.Schedule.StartDate
                    };
                }

                if (surveyDto.DeliveryConfig.Trigger != null)
                {
                    deliveryConfig.Trigger = new Trigger
                    {
                        Type = surveyDto.DeliveryConfig.Trigger.Type,
                        DelayHours = surveyDto.DeliveryConfig.Trigger.DelayHours,
                        SendAutomatically = surveyDto.DeliveryConfig.Trigger.SendAutomatically
                    };
                }

                survey.SetDeliveryConfig(deliveryConfig);
            }

            await _surveyRepository.UpdateAsync(survey);
        }

        public async Task DeleteSurveyAsync(Guid id)
        {
            await _surveyRepository.DeleteAsync(id);
        }

        public async Task<bool> SurveyExistsAsync(Guid id)
        {
            var survey = await _surveyRepository.GetByIdAsync(id);
            return survey != null;
        }

        public async Task<List<string>> GetAllCategoriesAsync()
        {
            var surveys = await _surveyRepository.GetAllAsync();
            return surveys.Select(s => s.Category).Distinct().Where(c => !string.IsNullOrWhiteSpace(c)).ToList();
        }

        public async Task<(List<SurveyDto> Surveys, int TotalCount)> GetPagedSurveysAsync(int pageNumber, int pageSize, string searchTerm = null, string statusFilter = null, string categoryFilter = null)
        {
            var (surveys, totalCount) = await _surveyRepository.GetPagedAsync(pageNumber, pageSize, searchTerm, statusFilter, categoryFilter);
            var surveyDtos = surveys.Select(MapToSurveyDto).ToList();
            return (surveyDtos, totalCount);
        }
    }
}
