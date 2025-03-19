
using System;
using System.Collections.Generic;

namespace SurveyApp.WebMvc.Models
{
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
        
        // User authentication and role properties
        public bool IsAuthenticated { get; set; }
        public string UserRole { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
    }
}
