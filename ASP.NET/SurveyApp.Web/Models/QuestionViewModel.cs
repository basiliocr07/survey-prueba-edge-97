
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SurveyApp.Web.Models
{
    /// <summary>
    /// Modelo de vista para preguntas de encuesta
    /// </summary>
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
        
        /// <summary>
        /// Propiedad Title para compatibilidad con la versión React
        /// Internamente, siempre se debe usar Text para la pregunta
        /// </summary>
        public string Title { 
            get { return Text; } 
            set { Text = value; } 
        }
        
        /// <summary>
        /// Convierte el ViewModel a un modelo de dominio Question
        /// </summary>
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
        
        /// <summary>
        /// Crea un ViewModel desde un modelo de dominio Question
        /// </summary>
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
        
        /// <summary>
        /// Asegura la consistencia del modelo, inicializando valores por defecto según el tipo de pregunta
        /// </summary>
        public void EnsureConsistency()
        {
            // Ensure text is not null
            Text = Text ?? string.Empty;
            
            // Add sample options for choice-based questions if none exist
            if ((Type == "multiple-choice" || Type == "single-choice" || Type == "dropdown" || Type == "ranking") && 
                (Options == null || Options.Count == 0))
            {
                Options = new List<string> { "Option 1", "Option 2" };
            }

            // Create default settings for special question types
            if (Settings == null)
            {
                Settings = new QuestionSettingsViewModel();
                
                if (Type == "rating")
                {
                    Settings.Min = 1;
                    Settings.Max = 5;
                }
                else if (Type == "nps")
                {
                    Settings.Min = 0;
                    Settings.Max = 10;
                }
            }
        }
    }
}
