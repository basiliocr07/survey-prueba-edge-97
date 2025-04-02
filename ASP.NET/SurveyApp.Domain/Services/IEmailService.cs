
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SurveyApp.Domain.Services
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string to, string subject, string htmlContent);
        Task<bool> SendBulkEmailAsync(List<string> to, string subject, string htmlContent);
        Task<(bool Success, string ErrorMessage)> TestConnectionAsync();
    }
}
