
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SurveyApp.Domain.Entities;

namespace SurveyApp.Application.Ports
{
    public interface ISurveyResponseRepository
    {
        Task<SurveyResponse> GetByIdAsync(Guid id);
        Task<List<SurveyResponse>> GetBySurveyIdAsync(Guid surveyId);
        Task<SurveyResponse> CreateAsync(SurveyResponse response);
        Task<int> GetResponseCountForSurveyAsync(Guid surveyId);
        Task<Dictionary<string, int>> GetQuestionResponseStatisticsAsync(Guid surveyId, Guid questionId);
        Task<List<SurveyResponse>> GetRecentResponsesAsync(int count);
    }
}
