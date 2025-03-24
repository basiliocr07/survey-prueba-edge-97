
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

    public class QuestionSettingsViewModel
    {
        public int? Min { get; set; }
        public int? Max { get; set; }
    }

    public class DeliveryConfigViewModel
    {
        public string Type { get; set; } = "manual";
        public List<string> EmailAddresses { get; set; } = new List<string>();
        public ScheduleSettingsViewModel? Schedule { get; set; }
        public TriggerSettingsViewModel? Trigger { get; set; }
    }

    public class ScheduleSettingsViewModel
    {
        public string Frequency { get; set; } = "monthly";
        public int DayOfMonth { get; set; } = 1;
        public string Time { get; set; } = "09:00";
    }

    public class TriggerSettingsViewModel
    {
        public string Type { get; set; } = "ticket-closed";
        public int DelayHours { get; set; } = 24;
        public bool SendAutomatically { get; set; } = false;
    }
}
