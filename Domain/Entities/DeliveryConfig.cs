
using System;
using System.Collections.Generic;

namespace SurveyApp.Domain.Entities
{
    public class DeliveryConfig
    {
        public string Type { get; set; }
        public List<string> EmailAddresses { get; set; }
        public Schedule Schedule { get; set; }
        public Trigger Trigger { get; set; }

        public DeliveryConfig()
        {
            Type = "Manual";
            EmailAddresses = new List<string>();
            Schedule = new Schedule();
            Trigger = new Trigger();
        }

        public void SetType(string type)
        {
            Type = type;
        }

        public void AddEmailAddress(string email)
        {
            if (!EmailAddresses.Contains(email))
            {
                EmailAddresses.Add(email);
            }
        }

        public void RemoveEmailAddress(string email)
        {
            EmailAddresses.Remove(email);
        }

        public void SetSchedule(Schedule schedule)
        {
            Schedule = schedule ?? new Schedule();
        }

        public void SetTrigger(Trigger trigger)
        {
            Trigger = trigger ?? new Trigger();
        }
    }

    public class Schedule
    {
        public string Frequency { get; set; } = "monthly";
        public int DayOfMonth { get; set; } = 1;
        public int? DayOfWeek { get; set; }
        public string Time { get; set; } = "09:00";
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class Trigger
    {
        public string Type { get; set; } = "ticket-closed";
        public int DelayHours { get; set; } = 24;
        public bool SendAutomatically { get; set; } = false;
        public string EventName { get; set; }
    }
}
