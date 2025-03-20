
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
        
        // Métodos para análisis avanzado
        Task<Dictionary<string, double>> GetCompletionRateByQuestionTypeAsync(Guid surveyId);
        Task<Dictionary<string, double>> GetAverageScoreByCategoryAsync(Guid surveyId);
        Task<Dictionary<string, int>> GetQuestionTypeDistributionAsync(Guid surveyId);
        Task<Dictionary<int, int>> GetNPSDistributionAsync(Guid surveyId);
        Task<Dictionary<int, int>> GetRatingDistributionAsync(Guid surveyId);
        Task<Dictionary<string, int>> GetDeviceDistributionAsync();
        Task<List<SurveyResponse>> GetResponsesWithFullDataAsync(Guid? surveyId = null);
        
        // Métodos adicionales para completar la funcionalidad de análisis
        Task<Dictionary<string, int>> GetBrowserDistributionAsync(Guid? surveyId = null);
        Task<Dictionary<string, int>> GetOperatingSystemDistributionAsync(Guid? surveyId = null);
        Task<Dictionary<string, int>> GetLocationDistributionAsync(Guid? surveyId = null);
        Task<Dictionary<DateTime, int>> GetResponsesOverTimeAsync(Guid surveyId, DateTime startDate, DateTime endDate);
        Task<double> GetAverageCompletionTimeAsync(Guid surveyId);
        Task<Dictionary<string, double>> GetAverageTimePerQuestionTypeAsync(Guid surveyId);
        Task<int> GetAbandonmentCountAsync(Guid surveyId);
        Task<double> GetAbandonmentRateAsync(Guid surveyId);
        Task<Dictionary<int, int>> GetPageViewsDistributionAsync(Guid surveyId);
        Task<Dictionary<string, int>> GetSourceDistributionAsync(Guid? surveyId = null);
        
        // Método faltante que se usa en GetGlobalAnalyticsDashboardAsync
        Task<int> GetTotalResponseCountAsync();
    }
}
