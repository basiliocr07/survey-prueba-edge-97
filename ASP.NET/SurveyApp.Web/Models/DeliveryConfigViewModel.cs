
using System.Collections.Generic;

namespace SurveyApp.Web.Models
{
    public class DeliveryConfigViewModel
    {
        public string Type { get; set; } = "manual";
        public List<string> EmailAddresses { get; set; } = new List<string>();
        public ScheduleSettingsViewModel Schedule { get; set; }
        public TriggerSettingsViewModel Trigger { get; set; }
    }

    public class ScheduleSettingsViewModel
    {
        public string Frequency { get; set; } = "daily";
        public int? DayOfMonth { get; set; }
        public int? DayOfWeek { get; set; }
        public string Time { get; set; } = "09:00";
        public string StartDate { get; set; }
    }

    public class TriggerSettingsViewModel
    {
        public string Type { get; set; } = "ticket-closed";
        public int DelayHours { get; set; } = 24;
        public bool SendAutomatically { get; set; } = true;
    }
}
