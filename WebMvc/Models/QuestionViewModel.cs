
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SurveyApp.WebMvc.Models
{
    public class QuestionViewModel
    {
        public Guid Id { get; set; }
        
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }
        
        public string Description { get; set; }
        
        [Required(ErrorMessage = "Question type is required")]
        public string Type { get; set; }
        
        public bool Required { get; set; }
        
        public List<string> Options { get; set; } = new List<string>();
        
        public int Order { get; set; }
        
        public ValidationRulesViewModel ValidationRules { get; set; } = new ValidationRulesViewModel();
        
        public QuestionSettingsViewModel Settings { get; set; } = new QuestionSettingsViewModel();
    }

    public class QuestionSettingsViewModel
    {
        // Para preguntas tipo Rating
        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }
        
        // Para preguntas tipo NPS
        public string LowLabel { get; set; } = "No es probable";
        public string MiddleLabel { get; set; } = "Neutral";
        public string HighLabel { get; set; } = "Muy probable";
    }

    public class ValidationRulesViewModel
    {
        public int? MinLength { get; set; }
        public int? MaxLength { get; set; }
        public string Pattern { get; set; }
        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }
    }
}
