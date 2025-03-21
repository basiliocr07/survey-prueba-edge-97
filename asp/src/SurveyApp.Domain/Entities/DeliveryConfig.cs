
using System;
using System.Collections.Generic;

namespace SurveyApp.Domain.Entities
{
    public class DeliveryConfig
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = "Manual";
        public List<string> EmailAddresses { get; set; } = new List<string>();
        public Schedule? Schedule { get; set; }
        public Trigger? Trigger { get; set; }
        
        public DeliveryConfig()
        {
            Id = Guid.NewGuid();
            EmailAddresses = new List<string>();
        }
    }

    public class Schedule
    {
        public Guid Id { get; set; }
        public string Frequency { get; set; } = "monthly";
        public int DayOfMonth { get; set; } = 1;
        public int? DayOfWeek { get; set; }
        public string Time { get; set; } = "09:00";
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        
        public Schedule()
        {
            Id = Guid.NewGuid();
        }
    }

    public class Trigger
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = "ticket-closed";
        public int DelayHours { get; set; } = 24;
        public bool SendAutomatically { get; set; } = false;
        public string EventName { get; set; } = string.Empty;
        
        public Trigger()
        {
            Id = Guid.NewGuid();
        }
    }
}
