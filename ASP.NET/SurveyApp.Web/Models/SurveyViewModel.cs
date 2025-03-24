
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
        public int ResponseCount { get; set; }
        
        // ResponseCount and Responses are the same thing to avoid ambiguity
        public int Responses { 
            get { return ResponseCount; } 
            set { ResponseCount = value; } 
        }
        
        public int CompletionRate { get; set; }
        public string Status { get; set; }
        
        // Add a collection of questions that might be needed in views
        public List<SurveyQuestionViewModel> Questions { get; set; } = new List<SurveyQuestionViewModel>();
    }
}
