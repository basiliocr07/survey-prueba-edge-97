
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SurveyApp.Domain.Services;

namespace SurveyApp.Infrastructure.EmailService
{
    public class EmailSender : IEmailSender
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IEmailService emailService, ILogger<EmailSender> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                await _emailService.SendEmailAsync(email, subject, htmlMessage);
                _logger.LogInformation($"Email enviado a {email} con asunto '{subject}'");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error enviando correo a {email}: {ex.Message}");
                throw;
            }
        }

        public async Task SendBulkEmailAsync(List<string> to, string subject, string htmlContent)
        {
            await _emailService.SendBulkEmailAsync(to, subject, htmlContent);
        }

        public async Task<(bool Success, string ErrorMessage)> TestConnectionAsync()
        {
            return await _emailService.TestConnectionAsync();
        }
    }
}
