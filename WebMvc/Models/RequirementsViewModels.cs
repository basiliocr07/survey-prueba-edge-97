
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
    }

    public class RequirementsListViewModel
    {
        public List<RequirementDto> Requirements { get; set; } = new List<RequirementDto>();
        public string[] Categories { get; set; }
        public string StatusFilter { get; set; }
        public string PriorityFilter { get; set; }
        public string CategoryFilter { get; set; }
        public string SearchTerm { get; set; }
    }

    public class KnowledgeBaseViewModel
    {
        public List<KnowledgeBaseItemDto> KnowledgeBaseItems { get; set; } = new List<KnowledgeBaseItemDto>();
        public string SearchTerm { get; set; }
    }
}
