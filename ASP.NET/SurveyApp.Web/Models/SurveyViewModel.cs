
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

    public class QuestionSettingsViewModel
    {
        public int? Min { get; set; }
        public int? Max { get; set; }
    }

    public class CreateSurveyViewModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Survey title is required")]
        public string Title { get; set; }
        public string Description { get; set; }
        public List<QuestionViewModel> Questions { get; set; } = new List<QuestionViewModel>();
        public string Status { get; set; } = "draft";
        public DeliveryConfigViewModel DeliveryConfig { get; set; } = new DeliveryConfigViewModel();
    }

    public class DeliveryConfigViewModel
    {
        public string Type { get; set; } = "manual"; // Can be "manual", "scheduled", or "trigger"
        public List<string> EmailAddresses { get; set; } = new List<string>();
        public ScheduleSettingsViewModel Schedule { get; set; } = new ScheduleSettingsViewModel();
        public TriggerSettingsViewModel Trigger { get; set; } = new TriggerSettingsViewModel();
    }

    public class ScheduleSettingsViewModel
    {
        public string Frequency { get; set; } = "monthly"; // Can be "daily", "weekly", "monthly"
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
