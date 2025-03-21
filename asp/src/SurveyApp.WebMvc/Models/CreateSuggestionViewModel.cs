
using System.ComponentModel.DataAnnotations;

namespace SurveyApp.WebMvc.Models
{
    public class CreateSuggestionViewModel
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }
        
        [Required(ErrorMessage = "Content is required")]
        [MinLength(10, ErrorMessage = "Suggestion must be at least 10 characters")]
        public string Content { get; set; }
        
        [Required(ErrorMessage = "Name is required")]
        public string CustomerName { get; set; }
        
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string CustomerEmail { get; set; }
        
        public string Category { get; set; }
        
        public bool IsAnonymous { get; set; } = false;
        
        // Campos adicionales para clientes corporativos
        public string Company { get; set; }
        
        public string Phone { get; set; }
    }
}
