
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SurveyApp.Application.Ports;
using SurveyApp.Domain.Entities;

namespace SurveyApp.Application.Services
{
    public class SurveyService : ISurveyService
    {
        private readonly ISurveyRepository _surveyRepository;
        private readonly ILogger<SurveyService> _logger;

        public SurveyService(ISurveyRepository surveyRepository, ILogger<SurveyService> logger)
        {
            _surveyRepository = surveyRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Survey>> GetAllSurveysAsync()
        {
            try
            {
                return await _surveyRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all surveys");
                throw;
            }
        }

        public async Task<Survey> GetSurveyByIdAsync(Guid id)
        {
            try
            {
                var survey = await _surveyRepository.GetByIdAsync(id);
                if (survey == null)
                {
                    throw new Exception($"Survey with ID {id} not found");
                }
                return survey;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while getting survey with ID {id}");
                throw;
            }
        }

        public async Task<Survey> CreateSurveyAsync(string title, string description, string category)
        {
            try
            {
                var survey = new Survey(title, description, category);
                return await _surveyRepository.CreateAsync(survey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a new survey");
                throw;
            }
        }

        public async Task UpdateSurveyAsync(Guid id, string title, string description, string category)
        {
            try
            {
                var survey = await _surveyRepository.GetByIdAsync(id);
                if (survey == null)
                {
                    throw new Exception($"Survey with ID {id} not found");
                }

                survey.UpdateTitle(title);
                survey.UpdateDescription(description);
                survey.UpdateCategory(category);

                await _surveyRepository.UpdateAsync(survey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating survey with ID {id}");
                throw;
            }
        }

        public async Task DeleteSurveyAsync(Guid id)
        {
            try
            {
                var survey = await _surveyRepository.GetByIdAsync(id);
                if (survey == null)
                {
                    throw new Exception($"Survey with ID {id} not found");
                }

                await _surveyRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting survey with ID {id}");
                throw;
            }
        }

        public async Task AddQuestionToSurveyAsync(Guid surveyId, Question question)
        {
            try
            {
                var survey = await _surveyRepository.GetByIdAsync(surveyId);
                if (survey == null)
                {
                    throw new Exception($"Survey with ID {surveyId} not found");
                }

                survey.AddQuestion(question);
                await _surveyRepository.UpdateAsync(survey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while adding question to survey with ID {surveyId}");
                throw;
            }
        }

        public async Task PublishSurveyAsync(Guid id)
        {
            try
            {
                var survey = await _surveyRepository.GetByIdAsync(id);
                if (survey == null)
                {
                    throw new Exception($"Survey with ID {id} not found");
                }

                survey.PublishSurvey();
                await _surveyRepository.UpdateAsync(survey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while publishing survey with ID {id}");
                throw;
            }
        }

        // Método para analytics que aún no se utiliza
        /*
        public async Task<SurveyAnalyticsDto> GetSurveyAnalyticsAsync(Guid id)
        {
            try
            {
                var survey = await _surveyRepository.GetByIdAsync(id);
                if (survey == null)
                {
                    throw new Exception($"Survey with ID {id} not found");
                }

                // Aquí iría la lógica para obtener y calcular las métricas de analytics
                // Pendiente de implementar cuando se utilice

                return new SurveyAnalyticsDto 
                {
                    SurveyId = id,
                    ResponseCount = survey.ResponseCount,
                    // Otras métricas
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while getting analytics for survey with ID {id}");
                throw;
            }
        }
        */
    }
}
