
using System;
using System.Threading.Tasks;
using SurveyApp.Application.DTOs;
using SurveyApp.Domain.Entities;

namespace SurveyApp.Application.Services
{
    // Create operations for suggestions
    public partial class SuggestionService
    {
        public async Task<SuggestionDto> CreateSuggestionAsync(CreateSuggestionDto suggestionDto)
        {
            var suggestion = new Suggestion
            {
                Id = Guid.NewGuid(),
                Content = suggestionDto.Content,
                CustomerName = suggestionDto.IsAnonymous ? "Anonymous" : suggestionDto.CustomerName,
                CustomerEmail = suggestionDto.CustomerEmail,
                CreatedAt = DateTime.UtcNow,
                Status = SuggestionStatus.New,
                Category = suggestionDto.Category ?? "Other",
                IsAnonymous = suggestionDto.IsAnonymous,
                Title = suggestionDto.Title,
                Description = suggestionDto.Description,
                Source = suggestionDto.Source,
                CustomerId = suggestionDto.CustomerId
            };

            await _suggestionRepository.CreateAsync(suggestion);
            
            // Send notification email to admin
            await _emailService.SendEmailAsync(
                "admin@surveyapp.com",
                "New Suggestion Received",
                $"A new suggestion has been received from {suggestion.CustomerName}: {suggestion.Content}"
            );

            return MapToDto(suggestion);
        }
    }
}
