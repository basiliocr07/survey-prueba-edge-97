
using System.ComponentModel.DataAnnotations;

namespace SurveyApp.WebMvc.Models
{
    public class CreateSuggestionViewModel
    {
        [Required(ErrorMessage = "El contenido es obligatorio")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "El contenido debe tener entre 10 y 2000 caracteres")]
        public string Content { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres")]
        public string CustomerName { get; set; }

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Dirección de email inválida")]
        public string CustomerEmail { get; set; }

        public string Category { get; set; }

        public bool IsAnonymous { get; set; }
    }
}
