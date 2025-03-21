
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
        
        // Stack trace information
        public string StackTrace { get; set; }
        
        // Route information
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        
        // HTTP context information
        public string HttpMethod { get; set; }
        public string QueryString { get; set; }
        
        // Form data summary - NEW
        public string FormDataSummary { get; set; }
        
        // Error type classification - NEW
        public string ErrorType { get; set; }
        
        // Model validation errors - NEW
        public Dictionary<string, List<string>> ValidationErrors { get; set; } = new Dictionary<string, List<string>>();
        
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        
        // Helper method to create a comprehensive error model
        public static ErrorViewModel CreateDetailedError(Exception ex, string requestId = null, string message = null)
        {
            return new ErrorViewModel
            {
                RequestId = requestId,
                Message = message ?? ex.Message,
                Exception = ex,
                StackTrace = ex.StackTrace,
                ErrorTimestamp = DateTime.UtcNow,
                ErrorSource = ex.Source,
                ErrorType = ex.GetType().Name
            };
        }
        
        // New helper method to add validation errors
        public void AddValidationError(string key, string errorMessage)
        {
            if (!ValidationErrors.ContainsKey(key))
            {
                ValidationErrors[key] = new List<string>();
            }
            
            ValidationErrors[key].Add(errorMessage);
        }
    }
}
