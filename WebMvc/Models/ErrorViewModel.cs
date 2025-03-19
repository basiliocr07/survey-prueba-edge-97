
namespace SurveyApp.WebMvc.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; } = string.Empty;
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public string Message { get; set; } = string.Empty;
        
        // User authentication and role properties
        public bool IsAuthenticated { get; set; }
        public string UserRole { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
    }
}
