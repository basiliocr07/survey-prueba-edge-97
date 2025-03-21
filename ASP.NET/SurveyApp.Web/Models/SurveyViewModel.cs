
using System;
using System.Collections.Generic;

namespace SurveyApp.Web.Models
{
    public class SurveyViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Responses { get; set; }
        public int CompletionRate { get; set; }
        public string Status { get; set; } = "active"; // Can be "active", "draft", or "archived"
    }

    public class QuestionViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public bool Required { get; set; } = true;
        public string Description { get; set; }
        public List<string> Options { get; set; } = new List<string>();
        public QuestionSettingsViewModel Settings { get; set; }
    }

    public class QuestionSettingsViewModel
    {
        public int? Min { get; set; }
        public int? Max { get; set; }
    }

    public class CreateSurveyViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<QuestionViewModel> Questions { get; set; } = new List<QuestionViewModel>();
        public string Status { get; set; } = "draft";
    }
}
