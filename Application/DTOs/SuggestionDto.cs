
using System;

namespace SurveyApp.Application.DTOs
{
    public class SuggestionDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public string CustomerName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
    }

    public class CreateSuggestionDto
    {
        public string Content { get; set; }
        public string CustomerName { get; set; }
    }

    public class UpdateSuggestionDto
    {
        public string Content { get; set; }
        public string CustomerName { get; set; }
        public string Status { get; set; }
    }
}
