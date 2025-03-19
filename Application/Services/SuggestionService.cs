
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SurveyApp.Application.DTOs;
using SurveyApp.Application.Ports;
using SurveyApp.Domain.Entities;

namespace SurveyApp.Application.Services
{
    public class SuggestionService : ISuggestionService
    {
        private readonly ISuggestionRepository _suggestionRepository;
        private readonly IEmailService _emailService;

        public SuggestionService(ISuggestionRepository suggestionRepository, IEmailService emailService)
        {
            _suggestionRepository = suggestionRepository;
            _emailService = emailService;
        }

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

        public async Task UpdateSuggestionStatusAsync(Guid id, SuggestionStatus status, string response = null)
        {
            var suggestion = await _suggestionRepository.GetByIdAsync(id);
            if (suggestion != null)
            {
                suggestion.Status = status;
                
                if (!string.IsNullOrEmpty(response))
                {
                    suggestion.Response = response;
                    suggestion.ResponseDate = DateTime.UtcNow;
                }

                await _suggestionRepository.UpdateAsync(suggestion);

                // Notify customer if not anonymous and there's a response
                if (!suggestion.IsAnonymous && !string.IsNullOrEmpty(response))
                {
                    await _emailService.SendEmailAsync(
                        suggestion.CustomerEmail,
                        $"Update on Your Suggestion: {status}",
                        $"Your suggestion has been updated to status: {status}.\n\nResponse: {response}"
                    );
                }
            }
        }

        public async Task<MonthlyReportDto> GenerateMonthlyReportAsync(int months)
        {
            var endDate = DateTime.UtcNow;
            var startDate = endDate.AddMonths(-months);
            
            var allSuggestions = await _suggestionRepository.GetAllAsync();
            var filteredSuggestions = allSuggestions.Where(s => s.CreatedAt >= startDate && s.CreatedAt <= endDate).ToList();
            
            var implementedCount = filteredSuggestions.Count(s => s.Status == SuggestionStatus.Implemented);

            // Group by month
            var monthlyData = filteredSuggestions
                .GroupBy(s => new { Month = s.CreatedAt.Month, Year = s.CreatedAt.Year })
                .Select(g => new MonthlyDataDto
                {
                    Month = g.Key.Month.ToString(),
                    Year = g.Key.Year,
                    TotalSuggestions = g.Count(),
                    ImplementedSuggestions = g.Count(s => s.Status == SuggestionStatus.Implemented)
                })
                .OrderByDescending(m => m.Year)
                .ThenByDescending(m => int.Parse(m.Month))
                .ToList();

            // Get top categories
            var categoryGroups = filteredSuggestions
                .Where(s => !string.IsNullOrEmpty(s.Category))
                .GroupBy(s => s.Category)
                .Select(g => new CategoryCountDto { Category = g.Key, Count = g.Count() })
                .OrderByDescending(c => c.Count)
                .Take(5)
                .ToList();

            return new MonthlyReportDto
            {
                TotalSuggestions = filteredSuggestions.Count,
                ImplementedSuggestions = implementedCount,
                TopCategories = categoryGroups,
                MonthlyData = monthlyData,
                Suggestions = filteredSuggestions.Select(MapToDto).ToList()
            };
        }

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

        private SuggestionDto MapToDto(Suggestion suggestion)
        {
            // Calcular el porcentaje de completación basado en el estado
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
                    completionPercentage = 100;
                    break;
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
                
                // Asignar valores a las nuevas propiedades
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
