
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SurveyApp.WebMvc.Models
{
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
}
