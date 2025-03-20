
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
    }
}
