
using System;

namespace SurveyApp.WebMvc.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
        public string LogReference { get; set; }
        
        // Database connection information for troubleshooting
        public string DatabaseConnectionInfo { get; set; }
        
        // User authentication and role information
        public bool IsAuthenticated { get; set; }
        public string Username { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
        
        // Additional diagnostic information
        public bool IsDatabaseError { get; set; }
        public string ErrorSource { get; set; }
        public DateTime ErrorTimestamp { get; set; } = DateTime.UtcNow;
        
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
