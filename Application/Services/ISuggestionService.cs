
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SurveyApp.Application.DTOs;
using SurveyApp.Domain.Entities;

namespace SurveyApp.Application.Services
{
    public interface ISuggestionService
    {
        Task<SuggestionDto> GetSuggestionByIdAsync(Guid id);
        Task<List<SuggestionDto>> GetAllSuggestionsAsync();
        Task<SuggestionDto> CreateSuggestionAsync(CreateSuggestionDto suggestionDto);
        Task UpdateSuggestionStatusAsync(Guid id, SuggestionStatus status, string response = null);
        Task<List<SuggestionDto>> GetSuggestionsByStatusAsync(SuggestionStatus status);
        Task<List<SuggestionDto>> GetSuggestionsByCategoryAsync(string category);
        Task<List<SuggestionDto>> SearchSuggestionsAsync(string searchTerm);
        Task<MonthlyReportDto> GenerateMonthlyReportAsync(int months);
        Task<List<SuggestionDto>> FindSimilarSuggestionsAsync(string content);
    }
}
