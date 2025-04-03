
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SurveyApp.Web.Models
{
    public class CustomerFormViewModel
    {
        [Required(ErrorMessage = "El nombre de marca es obligatorio")]
        public string BrandName { get; set; }

        [Required(ErrorMessage = "El nombre de contacto es obligatorio")]
        public string ContactName { get; set; }

        [Required(ErrorMessage = "El email de contacto es obligatorio")]
        [EmailAddress(ErrorMessage = "Introduce un email válido")]
        public string ContactEmail { get; set; }

        public string ContactPhone { get; set; }
        public string CustomerType { get; set; } = "client";
        public List<string> SelectedServices { get; set; } = new List<string>();
    }

    public class UpdateCustomerViewModel : CustomerFormViewModel
    {
        public int Id { get; set; }
    }
}
