
using System;

namespace SurveyApp.WebMvc.Models
{
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
        public string AcceptanceCriteria { get; set; }
        public DateTime? TargetDate { get; set; }
    }
}
