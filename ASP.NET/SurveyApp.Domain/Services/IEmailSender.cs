
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SurveyApp.Domain.Services
{
    // Esta interfaz se mantiene para compatibilidad con el código existente
    // pero delegará sus implementaciones a IEmailService
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string htmlMessage);
        Task SendBulkEmailAsync(List<string> to, string subject, string htmlContent);
        Task<(bool Success, string ErrorMessage)> TestConnectionAsync();
    }
}
