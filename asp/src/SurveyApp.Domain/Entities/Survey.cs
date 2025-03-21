
using System;
using System.Collections.Generic;

namespace SurveyApp.Domain.Entities
{
    public class Survey
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public bool IsPublished { get; private set; }
        public string Category { get; set; }
        public List<Question> Questions { get; private set; } = new List<Question>();
        public SurveyDeliveryConfig DeliveryConfig { get; private set; }

        // For EF Core
        private Survey() { }

        public Survey(string title, string description, string category = "General")
        {
            Id = Guid.NewGuid();
            Title = title;
            Description = description;
            Category = category;
            CreatedAt = DateTime.UtcNow;
            IsPublished = false;
            DeliveryConfig = new SurveyDeliveryConfig();
        }

        public void AddQuestion(Question question)
        {
            Questions.Add(question);
        }

        public void PublishSurvey()
        {
            IsPublished = true;
        }

        public void UnpublishSurvey()
        {
            IsPublished = false;
        }

        public void UpdateTitle(string title)
        {
            Title = title;
        }

        public void UpdateDescription(string description)
        {
            Description = description;
        }

        public void UpdateCategory(string category)
        {
            Category = category;
        }

        public void ConfigureDelivery(DeliveryType type, string[] emailAddresses = null, DeliverySchedule schedule = null, DeliveryTrigger trigger = null)
        {
            DeliveryConfig.UpdateDeliveryConfig(type, emailAddresses, schedule, trigger);
        }
    }

    public class SurveyDeliveryConfig
    {
        public DeliveryType Type { get; private set; }
        public List<string> EmailAddresses { get; private set; } = new List<string>();
        public DeliverySchedule Schedule { get; private set; }
        public DeliveryTrigger Trigger { get; private set; }

        public SurveyDeliveryConfig()
        {
            Type = DeliveryType.Manual;
        }

        public void UpdateDeliveryConfig(DeliveryType type, string[] emailAddresses = null, DeliverySchedule schedule = null, DeliveryTrigger trigger = null)
        {
            Type = type;
            
            if (emailAddresses != null)
            {
                EmailAddresses.Clear();
                EmailAddresses.AddRange(emailAddresses);
            }

            Schedule = schedule;
            Trigger = trigger;
        }
    }

    public enum DeliveryType
    {
        Manual,
        Automated,
        Scheduled,
        Triggered
    }

    public class DeliverySchedule
    {
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string RecurrencePattern { get; set; }
    }

    public class DeliveryTrigger
    {
        public string EventType { get; set; }
        public Dictionary<string, string> EventParameters { get; set; } = new Dictionary<string, string>();
    }
}
