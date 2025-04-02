
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using SurveyApp.Domain.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace SurveyApp.Infrastructure.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmtpEmailService> _logger;
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _senderEmail;
        private readonly string _senderName;

        public SmtpEmailService(IConfiguration configuration, ILogger<SmtpEmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            // Cargar configuración desde appsettings.json o usar valores por defecto
            _smtpServer = _configuration["Email:SmtpServer"] ?? "smtp.gmail.com";
            _smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
            _smtpUsername = _configuration["Email:SmtpUsername"] ?? "crisant231@gmail.com";
            _smtpPassword = _configuration["Email:SmtpPassword"] ?? "avufrkhruqddomsh";
            _senderEmail = _configuration["Email:SenderEmail"] ?? "crisant231@gmail.com";
            _senderName = _configuration["Email:SenderName"] ?? "Sistema de Encuestas";
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string htmlContent)
        {
            try
            {
                _logger.LogInformation($"Preparing to send email to {to}");
                
                using var message = new MailMessage();
                message.From = new MailAddress(_senderEmail, _senderName);
                message.To.Add(new MailAddress(to));
                message.Subject = subject;
                message.Body = htmlContent;
                message.IsBodyHtml = true;

                using var client = new SmtpClient(_smtpServer, _smtpPort);
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                
                _logger.LogInformation($"Connecting to SMTP server: {_smtpServer}:{_smtpPort}");
                await client.SendMailAsync(message);
                _logger.LogInformation($"Email sent successfully to {to}");
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending email to {to}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SendBulkEmailAsync(List<string> recipients, string subject, string htmlContent)
        {
            bool allSucceeded = true;
            
            foreach (var recipient in recipients)
            {
                var success = await SendEmailAsync(recipient, subject, htmlContent);
                if (!success)
                {
                    allSucceeded = false;
                    _logger.LogWarning($"Failed to send email to {recipient}");
                }
            }
            
            return allSucceeded;
        }
    }
}
