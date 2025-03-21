
using System;
using System.Collections.Generic;

namespace SurveyApp.Application.DTOs
{
    public class SurveyDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<QuestionDto> Questions { get; set; } = new List<QuestionDto>();
        public DateTime CreatedAt { get; set; }
        public int Responses { get; set; }
        public int CompletionRate { get; set; }
        public DeliveryConfigDto? DeliveryConfig { get; set; }
        public string Status { get; set; } = "Active";
        public string Category { get; set; } = "General";
        public DateTime? ExpiryDate { get; set; }
        public bool AllowAnonymousResponses { get; set; } = true;
        public bool LimitOneResponsePerUser { get; set; } = false;
        public string ThankYouMessage { get; set; } = "¡Gracias por completar nuestra encuesta!";
    }

    public class CreateSurveyDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<CreateQuestionDto> Questions { get; set; } = new List<CreateQuestionDto>();
        public DeliveryConfigDto? DeliveryConfig { get; set; }
        public string Category { get; set; } = "General";
        public DateTime? ExpiryDate { get; set; }
        public bool AllowAnonymousResponses { get; set; } = true;
        public bool LimitOneResponsePerUser { get; set; } = false;
        public string ThankYouMessage { get; set; } = "¡Gracias por completar nuestra encuesta!";
    }

    public class QuestionDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool Required { get; set; }
        public List<string> Options { get; set; } = new List<string>();
        public QuestionSettingsDto? Settings { get; set; }
    }
    
    public class QuestionSettingsDto
    {
        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }
        public string LowLabel { get; set; } = string.Empty;
        public string MiddleLabel { get; set; } = string.Empty;
        public string HighLabel { get; set; } = string.Empty;
    }

    public class CreateQuestionDto
    {
        public string Type { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool Required { get; set; }
        public List<string> Options { get; set; } = new List<string>();
        public QuestionSettingsDto? Settings { get; set; }
    }

    public class DeliveryConfigDto
    {
        public string Type { get; set; } = "Manual";
        public List<string> EmailAddresses { get; set; } = new List<string>();
        public ScheduleDto? Schedule { get; set; }
        public TriggerDto? Trigger { get; set; }
    }

    public class ScheduleDto
    {
        public string Frequency { get; set; } = "monthly";
        public int DayOfMonth { get; set; } = 1;
        public int? DayOfWeek { get; set; }
        public string Time { get; set; } = "09:00";
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class TriggerDto
    {
        public string Type { get; set; } = "ticket-closed";
        public int DelayHours { get; set; } = 24;
        public bool SendAutomatically { get; set; } = false;
        public string EventName { get; set; } = string.Empty;
    }
}
