
using System;

namespace SurveyApp.Application.DTOs
{
    public class UpdateRequirementDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public string ProjectArea { get; set; }
        public string Category { get; set; }
        public string Response { get; set; }
        public int? CompletionPercentage { get; set; }
        public string AcceptanceCriteria { get; set; }
        public DateTime? TargetDate { get; set; }
    }
}
