
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
        
        // Title property for compatibility with React version
        public string Title { 
            get { return Text; } 
            set { Text = value; } 
        }
        
        // Método para convertir a modelo de dominio
        public SurveyApp.Domain.Models.Question ToDomainModel()
        {
            int questionId = 0;
            
            // Try to parse the ID if it's not null/empty and doesn't start with "new-"
            if (!string.IsNullOrEmpty(Id) && !Id.StartsWith("new-"))
            {
                // Try to parse as integer (Database ID)
                if (!int.TryParse(Id, out questionId))
                {
                    // If not an integer, it's likely a GUID (temporary ID)
                    questionId = 0;
                }
            }
            
            return new SurveyApp.Domain.Models.Question
            {
                Id = questionId,
                Text = Text ?? string.Empty,
                Type = Type ?? "text",
                Required = Required,
                Description = Description ?? string.Empty,
                Options = Options ?? new List<string>(),
                Settings = Settings != null ? new SurveyApp.Domain.Models.QuestionSettings
                {
                    Min = Settings.Min,
                    Max = Settings.Max
                } : null
            };
        }
        
        // Método para crear desde modelo de dominio
        public static QuestionViewModel FromDomainModel(SurveyApp.Domain.Models.Question question)
        {
            return new QuestionViewModel
            {
                Id = question.Id.ToString(),
                Text = question.Text,
                Type = question.Type,
                Required = question.Required,
                Description = question.Description,
                Options = question.Options ?? new List<string>(),
                Settings = question.Settings != null ? new QuestionSettingsViewModel
                {
                    Min = question.Settings.Min,
                    Max = question.Settings.Max
                } : null
            };
        }
    }
}
