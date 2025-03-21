
using System;
using System.Collections.Generic;

namespace SurveyApp.WebMvc.Models
{
    public class ThankYouViewModel
    {
        public string SurveyTitle { get; set; }
        public string ThankYouMessage { get; set; }
        public List<SurveyListItemViewModel> RelatedSurveys { get; set; } = new List<SurveyListItemViewModel>();
    }
    
    public class SurveyListItemViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public int ResponseCount { get; set; }
    }
}
