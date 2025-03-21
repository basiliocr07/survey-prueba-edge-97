
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
        
        // Propiedades adicionales para analytics
        public double CompletionTime { get; set; }
        public string DeviceType { get; set; }
        public string Browser { get; set; }
        public string Location { get; set; }
        
        // Artículos del knowledge base relacionados
        public List<KnowledgeItemViewModel> RelatedKnowledgeItems { get; set; } = new List<KnowledgeItemViewModel>();
        
        // Propiedades para la visualización en el dashboard
        public string RespondentInfo => $"{RespondentName} ({RespondentEmail})";
        public string CompanyInfo => !string.IsNullOrEmpty(RespondentCompany) ? RespondentCompany : "No especificado";
    }
    
    public class KnowledgeItemViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
    }
    
    public class ThankYouViewModel
    {
        public string SurveyTitle { get; set; }
        public string ThankYouMessage { get; set; }
        public List<SurveyListItemViewModel> RelatedSurveys { get; set; } = new List<SurveyListItemViewModel>();
    }
    
    public class SurveyListItemViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public int ResponseCount { get; set; }
    }
}
