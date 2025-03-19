
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SurveyApp.Application.DTOs;

namespace SurveyApp.Application.Services
{
    // Similar suggestion search operations
    public partial class SuggestionService
    {
        public async Task<List<SuggestionDto>> FindSimilarSuggestionsAsync(string content)
        {
            if (string.IsNullOrWhiteSpace(content) || content.Length < 5)
                return new List<SuggestionDto>();

            // Simple keyword search
            var searchTerms = content.ToLower().Split(' ')
                .Where(term => term.Length > 3)
                .ToList();
            
            if (!searchTerms.Any())
                return new List<SuggestionDto>();

            var allSuggestions = await _suggestionRepository.GetAllAsync();
            
            var similarSuggestions = allSuggestions
                .Where(suggestion => {
                    var suggestionText = suggestion.Content.ToLower();
                    return searchTerms.Any(term => suggestionText.Contains(term));
                })
                .Take(5)
                .ToList();

            return similarSuggestions.Select(MapToDto).ToList();
        }
    }
}
