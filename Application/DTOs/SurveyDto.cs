
using System;
using System.Collections.Generic;

namespace SurveyApp.Application.DTOs
{
    public class SurveyDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<QuestionDto> Questions { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Responses { get; set; }
        public int CompletionRate { get; set; }
        public DeliveryConfigDto DeliveryConfig { get; set; }
    }

    public class CreateSurveyDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<CreateQuestionDto> Questions { get; set; }
        public DeliveryConfigDto DeliveryConfig { get; set; }
    }

    public class QuestionDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }
        public List<string> Options { get; set; }
    }

    public class CreateQuestionDto
    {
        public string Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool Required { get; set; }
        public List<string> Options { get; set; }
    }

    public class DeliveryConfigDto
    {
        public string Type { get; set; }
        public List<string> EmailAddresses { get; set; }
        public ScheduleDto Schedule { get; set; }
        public TriggerDto Trigger { get; set; }
    }

    public class ScheduleDto
    {
        public string Frequency { get; set; }
        public int? DayOfMonth { get; set; }
        public int? DayOfWeek { get; set; }
        public string Time { get; set; }
        public DateTime? StartDate { get; set; }
    }

    public class TriggerDto
    {
        public string Type { get; set; }
        public int DelayHours { get; set; }
        public bool SendAutomatically { get; set; }
    }
}
