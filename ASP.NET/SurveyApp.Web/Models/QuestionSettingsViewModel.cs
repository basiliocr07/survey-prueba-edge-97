
using System;
using System.Collections.Generic;

namespace SurveyApp.Web.Models
{
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
