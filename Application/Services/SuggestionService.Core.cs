
using System;
using System.Threading.Tasks;
using SurveyApp.Application.DTOs;
using SurveyApp.Application.Ports;
using SurveyApp.Domain.Entities;

namespace SurveyApp.Application.Services
{
    // Core functionality and constructor
    public partial class SuggestionService : ISuggestionService
    {
        private readonly ISuggestionRepository _suggestionRepository;
        private readonly IEmailService _emailService;

        public SuggestionService(ISuggestionRepository suggestionRepository, IEmailService emailService)
        {
            _suggestionRepository = suggestionRepository;
            _emailService = emailService;
        }

        // DTO mapping helper - common functionality used by multiple methods
        internal SuggestionDto MapToDto(Suggestion suggestion)
        {
            // Calculate completion percentage based on status
            int completionPercentage = 0;
            switch (suggestion.Status)
            {
                case SuggestionStatus.New:
                    completionPercentage = 0;
                    break;
                case SuggestionStatus.Reviewed:
                    completionPercentage = 50;
                    break;
                case SuggestionStatus.Implemented:
                case SuggestionStatus.Rejected:
                    completionPercentage = 100;
                    break;
            }

            return new SuggestionDto
            {
                Id = suggestion.Id,
                Content = suggestion.Content,
                CustomerName = suggestion.CustomerName,
                CustomerEmail = suggestion.CustomerEmail,
                CreatedAt = suggestion.CreatedAt,
                Status = suggestion.Status.ToString().ToLower(),
                Category = suggestion.Category,
                Priority = suggestion.Priority?.ToString().ToLower(),
                IsAnonymous = suggestion.IsAnonymous,
                Response = suggestion.Response,
                ResponseDate = suggestion.ResponseDate,
                SimilarSuggestions = suggestion.SimilarSuggestions,
                Title = !string.IsNullOrEmpty(suggestion.Title) 
                    ? suggestion.Title 
                    : (!string.IsNullOrEmpty(suggestion.Content) 
                        ? (suggestion.Content.Length > 50 
                            ? suggestion.Content.Substring(0, 47) + "..." 
                            : suggestion.Content)
                        : "Sin título"),
                Description = suggestion.Description ?? suggestion.Content,
                Source = suggestion.Source ?? (suggestion.IsAnonymous ? "Anonymous" : "Customer"),
                CompletionPercentage = completionPercentage,
                TargetDate = suggestion.TargetDate,
                AcceptanceCriteria = suggestion.AcceptanceCriteria
            };
        }
    }
}
