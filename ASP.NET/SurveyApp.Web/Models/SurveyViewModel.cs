
using System;
using System.Collections.Generic;

namespace SurveyApp.Web.Models
{
    public class SurveyViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ResponseCount { get; set; }  // Changed from Responses to match domain model
        public int CompletionRate { get; set; }
        public string Status { get; set; }
    }
}
