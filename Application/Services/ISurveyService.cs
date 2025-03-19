
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SurveyApp.Application.DTOs;

namespace SurveyApp.Application.Services
{
    public interface ISurveyService
    {
        Task<SurveyDto> GetSurveyByIdAsync(Guid id);
        Task<List<SurveyDto>> GetAllSurveysAsync();
        Task<SurveyDto> CreateSurveyAsync(CreateSurveyDto surveyDto);
        Task UpdateSurveyAsync(Guid id, UpdateSurveyDto surveyDto);
        Task DeleteSurveyAsync(Guid id);
        Task<bool> SurveyExistsAsync(Guid id);
        Task<List<string>> GetAllCategoriesAsync();
        Task<(List<SurveyDto> Surveys, int TotalCount)> GetPagedSurveysAsync(int pageNumber, int pageSize, string searchTerm = null, string statusFilter = null, string categoryFilter = null);
        Task UpdateSurveyStatusAsync(Guid id, string status);
        
        // Métodos para respuestas
        Task<SurveyResponseDto> SubmitSurveyResponseAsync(CreateSurveyResponseDto responseDto);
        Task<List<SurveyResponseDto>> GetSurveyResponsesAsync(Guid surveyId);
        
        // Nuevo método para obtener respuestas recientes
        Task<List<RecentResponseDto>> GetRecentResponsesAsync(int count);
    }
}
