
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SurveyApp.WebMvc.Models
{
    public class UserViewModel
    {
        public string Id { get; set; }
        
        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        public string Username { get; set; }
        
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Por favor, introduce un email válido")]
        public string Email { get; set; }
        
        public string Role { get; set; } // "Admin" o "Client"
        
        public DateTime CreatedAt { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "La contraseña actual es obligatoria")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }
        
        [Required(ErrorMessage = "La nueva contraseña es obligatoria")]
        [StringLength(100, ErrorMessage = "La {0} debe tener al menos {2} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Las contraseñas nuevas no coinciden.")]
        public string ConfirmNewPassword { get; set; }
    }

    public class UserListViewModel
    {
        public List<UserViewModel> Users { get; set; }
    }
}
