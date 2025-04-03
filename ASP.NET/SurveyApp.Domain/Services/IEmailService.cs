
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SurveyApp.Domain.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string to, string subject, string htmlContent);
        Task<bool> SendBulkEmailAsync(List<string> to, string subject, string htmlContent);
        Task<(bool Success, string ErrorMessage)> TestConnectionAsync();
        
        // Nuevos métodos para recibir emails y notificar
        Task<(bool Success, List<EmailMessage> Messages, string ErrorMessage)> CheckEmailsAsync();
        Task<bool> SendNotificationAsync(EmailMessage message);
    }
    
    public class EmailMessage
    {
        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string HtmlBody { get; set; }
        public string TextBody { get; set; }
        public string MessageId { get; set; }
        public System.DateTime Date { get; set; }
        public List<string> Attachments { get; set; }
        
        public EmailMessage()
        {
            Attachments = new List<string>();
        }
    }
}
