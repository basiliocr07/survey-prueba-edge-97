
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
        public DateTime? UpdatedAt { get; set; }
        public string Status { get; set; } = "active"; // Can be "active", "draft", or "archived"
        public int ResponseCount { get; set; }
        public int CompletionRate { get; set; }
        public List<Question> Questions { get; set; } = new List<Question>();
        public DeliveryConfiguration? DeliveryConfig { get; set; }
        
        // Navigation property
        public List<SurveyResponse>? Responses { get; set; }
    }

    public class Question
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public bool Required { get; set; } = true;
        public string Description { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new List<string>();
        public QuestionSettings? Settings { get; set; }
        
        // Propiedad Title para compatibilidad con la versión React
        public string Title
        {
            get { return Text; }
            set { Text = value; }
        }
    }

    public class QuestionSettings
    {
        public int? Min { get; set; }
        public int? Max { get; set; }
    }

    public class DeliveryConfiguration
    {
        public string Type { get; set; } = "manual"; // Can be "manual", "scheduled", or "trigger"
        public List<string> EmailAddresses { get; set; } = new List<string>();
        public ScheduleSettings? Schedule { get; set; }
        public TriggerSettings? Trigger { get; set; }
    }

    public class ScheduleSettings
    {
        public string Frequency { get; set; } = "monthly"; // Can be "daily", "weekly", "monthly"
        public int DayOfMonth { get; set; } = 1;
        public int? DayOfWeek { get; set; }
        public string Time { get; set; } = "09:00";
        public DateTime? StartDate { get; set; }
    }

    public class TriggerSettings
    {
        public string Type { get; set; } = "ticket-closed"; 
        public int DelayHours { get; set; } = 24;
        public bool SendAutomatically { get; set; } = false;
    }
}
