
using System;

namespace SurveyApp.WebMvc.Models
{
    public class SurveyListItemViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ResponseCount { get; set; }
        public int CompletionRate { get; set; }
        public string Status { get; set; } = "Active";
    }
}
