
using System;

namespace SurveyApp.WebMvc.Models
{
    public class SuggestionDetailViewModel
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public string Category { get; set; }
        public string Response { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int CompletionPercentage { get; set; }
        public DateTime? ResponseDate { get; set; }
        public bool IsAnonymous { get; set; }
    }
}
