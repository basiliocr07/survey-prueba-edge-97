
using System;
using System.Collections.Generic;

namespace SurveyApp.Web.Models
{
    public class DeliveryConfigViewModel
    {
        public string Type { get; set; } = "manual";
        public List<string> EmailAddresses { get; set; } = new List<string>();
        public ScheduleSettingsViewModel Schedule { get; set; } = new ScheduleSettingsViewModel();
        public TriggerSettingsViewModel Trigger { get; set; } = new TriggerSettingsViewModel();
        public bool IncludeAllCustomers { get; set; }
        public string CustomerTypeFilter { get; set; }
    }

    public class ScheduleSettingsViewModel
    {
        public string Frequency { get; set; } = "monthly";
        public int? DayOfMonth { get; set; } = 1;
        public int? DayOfWeek { get; set; }
        public string Time { get; set; } = "09:00";
        public DateTime? StartDate { get; set; }
    }

    public class TriggerSettingsViewModel
    {
        public string Type { get; set; } = "ticket-closed";
        public int DelayHours { get; set; } = 24;
        public bool SendAutomatically { get; set; }
    }
}
