
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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
        [Required(ErrorMessage = "Question title is required")]
        public string Title { get; set; }
        public string Type { get; set; }
        public bool Required { get; set; } = true;
        public string Description { get; set; }
        public List<string> Options { get; set; } = new List<string>();
        public QuestionSettingsViewModel Settings { get; set; } = new QuestionSettingsViewModel();
    }
}
