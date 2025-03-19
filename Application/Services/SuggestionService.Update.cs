
using System;
using System.Threading.Tasks;
using SurveyApp.Domain.Entities;

namespace SurveyApp.Application.Services
{
    // Update operations for suggestions
    public partial class SuggestionService
    {
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
    }
}
