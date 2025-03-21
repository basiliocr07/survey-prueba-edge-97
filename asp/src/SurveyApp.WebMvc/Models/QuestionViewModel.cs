
using System;
using System.Collections.Generic;

namespace SurveyApp.WebMvc.Models
{
    public class QuestionViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool Required { get; set; }
        public List<string> Options { get; set; } = new List<string>();
        public QuestionSettings? Settings { get; set; }
    }

    public class QuestionSettings
    {
        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }
        public string? LowLabel { get; set; }
        public string? MiddleLabel { get; set; }
        public string? HighLabel { get; set; }
    }
}
