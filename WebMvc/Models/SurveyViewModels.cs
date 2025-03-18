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
        
        public bool IsFeatured { get; set; }
        
        public DateTime? LastUpdated { get; set; }
        
        public DateTime? LastResponseDate { get; set; }
        
        public string CreatedBy { get; set; }
    }

    public class SurveyListViewModel
    {
        public List<SurveyListItemViewModel> Surveys { get; set; } = new List<SurveyListItemViewModel>();
        public string SearchTerm { get; set; }
        public string StatusFilter { get; set; }
        public string CategoryFilter { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public List<string> Categories { get; set; } = new List<string>();
        public List<string> Statuses { get; set; } = new List<string>() { "Active", "Draft", "Archived", "Completed" };
    }

    public class QuestionViewModel
    {
        public Guid Id { get; set; }
        
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }
        
        public string Description { get; set; }
        
        [Required(ErrorMessage = "Question type is required")]
        public string Type { get; set; }
        
        public bool Required { get; set; }
        
        public List<string> Options { get; set; } = new List<string>();
        
        public int Order { get; set; }
        
        public ValidationRulesViewModel ValidationRules { get; set; } = new ValidationRulesViewModel();
    }

    public class ValidationRulesViewModel
    {
        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }
        public string Pattern { get; set; }
        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }
    }

    public class SurveyCreateViewModel
    {
        public Guid Id { get; set; }
        
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title cannot be longer than 200 characters")]
        public string Title { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot be longer than 1000 characters")]
        public string Description { get; set; }

        public List<QuestionViewModel> Questions { get; set; } = new List<QuestionViewModel>();

        public DeliveryConfigViewModel DeliveryConfig { get; set; } = new DeliveryConfigViewModel();
        
        public string Category { get; set; }
        
        public bool IsFeatured { get; set; }
        
        public bool EnableEmailDelivery { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string EmailSubject { get; set; }
        public string EmailBody { get; set; }
        
        public DateTime? ExpiryDate { get; set; }
        public bool AllowAnonymousResponses { get; set; } = true;
        public bool LimitOneResponsePerUser { get; set; }
        public string ThankYouMessage { get; set; } = "Thank you for completing our survey!";
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
}
