
using System;
using System.Collections.Generic;

namespace SurveyApp.Application.DTOs
{
    public class RequirementDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Status { get; set; }
        public string ProjectArea { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public bool IsAnonymous { get; set; }
        public string Response { get; set; }
        public DateTime? ResponseDate { get; set; }
        public int CompletionPercentage { get; set; }
        public string Category { get; set; }
        public string AcceptanceCriteria { get; set; }
        public DateTime? TargetDate { get; set; }
    }

    public class CreateRequirementDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public string ProjectArea { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public bool IsAnonymous { get; set; }
        public string Category { get; set; }
        public string AcceptanceCriteria { get; set; }
        public DateTime? TargetDate { get; set; }
    }

    public class UpdateRequirementDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public string ProjectArea { get; set; }
        public string Response { get; set; }
        public int? CompletionPercentage { get; set; }
        public string Category { get; set; }
        public string AcceptanceCriteria { get; set; }
        public DateTime? TargetDate { get; set; }
    }

    public class RequirementStatusUpdateDto
    {
        public string Status { get; set; }
        public string Response { get; set; }
        public int? CompletionPercentage { get; set; }
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
}
