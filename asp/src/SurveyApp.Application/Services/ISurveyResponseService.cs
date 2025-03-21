
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SurveyApp.Application.DTOs;
using SurveyApp.Domain.Entities;

namespace SurveyApp.Application.Services
{
    public interface ISurveyResponseService
    {
        Task<List<SurveyResponseDto>> GetAllSurveyResponsesAsync();
        Task<List<SurveyResponseDto>> GetSurveyResponsesByIdAsync(Guid surveyId);
        Task<SurveyResponseDto> GetSurveyResponseByIdAsync(Guid id);
        Task<int> GetSurveyResponseCountAsync(Guid surveyId);
        Task<SurveyResponseDto> AddSurveyResponseAsync(CreateSurveyResponseDto surveyResponseDto);
        Task UpdateSurveyResponseAsync(SurveyResponseDto surveyResponseDto);
        Task DeleteSurveyResponseAsync(Guid id);
        
        // Analytics methods
        Task<Dictionary<string, int>> GetDeviceDistributionAsync(Guid? surveyId = null);
        Task<Dictionary<string, int>> GetBrowserDistributionAsync(Guid? surveyId = null);
        Task<Dictionary<string, int>> GetLocationDistributionAsync(Guid? surveyId = null);
        Task<double> GetAverageCompletionTimeAsync(Guid surveyId);
        Task<int> GetAbandonmentCountAsync(Guid surveyId);
        Task<double> GetAbandonmentRateAsync(Guid surveyId);
        Task<Dictionary<DateTime, int>> GetResponsesOverTimeAsync(Guid surveyId, DateTime startDate, DateTime endDate);
        
        // Advanced analysis
        Task<Dictionary<string, double>> GetCompletionRateByQuestionTypeAsync(Guid surveyId);
        Task<Dictionary<string, double>> GetAverageScoreByCategoryAsync(Guid surveyId);
        Task<List<SurveyResponseAnalyticsDto>> GetDetailedResponseAnalyticsAsync(Guid surveyId);
    }
}
