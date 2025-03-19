
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
        public string Status { get; set; }
    }

    public class CreateRequirementDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
    }

    public class UpdateRequirementDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
    }
}
