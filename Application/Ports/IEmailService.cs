
using System.Threading.Tasks;

namespace SurveyApp.Application.Ports
{
    public interface IEmailService
    {
        Task SendSurveyInvitationAsync(string toEmail, string surveyTitle, string surveyLink);
        
        Task SendEmailAsync(string toEmail, string subject, string body);
        
        // Método para pruebas que devuelve información sobre el envío
        Task<bool> TestEmailServiceAsync(string toEmail);
    }
}
