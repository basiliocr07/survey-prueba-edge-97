
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
                Text = model.Text,
                Description = model.Description,
                Options = model.Options,
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
                Description = this.Description,
                Options = this.Options,
                Required = this.Required,
                Settings = this.Settings
            };
        }
    }
}
