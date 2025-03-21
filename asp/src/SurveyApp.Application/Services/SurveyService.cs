
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

        public SurveyService(ISurveyRepository surveyRepository)
        {
            _surveyRepository = surveyRepository;
        }

        public async Task<IEnumerable<SurveyDto>> GetAllSurveysAsync()
        {
            var surveys = await _surveyRepository.GetAllAsync();
            return surveys.Select(MapToDto);
        }

        public async Task<SurveyDto?> GetSurveyByIdAsync(Guid id)
        {
            var survey = await _surveyRepository.GetByIdAsync(id);
            return survey != null ? MapToDto(survey) : null;
        }

        public async Task<SurveyDto> CreateSurveyAsync(CreateSurveyDto surveyDto)
        {
            var survey = new Survey
            {
                Title = surveyDto.Title,
                Description = surveyDto.Description,
                Category = !string.IsNullOrWhiteSpace(surveyDto.Category) ? surveyDto.Category : "General",
                ExpiryDate = surveyDto.ExpiryDate,
                AllowAnonymousResponses = surveyDto.AllowAnonymousResponses,
                LimitOneResponsePerUser = surveyDto.LimitOneResponsePerUser,
                ThankYouMessage = !string.IsNullOrWhiteSpace(surveyDto.ThankYouMessage) 
                    ? surveyDto.ThankYouMessage 
                    : "¡Gracias por completar nuestra encuesta!"
            };

            // Add questions
            if (surveyDto.Questions != null)
            {
                foreach (var questionDto in surveyDto.Questions)
                {
                    var question = new Question
                    {
                        Type = questionDto.Type,
                        Title = questionDto.Title,
                        Description = questionDto.Description,
                        Required = questionDto.Required,
                        Options = questionDto.Options?.ToList() ?? new List<string>()
                    };

                    if (questionDto.Settings != null)
                    {
                        question.Settings = new QuestionSettings
                        {
                            MinValue = questionDto.Settings.MinValue,
                            MaxValue = questionDto.Settings.MaxValue,
                            LowLabel = questionDto.Settings.LowLabel,
                            MiddleLabel = questionDto.Settings.MiddleLabel,
                            HighLabel = questionDto.Settings.HighLabel
                        };
                    }

                    survey.AddQuestion(question);
                }
            }

            // Set delivery config
            if (surveyDto.DeliveryConfig != null)
            {
                var deliveryConfig = new DeliveryConfig
                {
                    Type = surveyDto.DeliveryConfig.Type,
                    EmailAddresses = surveyDto.DeliveryConfig.EmailAddresses?.ToList() ?? new List<string>()
                };

                if (surveyDto.DeliveryConfig.Schedule != null)
                {
                    deliveryConfig.Schedule = new Schedule
                    {
                        Frequency = surveyDto.DeliveryConfig.Schedule.Frequency,
                        DayOfMonth = surveyDto.DeliveryConfig.Schedule.DayOfMonth,
                        DayOfWeek = surveyDto.DeliveryConfig.Schedule.DayOfWeek,
                        Time = surveyDto.DeliveryConfig.Schedule.Time,
                        StartDate = surveyDto.DeliveryConfig.Schedule.StartDate,
                        EndDate = surveyDto.DeliveryConfig.Schedule.EndDate
                    };
                }

                if (surveyDto.DeliveryConfig.Trigger != null)
                {
                    deliveryConfig.Trigger = new Trigger
                    {
                        Type = surveyDto.DeliveryConfig.Trigger.Type,
                        DelayHours = surveyDto.DeliveryConfig.Trigger.DelayHours,
                        SendAutomatically = surveyDto.DeliveryConfig.Trigger.SendAutomatically,
                        EventName = surveyDto.DeliveryConfig.Trigger.EventName
                    };
                }

                survey.SetDeliveryConfig(deliveryConfig);
            }

            var createdSurvey = await _surveyRepository.CreateAsync(survey);
            return MapToDto(createdSurvey);
        }

        public async Task UpdateSurveyAsync(Guid id, CreateSurveyDto surveyDto)
        {
            var existingSurvey = await _surveyRepository.GetByIdAsync(id);
            if (existingSurvey == null)
            {
                throw new KeyNotFoundException($"Survey with id {id} not found.");
            }

            existingSurvey.Title = surveyDto.Title;
            existingSurvey.Description = surveyDto.Description;
            existingSurvey.UpdateCategory(surveyDto.Category);
            existingSurvey.ExpiryDate = surveyDto.ExpiryDate;
            existingSurvey.AllowAnonymousResponses = surveyDto.AllowAnonymousResponses;
            existingSurvey.LimitOneResponsePerUser = surveyDto.LimitOneResponsePerUser;
            existingSurvey.ThankYouMessage = !string.IsNullOrWhiteSpace(surveyDto.ThankYouMessage) 
                ? surveyDto.ThankYouMessage 
                : "¡Gracias por completar nuestra encuesta!";

            // Replace questions
            existingSurvey.Questions.Clear();
            if (surveyDto.Questions != null)
            {
                foreach (var questionDto in surveyDto.Questions)
                {
                    var question = new Question
                    {
                        Type = questionDto.Type,
                        Title = questionDto.Title,
                        Description = questionDto.Description,
                        Required = questionDto.Required,
                        Options = questionDto.Options?.ToList() ?? new List<string>()
                    };

                    if (questionDto.Settings != null)
                    {
                        question.Settings = new QuestionSettings
                        {
                            MinValue = questionDto.Settings.MinValue,
                            MaxValue = questionDto.Settings.MaxValue,
                            LowLabel = questionDto.Settings.LowLabel,
                            MiddleLabel = questionDto.Settings.MiddleLabel,
                            HighLabel = questionDto.Settings.HighLabel
                        };
                    }

                    existingSurvey.AddQuestion(question);
                }
            }

            // Update delivery config
            if (surveyDto.DeliveryConfig != null)
            {
                var deliveryConfig = new DeliveryConfig
                {
                    Type = surveyDto.DeliveryConfig.Type,
                    EmailAddresses = surveyDto.DeliveryConfig.EmailAddresses?.ToList() ?? new List<string>()
                };

                if (surveyDto.DeliveryConfig.Schedule != null)
                {
                    deliveryConfig.Schedule = new Schedule
                    {
                        Frequency = surveyDto.DeliveryConfig.Schedule.Frequency,
                        DayOfMonth = surveyDto.DeliveryConfig.Schedule.DayOfMonth,
                        DayOfWeek = surveyDto.DeliveryConfig.Schedule.DayOfWeek,
                        Time = surveyDto.DeliveryConfig.Schedule.Time,
                        StartDate = surveyDto.DeliveryConfig.Schedule.StartDate,
                        EndDate = surveyDto.DeliveryConfig.Schedule.EndDate
                    };
                }

                if (surveyDto.DeliveryConfig.Trigger != null)
                {
                    deliveryConfig.Trigger = new Trigger
                    {
                        Type = surveyDto.DeliveryConfig.Trigger.Type,
                        DelayHours = surveyDto.DeliveryConfig.Trigger.DelayHours,
                        SendAutomatically = surveyDto.DeliveryConfig.Trigger.SendAutomatically,
                        EventName = surveyDto.DeliveryConfig.Trigger.EventName
                    };
                }

                existingSurvey.SetDeliveryConfig(deliveryConfig);
            }
            else
            {
                existingSurvey.DeliveryConfig = null;
            }

            await _surveyRepository.UpdateAsync(existingSurvey);
        }

        public async Task DeleteSurveyAsync(Guid id)
        {
            await _surveyRepository.DeleteAsync(id);
        }

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
                Status = survey.Status,
                Category = survey.Category,
                ExpiryDate = survey.ExpiryDate,
                AllowAnonymousResponses = survey.AllowAnonymousResponses,
                LimitOneResponsePerUser = survey.LimitOneResponsePerUser,
                ThankYouMessage = survey.ThankYouMessage,
                Questions = survey.Questions.Select(q => new QuestionDto
                {
                    Id = q.Id,
                    Type = q.Type,
                    Title = q.Title,
                    Description = q.Description,
                    Required = q.Required,
                    Options = q.Options,
                    Settings = q.Settings != null ? new QuestionSettingsDto
                    {
                        MinValue = q.Settings.MinValue,
                        MaxValue = q.Settings.MaxValue,
                        LowLabel = q.Settings.LowLabel,
                        MiddleLabel = q.Settings.MiddleLabel,
                        HighLabel = q.Settings.HighLabel
                    } : null
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
                        StartDate = survey.DeliveryConfig.Schedule.StartDate,
                        EndDate = survey.DeliveryConfig.Schedule.EndDate
                    } : null,
                    Trigger = survey.DeliveryConfig.Trigger != null ? new TriggerDto
                    {
                        Type = survey.DeliveryConfig.Trigger.Type,
                        DelayHours = survey.DeliveryConfig.Trigger.DelayHours,
                        SendAutomatically = survey.DeliveryConfig.Trigger.SendAutomatically,
                        EventName = survey.DeliveryConfig.Trigger.EventName
                    } : null
                } : null
            };
        }
    }
}
