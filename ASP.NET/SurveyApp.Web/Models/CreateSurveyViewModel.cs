
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SurveyApp.Web.Models
{
    public class CreateSurveyViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot be longer than 200 characters")]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot be longer than 1000 characters")]
        public string Description { get; set; } = string.Empty;

        public string Status { get; set; } = "draft";

        public List<SurveyQuestionViewModel> Questions { get; set; } = new List<SurveyQuestionViewModel>();

        public DeliveryConfigViewModel? DeliveryConfig { get; set; }
    }

    public class SurveyQuestionViewModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        
        [Required(ErrorMessage = "Question title is required")]
        public string Title { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Question type is required")]
        public string Type { get; set; } = string.Empty;
        
        public bool Required { get; set; } = true;
        
        public List<string> Options { get; set; } = new List<string>();
        
        public QuestionSettingsViewModel? Settings { get; set; }
    }
}
