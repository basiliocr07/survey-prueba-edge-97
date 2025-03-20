
using System;
using System.Collections.Generic;
using SurveyApp.Application.DTOs;

namespace SurveyApp.WebMvc.Models
{
    public class AdvancedRequirementsViewModel
    {
        public IEnumerable<RequirementDto> Requirements { get; set; }
        public string[] Categories { get; set; }
        public string[] ProjectAreas { get; set; }
        public int TotalCount { get; set; }
        public int ProposedCount { get; set; }
        public int InProgressCount { get; set; }
        public int ImplementedCount { get; set; }
        public int RejectedCount { get; set; }
        public int CriticalCount { get; set; }
        public int HighCount { get; set; }
        public int MediumCount { get; set; }
        public int LowCount { get; set; }
        public Dictionary<string, int> CategoryDistribution { get; set; }
        public Dictionary<string, int> ProjectAreaDistribution { get; set; }
        public Dictionary<string, int> MonthlyRequirements { get; set; }
    }

    public class RequirementDetailViewModel
    {
        public RequirementDto Requirement { get; set; }
        public List<RequirementDto> RelatedRequirements { get; set; }
    }
}
