
using System;
using System.Collections.Generic;

namespace SurveyApp.Web.Models
{
    public class EmailSettingsViewModel
    {
        public List<SurveyListItemViewModel> Surveys { get; set; } = new List<SurveyListItemViewModel>();
        public DeliveryConfigViewModel DeliveryConfig { get; set; } = new DeliveryConfigViewModel();
        public int? SelectedSurveyId { get; set; }
    }

    public class SurveyListItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool HasCustomDeliveryConfig { get; set; }
    }
}
