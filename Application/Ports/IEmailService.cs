
using System.Threading.Tasks;

namespace SurveyApp.Application.Ports
{
    public interface IEmailService
    {
        Task SendSurveyInvitationAsync(string toEmail, string surveyTitle, string surveyLink);
        
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
