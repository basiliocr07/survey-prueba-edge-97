
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SurveyApp.Domain.Repositories;

namespace SurveyApp.Web.Models
{
    /// <summary>
    /// ViewModel para la configuración de entrega de encuestas
    /// </summary>
    public class DeliveryConfigViewModel
    {
        /// <summary>
        /// Tipo de entrega: manual, scheduled, triggered
        /// </summary>
        public string Type { get; set; } = "manual";
        
        /// <summary>
        /// Lista de direcciones de correo electrónico para enviar la encuesta
        /// </summary>
        public List<string> EmailAddresses { get; set; } = new List<string>();
        
        /// <summary>
        /// Configuración para envíos programados
        /// </summary>
        public ScheduleSettingsViewModel Schedule { get; set; } = new ScheduleSettingsViewModel();
        
        /// <summary>
        /// Configuración para envíos desencadenados por eventos
        /// </summary>
        public TriggerSettingsViewModel Trigger { get; set; } = new TriggerSettingsViewModel();
        
        /// <summary>
        /// Indica si se deben incluir todos los clientes en el envío
        /// </summary>
        public bool IncludeAllCustomers { get; set; }
        
        /// <summary>
        /// Filtro de tipo de cliente para incluir solo ciertos tipos
        /// </summary>
        public string CustomerTypeFilter { get; set; }
        
        /// <summary>
        /// Carga los correos electrónicos de los clientes según el filtro configurado
        /// </summary>
        /// <param name="customerRepository">Repositorio de clientes para obtener los emails</param>
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
        
        /// <summary>
        /// Convierte este ViewModel a un modelo DeliveryConfiguration del dominio
        /// </summary>
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
        
        /// <summary>
        /// Crea un ViewModel desde un modelo DeliveryConfiguration del dominio
        /// </summary>
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

    /// <summary>
    /// Configuración para envíos programados
    /// </summary>
    public class ScheduleSettingsViewModel
    {
        public string Frequency { get; set; } = "monthly";
        public int? DayOfMonth { get; set; } = 1;
        public int? DayOfWeek { get; set; }
        public string Time { get; set; } = "09:00";
        public DateTime? StartDate { get; set; }
    }

    /// <summary>
    /// Configuración para envíos desencadenados por eventos
    /// </summary>
    public class TriggerSettingsViewModel
    {
        public string Type { get; set; } = "ticket-closed";
        public int DelayHours { get; set; } = 24;
        public bool SendAutomatically { get; set; }
    }
}
