
using System;
using System.Collections.Generic;

namespace SurveyApp.Domain.Entities
{
    public class Question
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool Required { get; set; }
        public List<string> Options { get; set; } = new List<string>();
        public QuestionSettings? Settings { get; set; }
        
        public Question()
        {
            Id = Guid.NewGuid();
            Required = false;
            Options = new List<string>();
        }
    }

    public class QuestionSettings
    {
        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }
        public string LowLabel { get; set; } = string.Empty;
        public string MiddleLabel { get; set; } = string.Empty;
        public string HighLabel { get; set; } = string.Empty;
    }
}
