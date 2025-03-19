
using System;
using System.Collections.Generic;
using SurveyApp.Application.DTOs;

namespace SurveyApp.WebMvc.Models
{
    public class DashboardViewModel
    {
        public SurveyListItemViewModel LatestSurvey { get; set; }
        public SuggestionListItemViewModel LatestSuggestion { get; set; }
        public RequirementListItemViewModel LatestRequirement { get; set; }
        
        // Listas de elementos recientes
        public List<SurveyResponseItemViewModel> RecentSurveyResponses { get; set; } = new List<SurveyResponseItemViewModel>();
        public List<SuggestionListItemViewModel> RecentSuggestions { get; set; } = new List<SuggestionListItemViewModel>();
        public List<RequirementListItemViewModel> RecentRequirements { get; set; } = new List<RequirementListItemViewModel>();
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
        public string ProjectArea { get; set; }
    }
    
    public class SurveyResponseItemViewModel
    {
        public Guid Id { get; set; }
        public Guid SurveyId { get; set; }
        public string SurveyTitle { get; set; }
        public string RespondentName { get; set; }
        public DateTime SubmittedAt { get; set; }
    }
}
