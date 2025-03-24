
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SurveyApp.Web.Models
{
    public class SurveySubmissionViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }

        public string Phone { get; set; }

        public string Company { get; set; }

        public Dictionary<string, string> Answers { get; set; } = new Dictionary<string, string>();
    }
}
