
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SurveyApp.WebMvc.Models
{
    public class CustomerGrowthViewModel
    {
        public List<CustomerViewModel> Customers { get; set; } = new List<CustomerViewModel>();
        public List<ServiceUsageViewModel> ServiceUsageData { get; set; } = new List<ServiceUsageViewModel>();
        public List<string> AvailableServices { get; set; } = new List<string>();
    }

    public class CustomerViewModel
    {
        public Guid Id { get; set; }
        public string BrandName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string ContactName { get; set; }
        public List<string> AcquiredServices { get; set; } = new List<string>();
        public DateTime CreatedAt { get; set; }
        public List<GrowthMetricViewModel> GrowthMetrics { get; set; } = new List<GrowthMetricViewModel>();
    }

    public class GrowthMetricViewModel
    {
        public string Period { get; set; }
        public decimal Revenue { get; set; }
        public int UserCount { get; set; }
    }

    public class ServiceUsageViewModel
    {
        public string Name { get; set; }
        public int Count { get; set; }
    }

    public class CreateCustomerViewModel
    {
        [Required(ErrorMessage = "Brand name is required")]
        [Display(Name = "Brand Name")]
        public string BrandName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Display(Name = "Contact Email")]
        public string ContactEmail { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Display(Name = "Contact Phone")]
        public string ContactPhone { get; set; }

        [Required(ErrorMessage = "Contact name is required")]
        [Display(Name = "Contact Name")]
        public string ContactName { get; set; }

        [Display(Name = "Services")]
        public List<string> SelectedServices { get; set; } = new List<string>();

        public List<string> AvailableServices { get; set; } = new List<string>();
    }

    public class EditCustomerViewModel : CreateCustomerViewModel
    {
        public Guid Id { get; set; }
    }
}
