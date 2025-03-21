using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SurveyApp.WebMvc.Models
{
    public class SurveyViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot be longer than 200 characters")]
        public string Title { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot be longer than 1000 characters")]
        public string Description { get; set; }

        public List<QuestionViewModel> Questions { get; set; } = new List<QuestionViewModel>();

        public DateTime CreatedAt { get; set; }
        
        public int Responses { get; set; }
        
        public int CompletionRate { get; set; }

        public DeliveryConfigViewModel DeliveryConfig { get; set; } = new DeliveryConfigViewModel();
        
        public string Status { get; set; } = "Active";
        
        public string Category { get; set; }
        
        // Propiedades adicionales para gestionar la autenticación y roles
        public bool IsAuthenticated { get; set; }
        public string UserRole { get; set; }
        public string Username { get; set; }
    }

    public class SurveyCreateViewModel
    {
        public Guid Id { get; set; }
        
        [Required(ErrorMessage = "El título es obligatorio")]
        [StringLength(200, ErrorMessage = "El título no puede tener más de 200 caracteres")]
        public string Title { get; set; }

        [StringLength(1000, ErrorMessage = "La descripción no puede tener más de 1000 caracteres")]
        public string Description { get; set; }

        public List<QuestionViewModel> Questions { get; set; } = new List<QuestionViewModel>();

        public DeliveryConfigViewModel DeliveryConfig { get; set; } = new DeliveryConfigViewModel();
        
        public string Category { get; set; }
        
        // Email specific properties
        public bool EnableEmailDelivery { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        
        public DateTime? ExpiryDate { get; set; }
        public bool AllowAnonymousResponses { get; set; } = true;
        public string ThankYouMessage { get; set; } = "¡Gracias por completar nuestra encuesta!";
    }

    public class DeliveryConfigViewModel
    {
        public string Type { get; set; } = "Manual";
        
        public List<string> EmailAddresses { get; set; } = new List<string>();
        
        public ScheduleViewModel Schedule { get; set; } = new ScheduleViewModel();
        
        public TriggerViewModel Trigger { get; set; } = new TriggerViewModel();
    }

    public class ScheduleViewModel
    {
        public string Frequency { get; set; } = "monthly";
        
        public int DayOfMonth { get; set; } = 1;
        
        public int? DayOfWeek { get; set; }
        
        public string Time { get; set; } = "09:00";
        
        public DateTime? StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }
    }

    public class TriggerViewModel
    {
        public string Type { get; set; } = "ticket-closed";
        
        public int DelayHours { get; set; } = 24;
        
        public bool SendAutomatically { get; set; } = false;
        
        public string EventName { get; set; }
    }

    public class QuestionViewModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required(ErrorMessage = "El título de la pregunta es obligatorio")]
        public string Title { get; set; }
        
        public string Description { get; set; }
        
        [Required(ErrorMessage = "El tipo de pregunta es obligatorio")]
        public string Type { get; set; } = "single-choice";
        
        public bool Required { get; set; } = true;
        
        public List<string> Options { get; set; } = new List<string> { "Opción 1", "Opción 2", "Opción 3" };
        
        public QuestionSettingsViewModel Settings { get; set; }
    }
    
    public class QuestionSettingsViewModel
    {
        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }
        
        public string LowLabel { get; set; }
        public string MiddleLabel { get; set; }
        public string HighLabel { get; set; }
    }
}
