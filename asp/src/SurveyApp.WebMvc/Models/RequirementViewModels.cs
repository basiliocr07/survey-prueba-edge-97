
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using SurveyApp.Application.DTOs;

namespace SurveyApp.WebMvc.Models
{
    public class RequirementsViewModel
    {
        public List<RequirementDto> Requirements { get; set; } = new List<RequirementDto>();
        public string[] Categories { get; set; } = { "Feature", "Bug", "UI/UX", "Performance", "Security", "Other" };
        public string[] ProjectAreas { get; set; } = { "Frontend", "Backend", "Database", "Infrastructure", "Mobile", "General" };
        public string ActiveTab { get; set; } = "view";

        // Statistics for dashboard
        public int TotalRequirements { get; set; }
        public int ProposedRequirements { get; set; }
        public int InProgressRequirements { get; set; }
        public int ImplementedRequirements { get; set; }
        public int RejectedRequirements { get; set; }
        
        // Status distribution for charts
        public Dictionary<string, int> StatusDistribution { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> PriorityDistribution { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> CategoryDistribution { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> ProjectAreaDistribution { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> MonthlyRequirements { get; set; } = new Dictionary<string, int>();
    }

    public class RequirementDetailViewModel
    {
        public RequirementDto Requirement { get; set; }
        public List<KnowledgeBaseItemDto> RelatedItems { get; set; } = new List<KnowledgeBaseItemDto>();
        public string[] StatusOptions { get; set; } = new[] { "Proposed", "In-Progress", "Testing", "Implemented", "Rejected" };
        public string[] PriorityOptions { get; set; } = new[] { "Low", "Medium", "High", "Critical" };
    }

    public class NewRequirementViewModel
    {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title must be between 5 and 200 characters", MinimumLength = 5)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(1000, ErrorMessage = "Description must be between 10 and 1000 characters", MinimumLength = 10)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Priority is required")]
        public string Priority { get; set; } = "Medium";

        [Required(ErrorMessage = "Project Area is required")]
        public string ProjectArea { get; set; } = "General";

        public string Status { get; set; } = "Proposed";

        public string CustomerName { get; set; }

        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string CustomerEmail { get; set; }

        public bool IsAnonymous { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public string Category { get; set; } = "Feature";

        public string AcceptanceCriteria { get; set; }

        public DateTime? TargetDate { get; set; }
    }

    public class UpdateRequirementViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, ErrorMessage = "Title must be between 5 and 200 characters", MinimumLength = 5)]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(1000, ErrorMessage = "Description must be between 10 and 1000 characters", MinimumLength = 10)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Priority is required")]
        public string Priority { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; }

        [Required(ErrorMessage = "Project Area is required")]
        public string ProjectArea { get; set; }

        public string Response { get; set; }

        [Range(0, 100, ErrorMessage = "Completion percentage must be between 0 and 100")]
        public int CompletionPercentage { get; set; }

        [Required(ErrorMessage = "Category is required")]
        public string Category { get; set; }

        public string AcceptanceCriteria { get; set; }

        public DateTime? TargetDate { get; set; }
    }
}
