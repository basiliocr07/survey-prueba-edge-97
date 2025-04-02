
using System;
using System.Collections.Generic;

namespace SurveyApp.Web.Models
{
    public class EmailSettingsViewModel
    {
        public List<SurveyListItemViewModel> Surveys { get; set; } = new List<SurveyListItemViewModel>();
        public DeliveryConfigViewModel DeliveryConfig { get; set; } = new DeliveryConfigViewModel();
        public int? SelectedSurveyId { get; set; }
        public List<CustomerViewModel> Customers { get; set; } = new List<CustomerViewModel>();
    }

    public class SurveyListItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool HasCustomDeliveryConfig { get; set; }
    }

    public class CustomerViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
