
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
        public SuggestionPriority? Priority { get; set; }
        public bool IsAnonymous { get; set; }
        public string Response { get; set; }
        public DateTime? ResponseDate { get; set; }
        public string[] SimilarSuggestions { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Source { get; set; }
        public int CompletionPercentage { get; set; }
        public DateTime? TargetDate { get; set; }
        public string AcceptanceCriteria { get; set; }
        public Guid? CustomerId { get; set; }

        public Suggestion()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            Status = SuggestionStatus.New;
            SimilarSuggestions = Array.Empty<string>();
            CompletionPercentage = 0;
        }
    }

    public enum SuggestionStatus
    {
        New,
        Reviewed,
        Implemented,
        Rejected
    }

    public enum SuggestionPriority
    {
        Low,
        Medium,
        High
    }
}
