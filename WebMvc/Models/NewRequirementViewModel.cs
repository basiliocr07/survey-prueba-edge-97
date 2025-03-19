
using System;
using System.ComponentModel.DataAnnotations;

namespace SurveyApp.WebMvc.Models
{
    public class NewRequirementViewModel
    {
        [Required(ErrorMessage = "El título es requerido.")]
        [Display(Name = "Título")]
        [StringLength(100, ErrorMessage = "El título no puede superar los 100 caracteres.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "La descripción es requerida.")]
        [Display(Name = "Descripción")]
        public string Description { get; set; }

        [Display(Name = "Prioridad")]
        public string Priority { get; set; } = "Media";

        [Display(Name = "Área del Proyecto")]
        public string ProjectArea { get; set; } = "General";
        
        [Display(Name = "Estado")]
        public string Status { get; set; } = "Propuesto";
        
        [Display(Name = "Nombre del Cliente")]
        public string CustomerName { get; set; }
        
        [EmailAddress]
        [Display(Name = "Email del Cliente")]
        public string CustomerEmail { get; set; }
    }
}
