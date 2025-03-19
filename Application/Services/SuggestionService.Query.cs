
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SurveyApp.Application.DTOs;
using SurveyApp.Domain.Entities;

namespace SurveyApp.Application.Services
{
    // Query methods for retrieving suggestions
    public partial class SuggestionService
    {
        public async Task<List<SuggestionDto>> GetAllSuggestionsAsync()
        {
            var suggestions = await _suggestionRepository.GetAllAsync();
            return suggestions.Select(MapToDto).ToList();
        }

        public async Task<SuggestionDto> GetSuggestionByIdAsync(Guid id)
        {
            var suggestion = await _suggestionRepository.GetByIdAsync(id);
            return suggestion != null ? MapToDto(suggestion) : null;
        }

        public async Task<List<SuggestionDto>> GetSuggestionsByStatusAsync(SuggestionStatus status)
        {
            var suggestions = await _suggestionRepository.GetByStatusAsync(status);
            return suggestions.Select(MapToDto).ToList();
        }

        public async Task<List<SuggestionDto>> GetSuggestionsByCategoryAsync(string category)
        {
            var suggestions = await _suggestionRepository.GetByCategoryAsync(category);
            return suggestions.Select(MapToDto).ToList();
        }

        public async Task<List<SuggestionDto>> SearchSuggestionsAsync(string searchTerm)
        {
            var suggestions = await _suggestionRepository.SearchAsync(searchTerm);
            return suggestions.Select(MapToDto).ToList();
        }
    }
}
