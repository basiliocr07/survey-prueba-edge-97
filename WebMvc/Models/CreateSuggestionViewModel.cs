
using System.ComponentModel.DataAnnotations;

namespace SurveyApp.WebMvc.Models
{
    public class CreateSuggestionViewModel
    {
        [Required(ErrorMessage = "El contenido es obligatorio")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "El contenido debe tener entre 10 y 2000 caracteres")]
        [Display(Name = "Contenido")]
        public string Content { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        [Display(Name = "Nombre")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Dirección de email inválida")]
        [Display(Name = "Email")]
        public string CustomerEmail { get; set; }

        [StringLength(20, ErrorMessage = "El teléfono no puede exceder los 20 caracteres")]
        [Display(Name = "Teléfono")]
        public string Phone { get; set; }

        [StringLength(100, ErrorMessage = "El nombre de la compañía no puede exceder los 100 caracteres")]
        [Display(Name = "Compañía")]
        public string Company { get; set; }

        [Display(Name = "Categoría")]
        public string Category { get; set; }

        [Display(Name = "Enviar anónimamente")]
        public bool IsAnonymous { get; set; }

        // Indica si es un cliente existente
        [Display(Name = "¿Ya es cliente?")]
        public bool IsExistingClient { get; set; }

        // ID del cliente si ya existe
        public Guid? ExistingClientId { get; set; }
    }
}
