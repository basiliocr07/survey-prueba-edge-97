
using System.Collections.Generic;

namespace SurveyApp.Web.Models
{
    public class SurveyQuestionViewModel
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Text { get; set; }
        
        // Alias for Text to maintain compatibility
        public string Title { 
            get { return Text; } 
            set { Text = value; } 
        }
        
        public string Description { get; set; }
        public List<string> Options { get; set; } = new List<string>();
        public bool Required { get; set; }
        public QuestionSettingsViewModel Settings { get; set; }
        
        // Conversion methods to ensure compatibility
        public static SurveyQuestionViewModel FromQuestionViewModel(QuestionViewModel model)
        {
            if (model == null) return null;
            
            return new SurveyQuestionViewModel
            {
                Id = model.Id,
                Type = model.Type,
                Text = model.Text ?? model.Title, // Handle both naming conventions
                Description = model.Description,
                Options = model.Options ?? new List<string>(),
                Required = model.Required,
                Settings = model.Settings
            };
        }
        
        public QuestionViewModel ToQuestionViewModel()
        {
            return new QuestionViewModel
            {
                Id = this.Id,
                Type = this.Type,
                Text = this.Text,
                Title = this.Text, // Ensure both properties are set
                Description = this.Description,
                Options = this.Options ?? new List<string>(),
                Required = this.Required,
                Settings = this.Settings
            };
        }
        
        // Helper method to ensure all properties are properly set
        public void EnsureConsistency()
        {
            // Make sure Text and Title are in sync
            if (string.IsNullOrEmpty(Text) && !string.IsNullOrEmpty(Title))
            {
                Text = Title;
            }
            else if (string.IsNullOrEmpty(Title) && !string.IsNullOrEmpty(Text))
            {
                Title = Text;
            }
            
            // Initialize collections if they're null
            if (Options == null)
            {
                Options = new List<string>();
            }
        }
    }
}
