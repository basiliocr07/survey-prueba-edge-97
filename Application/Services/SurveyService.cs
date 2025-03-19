
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
    }
}
