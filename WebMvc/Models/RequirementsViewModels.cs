
using System.Collections.Generic;
using SurveyApp.Application.DTOs;

namespace SurveyApp.WebMvc.Models
{
    public class RequirementsViewModel
    {
        public List<SuggestionDto> Requirements { get; set; }
        public List<KnowledgeBaseItemDto> KnowledgeBase { get; set; }
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
        public List<SuggestionDto> Requirements { get; set; }
        public string[] Categories { get; set; }
        public string CategoryFilter { get; set; }
        public string SearchTerm { get; set; }
    }

    public class KnowledgeBaseViewModel
    {
        public List<KnowledgeBaseItemDto> KnowledgeBaseItems { get; set; }
        public string SearchTerm { get; set; }
    }
}
