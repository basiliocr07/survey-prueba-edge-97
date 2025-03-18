using System;
using System.Collections.Generic;

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
        public List<string> SimilarSuggestions { get; set; } = new List<string>();
        
        // Nuevas propiedades
        public DateTime? TargetDate { get; set; }
        public string AcceptanceCriteria { get; set; }
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
