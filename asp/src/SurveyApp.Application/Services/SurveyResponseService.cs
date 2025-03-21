
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

        private SurveyResponseDto MapToDto(SurveyResponse response)
        {
            return new SurveyResponseDto
            {
                Id = response.Id,
                SurveyId = response.SurveyId,
                SurveyTitle = string.Empty, // This would need to be populated separately by joining with survey data
                RespondentName = response.RespondentName,
                RespondentEmail = response.RespondentEmail,
                SubmittedAt = response.SubmittedAt,
                Answers = response.Answers.Select(a => new QuestionResponseDto
                {
                    QuestionId = a.QuestionId,
                    QuestionTitle = a.QuestionTitle,
                    QuestionType = a.QuestionType,
                    Answer = a.Answer,
                    MultipleAnswers = a.MultipleAnswers
                }).ToList()
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
