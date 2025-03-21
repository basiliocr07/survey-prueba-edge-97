
using System;

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
}
