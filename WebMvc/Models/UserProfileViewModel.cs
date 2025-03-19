
namespace SurveyApp.WebMvc.Models
{
    public class UserProfileViewModel
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime? LastLogin { get; set; }
        public int TotalSurveys { get; set; }
        public int TotalResponses { get; set; }
    }
}
