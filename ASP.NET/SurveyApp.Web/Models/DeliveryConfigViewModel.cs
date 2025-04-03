
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SurveyApp.Domain.Repositories;

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
        
        // Método para cargar emails de clientes según el filtro
        public async Task LoadCustomerEmails(ICustomerRepository customerRepository)
        {
            if (IncludeAllCustomers)
            {
                var emails = await customerRepository.GetCustomerEmailsAsync(CustomerTypeFilter);
                if (EmailAddresses == null)
                    EmailAddresses = new List<string>();
                
                // Agregar los emails que no estén ya en la lista
                foreach (var email in emails)
                {
                    if (!EmailAddresses.Contains(email))
                    {
                        EmailAddresses.Add(email);
                    }
                }
            }
        }
        
        // Método para mapear a DeliveryConfiguration del dominio
        public SurveyApp.Domain.Models.DeliveryConfiguration ToDeliveryConfiguration()
        {
            return new SurveyApp.Domain.Models.DeliveryConfiguration
            {
                Type = this.Type,
                EmailAddresses = this.EmailAddresses ?? new List<string>(),
                IncludeAllCustomers = this.IncludeAllCustomers,
                CustomerTypeFilter = this.CustomerTypeFilter,
                Schedule = this.Schedule != null ? new SurveyApp.Domain.Models.ScheduleSettings
                {
                    Frequency = this.Schedule.Frequency,
                    DayOfMonth = this.Schedule.DayOfMonth ?? 1,
                    DayOfWeek = this.Schedule.DayOfWeek,
                    Time = this.Schedule.Time ?? "09:00",
                    StartDate = this.Schedule.StartDate
                } : null,
                Trigger = this.Trigger != null ? new SurveyApp.Domain.Models.TriggerSettings
                {
                    Type = this.Trigger.Type,
                    DelayHours = this.Trigger.DelayHours,
                    SendAutomatically = this.Trigger.SendAutomatically
                } : null
            };
        }
        
        // Método para crear desde DeliveryConfiguration del dominio
        public static DeliveryConfigViewModel FromDeliveryConfiguration(SurveyApp.Domain.Models.DeliveryConfiguration config)
        {
            if (config == null)
            {
                return new DeliveryConfigViewModel();
            }
            
            return new DeliveryConfigViewModel
            {
                Type = config.Type,
                EmailAddresses = config.EmailAddresses,
                IncludeAllCustomers = config.IncludeAllCustomers,
                CustomerTypeFilter = config.CustomerTypeFilter,
                Schedule = config.Schedule != null ? new ScheduleSettingsViewModel
                {
                    Frequency = config.Schedule.Frequency,
                    DayOfMonth = config.Schedule.DayOfMonth,
                    DayOfWeek = config.Schedule.DayOfWeek,
                    Time = config.Schedule.Time,
                    StartDate = config.Schedule.StartDate
                } : null,
                Trigger = config.Trigger != null ? new TriggerSettingsViewModel
                {
                    Type = config.Trigger.Type,
                    DelayHours = config.Trigger.DelayHours,
                    SendAutomatically = config.Trigger.SendAutomatically
                } : null
            };
        }
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
