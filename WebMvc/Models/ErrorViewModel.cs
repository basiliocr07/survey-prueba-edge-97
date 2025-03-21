
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace SurveyApp.WebMvc.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public Exception? Exception { get; set; }
        public string LogReference { get; set; } = string.Empty;
        
        // Database connection information for troubleshooting
        public string DatabaseConnectionInfo { get; set; } = string.Empty;
        
        // User authentication and role information
        public bool IsAuthenticated { get; set; }
        public string Username { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty;
        
        // Additional diagnostic information
        public bool IsDatabaseError { get; set; }
        public string ErrorSource { get; set; } = string.Empty;
        public DateTime ErrorTimestamp { get; set; } = DateTime.UtcNow;
        
        // Stack trace information
        public string StackTrace { get; set; } = string.Empty;
        
        // Route information
        public string ControllerName { get; set; } = string.Empty;
        public string ActionName { get; set; } = string.Empty;
        
        // HTTP context information
        public string HttpMethod { get; set; } = string.Empty;
        public string QueryString { get; set; } = string.Empty;
        
        // Form data summary
        public string FormDataSummary { get; set; } = string.Empty;
        
        // Error type classification
        public string ErrorType { get; set; } = string.Empty;
        
        // Model validation errors
        public Dictionary<string, List<string>> ValidationErrors { get; set; } = new Dictionary<string, List<string>>();
        
        // Request body content (for API debugging)
        public string RequestBody { get; set; } = string.Empty;
        
        // Response status code
        public int? StatusCode { get; set; }
        
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        
        // Helper method to create a comprehensive error model
        public static ErrorViewModel CreateDetailedError(Exception ex, string requestId = null, string message = null)
        {
            return new ErrorViewModel
            {
                RequestId = requestId ?? string.Empty,
                Message = message ?? ex.Message,
                Exception = ex,
                StackTrace = ex.StackTrace ?? string.Empty,
                ErrorTimestamp = DateTime.UtcNow,
                ErrorSource = ex.Source ?? string.Empty,
                ErrorType = ex.GetType().Name
            };
        }
        
        // Helper method to add validation errors
        public void AddValidationError(string key, string errorMessage)
        {
            if (!ValidationErrors.ContainsKey(key))
            {
                ValidationErrors[key] = new List<string>();
            }
            
            ValidationErrors[key].Add(errorMessage);
        }
        
        // New helper method to add model errors from ModelState
        public void AddModelErrors(Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary modelState)
        {
            foreach (var key in modelState.Keys)
            {
                foreach (var error in modelState[key].Errors)
                {
                    AddValidationError(key, error.ErrorMessage);
                }
            }
        }
        
        // Helper to capture form data
        public void CaptureFormData(Microsoft.AspNetCore.Http.IFormCollection form)
        {
            if (form == null || form.Count == 0)
            {
                FormDataSummary = "No form data submitted";
                return;
            }
            
            var sb = new StringBuilder();
            sb.AppendLine("Form Data:");
            
            foreach (var key in form.Keys)
            {
                var values = form[key];
                sb.AppendLine($"- {key}: {string.Join(", ", values)}");
            }
            
            FormDataSummary = sb.ToString();
        }
        
        // New helper method to summarize the error
        public string GetErrorSummary()
        {
            return $"[{ErrorType}] {Message}";
        }
    }
}
