
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
        
        // Nuevos métodos para análisis avanzado
        Task<Dictionary<string, double>> GetCompletionRateByQuestionTypeAsync(Guid surveyId);
        Task<Dictionary<string, double>> GetAverageScoreByCategoryAsync(Guid surveyId);
        Task<Dictionary<string, int>> GetQuestionTypeDistributionAsync(Guid surveyId);
        Task<Dictionary<int, int>> GetNPSDistributionAsync(Guid surveyId);
        Task<Dictionary<int, int>> GetRatingDistributionAsync(Guid surveyId);
        Task<Dictionary<string, int>> GetDeviceDistributionAsync();
        Task<List<SurveyResponse>> GetResponsesWithFullDataAsync(Guid? surveyId = null);
    }
}
