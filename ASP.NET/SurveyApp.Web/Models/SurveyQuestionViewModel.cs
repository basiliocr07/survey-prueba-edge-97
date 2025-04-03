
using System.Collections.Generic;

namespace SurveyApp.Web.Models
{
    /// <summary>
    /// Modelo de vista específico para mostrar preguntas en una encuesta
    /// </summary>
    public class SurveyQuestionViewModel
    {
        public string Id { get; set; }
        
        /// <summary>
        /// Título de la pregunta (equivalente a Text en QuestionViewModel)
        /// </summary>
        public string Title { get; set; }
        
        public string Description { get; set; }
        public string Type { get; set; }
        public bool Required { get; set; }
        public List<string> Options { get; set; } = new List<string>();
        public QuestionSettingsViewModel Settings { get; set; }

        /// <summary>
        /// Convierte un QuestionViewModel a SurveyQuestionViewModel
        /// </summary>
        public static SurveyQuestionViewModel FromQuestionViewModel(QuestionViewModel question)
        {
            return new SurveyQuestionViewModel
            {
                Id = question.Id,
                Title = question.Text, // Map Text to Title explicitly
                Description = question.Description,
                Type = question.Type,
                Required = question.Required,
                Options = question.Options ?? new List<string>(),
                Settings = question.Settings
            };
        }
        
        /// <summary>
        /// Convierte este SurveyQuestionViewModel a QuestionViewModel
        /// </summary>
        public QuestionViewModel ToQuestionViewModel()
        {
            var questionViewModel = new QuestionViewModel
            {
                Id = Id,
                Text = Title, // Map Title back to Text explicitly
                Description = Description,
                Type = Type,
                Required = Required,
                Options = Options ?? new List<string>(),
                Settings = Settings
            };
            
            questionViewModel.EnsureConsistency();
            return questionViewModel;
        }

        /// <summary>
        /// Asegura la consistencia del modelo según el tipo de pregunta
        /// </summary>
        public void EnsureConsistency()
        {
            // Add sample options for choice-based questions if none exist
            if ((Type == "multiple-choice" || Type == "single-choice" || Type == "dropdown" || Type == "ranking") && 
                (Options == null || Options.Count == 0))
            {
                Options = new List<string> { "Option 1", "Option 2" };
            }

            // Create default settings for special question types
            if (Type == "rating" && Settings == null)
            {
                Settings = new QuestionSettingsViewModel { Min = 1, Max = 5 };
            }
            else if (Type == "nps" && Settings == null)
            {
                Settings = new QuestionSettingsViewModel { Min = 0, Max = 10 };
            }
        }
    }
}
