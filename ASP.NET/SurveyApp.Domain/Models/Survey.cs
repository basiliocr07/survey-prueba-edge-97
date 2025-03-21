
using System;
using System.Collections.Generic;

namespace SurveyApp.Domain.Models
{
    public class Survey
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<Question> Questions { get; set; } = new List<Question>();
    }

    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new List<string>();
    }
}
