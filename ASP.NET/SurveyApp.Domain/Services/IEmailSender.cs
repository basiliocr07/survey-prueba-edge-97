
using System.Threading.Tasks;

namespace SurveyApp.Domain.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}
