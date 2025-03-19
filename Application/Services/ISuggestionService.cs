
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SurveyApp.Application.DTOs;

namespace SurveyApp.Application.Services
{
    public interface ISuggestionService
    {
        Task<SuggestionDto> GetSuggestionByIdAsync(Guid id);
        Task<List<SuggestionDto>> GetAllSuggestionsAsync();
        Task<SuggestionDto> CreateSuggestionAsync(CreateSuggestionDto suggestionDto);
        Task UpdateSuggestionAsync(Guid id, UpdateSuggestionDto suggestionDto);
        Task DeleteSuggestionAsync(Guid id);
        Task UpdateSuggestionStatusAsync(Guid id, string status);
        Task<List<SuggestionDto>> GetRecentSuggestionsAsync(int count);
    }
}
