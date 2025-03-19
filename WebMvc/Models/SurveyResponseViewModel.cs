
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SurveyApp.WebMvc.Models
{
    public class SurveyResponseViewModel
    {
        public Guid SurveyId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<QuestionViewModel> Questions { get; set; }
        
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string RespondentName { get; set; }
        
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Por favor, introduce un email válido")]
        public string RespondentEmail { get; set; }
    }

    public class QuestionViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public bool Required { get; set; }
        public List<string> Options { get; set; }
    }

    public class SurveyResponseInputModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string RespondentName { get; set; }
        
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Por favor, introduce un email válido")]
        public string RespondentEmail { get; set; }
        
        public Dictionary<string, object> Answers { get; set; } = new Dictionary<string, object>();
    }
}
