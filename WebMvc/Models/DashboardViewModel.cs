
using System;
using System.Collections.Generic;

namespace SurveyApp.WebMvc.Models
{
    public class DashboardViewModel
    {
        public SurveyListItemViewModel LatestSurvey { get; set; }
        public SuggestionListItemViewModel LatestSuggestion { get; set; }
        public RequirementListItemViewModel LatestRequirement { get; set; }
    }

    public class SuggestionListItemViewModel
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public string CustomerName { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
    }

    public class RequirementListItemViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
    }
}
