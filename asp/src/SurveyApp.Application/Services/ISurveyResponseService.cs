
using System;
using System.Collections.Generic;
using SurveyApp.Application.DTOs;

namespace SurveyApp.Application.Services
{
    public interface ISurveyResponseService
    {
        List<SurveyResponseDto> GetAllSurveyResponses();
        List<SurveyResponseDto> GetSurveyResponsesById(Guid surveyId);
        SurveyResponseDto GetSurveyResponseById(Guid id);
        int GetSurveyResponseCount(Guid surveyId);
        void AddSurveyResponse(SurveyResponseDto surveyResponseDto);
        void UpdateSurveyResponse(SurveyResponseDto surveyResponseDto);
        void DeleteSurveyResponse(Guid id);
    }
}
