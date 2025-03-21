
using System;
using System.Collections.Generic;
using SurveyApp.Application.DTOs;

namespace SurveyApp.WebMvc.Models
{
    public class RequirementsViewModel
    {
        public List<RequirementDto> Requirements { get; set; } = new List<RequirementDto>();
        public List<KnowledgeBaseItemDto> KnowledgeBase { get; set; } = new List<KnowledgeBaseItemDto>();
        public string[] Categories { get; set; }
        public string[] ProjectAreas { get; set; }
        public string ActiveTab { get; set; } = "new";

        // Additional properties for requirement counts
        public int ProposedCount { get; set; }
        public int InProgressCount { get; set; }
        public int TestingCount { get; set; }
        public int CompletedCount { get; set; }
        public int TotalCount { get; set; }
        
        // Additional properties for requirement priorities
        public int CriticalCount { get; set; }
        public int HighCount { get; set; }
        public int MediumCount { get; set; }
        public int LowCount { get; set; }
        
        // Distribuciones para reportes
        public Dictionary<string, int> CategoryDistribution { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> ProjectAreaDistribution { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> MonthlyRequirements { get; set; } = new Dictionary<string, int>();
    }

    public class RequirementsListViewModel
    {
        public List<RequirementDto> Requirements { get; set; } = new List<RequirementDto>();
        public string[] Categories { get; set; }
        public string StatusFilter { get; set; }
        public string PriorityFilter { get; set; }
        public string CategoryFilter { get; set; }
        public string SearchTerm { get; set; }
        public string ProjectAreaFilter { get; set; }
    }

    public class RequirementDetailViewModel
    {
        public RequirementDto Requirement { get; set; }
        public List<KnowledgeBaseItemDto> RelatedItems { get; set; } = new List<KnowledgeBaseItemDto>();
        public string[] StatusOptions { get; set; } = new[] { "Proposed", "In-Progress", "Testing", "Implemented", "Rejected" };
        public string[] PriorityOptions { get; set; } = new[] { "Low", "Medium", "High", "Critical" };
    }

    public class RequirementReportsViewModel
    {
        public int TotalRequirements { get; set; }
        public int ProposedRequirements { get; set; }
        public int InProgressRequirements { get; set; }
        public int ImplementedRequirements { get; set; }
        public int RejectedRequirements { get; set; }
        public Dictionary<string, int> CategoryDistribution { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> PriorityDistribution { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> ProjectAreaDistribution { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> MonthlyRequirements { get; set; } = new Dictionary<string, int>();
    }

    public class NewRequirementViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; } = "Medium";
        public string ProjectArea { get; set; } = "General";
        public string Status { get; set; } = "Proposed";
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public bool IsAnonymous { get; set; }
        public string Category { get; set; } = "Feature";
        public string AcceptanceCriteria { get; set; }
        public DateTime? TargetDate { get; set; }
    }
}
