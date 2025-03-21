
using System;

namespace SurveyApp.Domain.Entities
{
    public class Suggestion
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public DateTime CreatedAt { get; set; }
        public SuggestionStatus Status { get; set; }
        public string Category { get; set; }
        public string Response { get; set; }
        public bool IsAnonymous { get; set; }
        public string Priority { get; set; }
        public string[] SimilarSuggestions { get; set; } = Array.Empty<string>();

        // For EF Core
        private Suggestion() { }

        public Suggestion(string content, string customerName, string customerEmail, string category = null, bool isAnonymous = false)
        {
            Id = Guid.NewGuid();
            Content = content;
            CustomerName = customerName;
            CustomerEmail = customerEmail;
            CreatedAt = DateTime.UtcNow;
            Status = SuggestionStatus.New;
            Category = category;
            IsAnonymous = isAnonymous;
        }

        public void UpdateStatus(SuggestionStatus status)
        {
            Status = status;
        }

        public void AddResponse(string response)
        {
            Response = response;
            if (Status == SuggestionStatus.New)
            {
                Status = SuggestionStatus.Reviewed;
            }
        }

        public void SetPriority(string priority)
        {
            Priority = priority;
        }

        public void UpdateCategory(string category)
        {
            Category = category;
        }

        public void SetSimilarSuggestions(string[] similarSuggestions)
        {
            SimilarSuggestions = similarSuggestions;
        }
    }
}
