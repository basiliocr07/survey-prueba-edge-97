
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SurveyApp.Application.DTOs;
using SurveyApp.Application.Ports;
using SurveyApp.Domain.Entities;

namespace SurveyApp.Application.Services
{
    public class SurveyResponseService : ISurveyResponseService
    {
        private readonly ISurveyResponseRepository _responseRepository;
        private readonly ISurveyRepository _surveyRepository;

        public SurveyResponseService(
            ISurveyResponseRepository responseRepository,
            ISurveyRepository surveyRepository)
        {
            _responseRepository = responseRepository;
            _surveyRepository = surveyRepository;
        }

        public async Task<IEnumerable<SurveyResponseDto>> GetAllResponsesAsync()
        {
            var responses = await _responseRepository.GetAllAsync();
            return responses.Select(MapToDto);
        }

        public async Task<IEnumerable<SurveyResponseDto>> GetResponsesBySurveyIdAsync(Guid surveyId)
        {
            var responses = await _responseRepository.GetBySurveyIdAsync(surveyId);
            return responses.Select(MapToDto);
        }

        public async Task<SurveyResponseDto?> GetResponseByIdAsync(Guid id)
        {
            var response = await _responseRepository.GetByIdAsync(id);
            return response != null ? MapToDto(response) : null;
        }

        public async Task<SurveyResponseDto> CreateResponseAsync(CreateSurveyResponseDto responseDto)
        {
            var survey = await _surveyRepository.GetByIdAsync(responseDto.SurveyId);
            if (survey == null)
            {
                throw new KeyNotFoundException($"Survey with id {responseDto.SurveyId} not found.");
            }

            var response = new SurveyResponse
            {
                SurveyId = responseDto.SurveyId,
                RespondentName = responseDto.RespondentName,
                RespondentEmail = responseDto.RespondentEmail,
                RespondentPhone = responseDto.RespondentPhone,
                RespondentCompany = responseDto.RespondentCompany,
                IsExistingClient = responseDto.IsExistingClient,
                ExistingClientId = responseDto.ExistingClientId
            };

            foreach (var question in survey.Questions)
            {
                if (responseDto.Answers.TryGetValue(question.Id.ToString(), out var answerObj))
                {
                    var answer = new QuestionResponse
                    {
                        QuestionId = question.Id,
                        QuestionTitle = question.Title,
                        QuestionType = question.Type
                    };

                    if (question.Type == "multiple-choice" || question.Type == "checkbox")
                    {
                        if (answerObj is List<string> multipleAnswers)
                        {
                            answer.MultipleAnswers = multipleAnswers;
                        }
                        else if (answerObj is string singleAnswer)
                        {
                            answer.Answer = singleAnswer;
                        }
                    }
                    else if (question.Type == "rating" || question.Type == "nps")
                    {
                        answer.Answer = answerObj?.ToString() ?? string.Empty;
                    }
                    else
                    {
                        answer.Answer = answerObj?.ToString() ?? string.Empty;
                    }

                    response.Answers.Add(answer);
                }
            }

            var createdResponse = await _responseRepository.CreateAsync(response);
            
            // Update survey statistics
            survey.IncrementResponses();
            await _surveyRepository.UpdateAsync(survey);
            
            return MapToDto(createdResponse);
        }

        public async Task<SurveyResponseAnalyticsDto> GetResponseAnalyticsAsync(Guid surveyId)
        {
            var responses = await _responseRepository.GetBySurveyIdAsync(surveyId);
            var analytics = new SurveyResponseAnalyticsDto
            {
                SurveyId = surveyId,
                TotalResponses = responses.Count(),
                AverageCompletionTime = responses.Any() ? responses.Average(r => r.CompletionTime) : 0,
                CompletionRate = 100, // Asumimos que todas las respuestas están completas
                ResponsesByDate = responses
                    .GroupBy(r => r.SubmittedAt.Date)
                    .Select(g => new { Date = g.Key, Count = g.Count() })
                    .ToDictionary(x => x.Date.ToString("yyyy-MM-dd"), x => x.Count)
            };

            // Agregar análisis por tipo de dispositivo
            var deviceCounts = responses
                .GroupBy(r => string.IsNullOrEmpty(r.DeviceType) ? "Unknown" : r.DeviceType)
                .Select(g => new { Device = g.Key, Count = g.Count() })
                .ToDictionary(x => x.Device, x => x.Count);
            
            analytics.DeviceBreakdown = deviceCounts;

            return analytics;
        }

        private SurveyResponseDto MapToDto(SurveyResponse response)
        {
            return new SurveyResponseDto
            {
                Id = response.Id,
                SurveyId = response.SurveyId,
                SurveyTitle = string.Empty, // This would need to be populated separately by joining with survey data
                RespondentName = response.RespondentName,
                RespondentEmail = response.RespondentEmail,
                RespondentPhone = response.RespondentPhone,
                RespondentCompany = response.RespondentCompany,
                SubmittedAt = response.SubmittedAt,
                Answers = response.Answers.Select(a => new QuestionResponseDto
                {
                    QuestionId = a.QuestionId,
                    QuestionTitle = a.QuestionTitle,
                    QuestionType = a.QuestionType,
                    Answer = a.Answer,
                    MultipleAnswers = a.MultipleAnswers
                }).ToList(),
                IsExistingClient = response.IsExistingClient,
                ExistingClientId = response.ExistingClientId,
                CompletionTime = response.CompletionTime,
                DeviceType = response.DeviceType,
                Browser = response.Browser,
                Location = response.Location
            };
        }

        // Métodos para exportar respuestas que aún no se utilizan
        /*
        public async Task<byte[]> ExportResponsesAsCsvAsync(Guid surveyId)
        {
            var responses = await GetResponsesBySurveyIdAsync(surveyId);
            // Aquí iría la lógica para convertir respuestas a CSV
            // Pendiente de implementar cuando se necesite esta funcionalidad
            return new byte[0];
        }

        public async Task<byte[]> ExportResponsesAsExcelAsync(Guid surveyId)
        {
            var responses = await GetResponsesBySurveyIdAsync(surveyId);
            // Aquí iría la lógica para convertir respuestas a Excel
            // Pendiente de implementar cuando se necesite esta funcionalidad
            return new byte[0];
        }
        */
    }
}
