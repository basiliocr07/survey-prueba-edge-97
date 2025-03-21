
using System;
using System.Collections.Generic;
using System.Linq;
using SurveyApp.Application.DTOs;
using SurveyApp.Application.Ports;

namespace SurveyApp.Application.Services
{
    public class SurveyResponseService : ISurveyResponseService
    {
        private readonly ISurveyResponseRepository _surveyResponseRepository;
        
        public SurveyResponseService(ISurveyResponseRepository surveyResponseRepository)
        {
            _surveyResponseRepository = surveyResponseRepository;
        }
        
        public List<SurveyResponseDto> GetAllSurveyResponses()
        {
            var responses = _surveyResponseRepository.GetAll();
            return responses.Select(MapToDto).ToList();
        }
        
        public List<SurveyResponseDto> GetSurveyResponsesById(Guid surveyId)
        {
            var responses = _surveyResponseRepository.GetBySurveyId(surveyId);
            return responses.Select(MapToDto).ToList();
        }
        
        public SurveyResponseDto GetSurveyResponseById(Guid id)
        {
            var response = _surveyResponseRepository.GetById(id);
            return response != null ? MapToDto(response) : null;
        }
        
        public int GetSurveyResponseCount(Guid surveyId)
        {
            return _surveyResponseRepository.GetCountBySurveyId(surveyId);
        }
        
        public void AddSurveyResponse(SurveyResponseDto surveyResponseDto)
        {
            var response = MapFromDto(surveyResponseDto);
            _surveyResponseRepository.Add(response);
        }
        
        public void UpdateSurveyResponse(SurveyResponseDto surveyResponseDto)
        {
            var response = MapFromDto(surveyResponseDto);
            _surveyResponseRepository.Update(response);
        }
        
        public void DeleteSurveyResponse(Guid id)
        {
            _surveyResponseRepository.Delete(id);
        }
        
        private SurveyResponseDto MapToDto(Domain.Entities.SurveyResponse response)
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
                CompletionTime = response.CompletionTime,
                Answers = response.QuestionResponses.Select(qr => new QuestionResponseDto
                {
                    QuestionId = qr.QuestionId,
                    Answer = qr.Answer
                }).ToList()
            };
        }
        
        private Domain.Entities.SurveyResponse MapFromDto(SurveyResponseDto dto)
        {
            return new Domain.Entities.SurveyResponse
            {
                Id = dto.Id != Guid.Empty ? dto.Id : Guid.NewGuid(),
                SurveyId = dto.SurveyId,
                RespondentName = dto.RespondentName,
                RespondentEmail = dto.RespondentEmail,
                RespondentPhone = dto.RespondentPhone,
                RespondentCompany = dto.RespondentCompany,
                SubmittedAt = dto.SubmittedAt,
                CompletionTime = dto.CompletionTime,
                QuestionResponses = dto.Answers.Select(a => new Domain.Entities.QuestionResponse
                {
                    QuestionId = a.QuestionId,
                    Answer = a.Answer
                }).ToList()
            };
        }
    }
}
