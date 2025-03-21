
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
    }
}
