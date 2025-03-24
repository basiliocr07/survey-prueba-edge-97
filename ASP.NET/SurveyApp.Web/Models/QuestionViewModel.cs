
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SurveyApp.Web.Models
{
    public class QuestionViewModel
    {
        public string Id { get; set; }
        
        [Required(ErrorMessage = "Question text is required")]
        public string Text { get; set; }
        
        [Required(ErrorMessage = "Question type is required")]
        public string Type { get; set; }
        
        public bool Required { get; set; }
        
        public string Description { get; set; }
        
        public List<string> Options { get; set; } = new List<string>();
        
        public QuestionSettingsViewModel Settings { get; set; }
        
        // Add these properties for compatibility with SurveyQuestionViewModel
        public string Title { 
            get { return Text; } 
            set { Text = value; } 
        }
        
        public string Question { 
            get { return Text; } 
            set { Text = value; } 
        }
    }
}
