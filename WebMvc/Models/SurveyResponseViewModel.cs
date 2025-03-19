
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
        public List<QuestionViewModel> Questions { get; set; } = new List<QuestionViewModel>();
        
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
        [Display(Name = "Nombre")]
        public string RespondentName { get; set; }
        
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Por favor, introduce un email válido")]
        [StringLength(200, ErrorMessage = "El email no puede superar los 200 caracteres")]
        [Display(Name = "Email")]
        public string RespondentEmail { get; set; }
        
        [StringLength(50, ErrorMessage = "El teléfono no puede superar los 50 caracteres")]
        [Display(Name = "Teléfono")]
        public string RespondentPhone { get; set; }
        
        [StringLength(100, ErrorMessage = "La compañía no puede superar los 100 caracteres")]
        [Display(Name = "Compañía")]
        public string RespondentCompany { get; set; }
        
        // Propiedad para almacenar las respuestas
        public Dictionary<string, object> Answers { get; set; } = new Dictionary<string, object>();
        
        // Flag para indicar si es un cliente existente
        public bool IsExistingClient { get; set; }
        
        // ID del cliente si ya existe
        public Guid? ExistingClientId { get; set; }
        
        // Fecha de envío de la respuesta
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        
        // Propiedades para la visualización en el dashboard
        public string RespondentInfo => $"{RespondentName} ({RespondentEmail})";
        public string CompanyInfo => !string.IsNullOrEmpty(RespondentCompany) ? RespondentCompany : "No especificado";
    }

    public class QuestionViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public bool Required { get; set; }
        public List<string> Options { get; set; } = new List<string>();
    }

    public class SurveyResponseInputModel
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede superar los 100 caracteres")]
        [Display(Name = "Nombre")]
        public string RespondentName { get; set; }
        
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Por favor, introduce un email válido")]
        [StringLength(200, ErrorMessage = "El email no puede superar los 200 caracteres")]
        [Display(Name = "Email")]
        public string RespondentEmail { get; set; }
        
        [StringLength(50, ErrorMessage = "El teléfono no puede superar los 50 caracteres")]
        [Display(Name = "Teléfono")]
        public string RespondentPhone { get; set; }
        
        [StringLength(100, ErrorMessage = "La compañía no puede superar los 100 caracteres")]
        [Display(Name = "Compañía")]
        public string RespondentCompany { get; set; }
        
        public Guid SurveyId { get; set; }
        public Dictionary<string, object> Answers { get; set; } = new Dictionary<string, object>();
        
        // Flag para indicar si es un cliente existente
        public bool IsExistingClient { get; set; }
        
        // ID del cliente si ya existe
        public Guid? ExistingClientId { get; set; }
    }
    
    // Nuevo modelo para las respuestas en la vista de análisis
    public class SurveyResponseAnalyticsViewModel
    {
        public Guid Id { get; set; }
        public Guid SurveyId { get; set; }
        public string SurveyTitle { get; set; }
        public string RespondentName { get; set; }
        public string RespondentEmail { get; set; }
        public string RespondentCompany { get; set; }
        public DateTime SubmittedAt { get; set; }
        public List<QuestionAnswerViewModel> Answers { get; set; } = new List<QuestionAnswerViewModel>();
        public bool IsValidated { get; set; }
    }
    
    public class QuestionAnswerViewModel
    {
        public Guid QuestionId { get; set; }
        public string QuestionTitle { get; set; }
        public string QuestionType { get; set; }
        public string Answer { get; set; }
        public List<string> MultipleAnswers { get; set; }
        public bool IsValid { get; set; } = true;
    }
}
