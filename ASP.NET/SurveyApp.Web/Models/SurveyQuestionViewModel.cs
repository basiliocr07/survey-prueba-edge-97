
using System.Collections.Generic;

namespace SurveyApp.Web.Models
{
    public class SurveyQuestionViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public bool Required { get; set; }
        public List<string> Options { get; set; } = new List<string>();
        public QuestionSettingsViewModel Settings { get; set; }

        public static SurveyQuestionViewModel FromQuestionViewModel(QuestionViewModel question)
        {
            return new SurveyQuestionViewModel
            {
                Id = question.Id,
                Title = question.Text, // Map Text to Title
                Description = question.Description,
                Type = question.Type,
                Required = question.Required,
                Options = question.Options ?? new List<string>(),
                Settings = question.Settings
            };
        }
        
        public QuestionViewModel ToQuestionViewModel()
        {
            var questionViewModel = new QuestionViewModel
            {
                Id = Id,
                Text = Title, // Map Title back to Text
                Description = Description,
                Type = Type,
                Required = Required,
                Options = Options ?? new List<string>(),
                Settings = Settings
            };
            
            questionViewModel.EnsureConsistency();
            return questionViewModel;
        }

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
