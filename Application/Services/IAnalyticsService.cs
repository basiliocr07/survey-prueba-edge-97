
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SurveyApp.Application.DTOs;

namespace SurveyApp.Application.Services
{
    public interface IAnalyticsService
    {
        Task<AnalyticsDto> GetAnalyticsDataAsync();
        Task RefreshAnalyticsDataAsync();
        Task<SurveyResponseAnalyticsDto> GetResponseAnalyticsAsync(Guid responseId);
        Task<List<SurveyResponseAnalyticsDto>> GetResponsesAnalyticsAsync(Guid? surveyId = null);
        Task<Dictionary<string, object>> GetSurveyAnalyticsDashboardAsync(Guid surveyId);
        
        // Métodos adicionales para completar la funcionalidad de la vista
        Task<Dictionary<string, object>> GetGlobalAnalyticsDashboardAsync();
        Task<Dictionary<string, object>> GetUserEngagementMetricsAsync(Guid? surveyId = null);
        Task<Dictionary<string, object>> GetDeviceAnalyticsAsync(Guid? surveyId = null);
        Task<Dictionary<DateTime, int>> GetResponseTrendsAsync(Guid? surveyId = null, string timeRange = "last30days");
        Task<Dictionary<string, object>> GetCompletionAnalyticsAsync(Guid surveyId);
        Task<Dictionary<string, object>> GetQuestionPerformanceAnalyticsAsync(Guid surveyId);
        Task ExportAnalyticsDataAsync(Guid? surveyId, string format, bool includeRawData, string timeRange);
    }
}
