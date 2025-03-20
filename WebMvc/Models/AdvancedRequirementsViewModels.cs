
using System;
using System.Collections.Generic;
using SurveyApp.Application.DTOs;

namespace SurveyApp.WebMvc.Models
{
    public class AdvancedRequirementsViewModel
    {
        public List<RequirementDto> Requirements { get; set; } = new List<RequirementDto>();
        public string[] Categories { get; set; }
        public string[] ProjectAreas { get; set; }
        public int TotalCount { get; set; }
        public int ProposedCount { get; set; }
        public int InProgressCount { get; set; }
        public int ImplementedCount { get; set; }
        public int RejectedCount { get; set; }
        public Dictionary<string, int> CategoryDistribution { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> ProjectAreaDistribution { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> MonthlyRequirements { get; set; } = new Dictionary<string, int>();
    }

    public class RequirementDetailViewModel
    {
        public RequirementDto Requirement { get; set; }
        public string[] StatusOptions { get; set; } = new[] { "Proposed", "In-Progress", "Testing", "Implemented", "Rejected" };
        public string[] PriorityOptions { get; set; } = new[] { "Low", "Medium", "High", "Critical" };
    }

    public class NewRequirementViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; } = "Medium";
        public string ProjectArea { get; set; }
        public string Category { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public bool IsAnonymous { get; set; }
    }
}
