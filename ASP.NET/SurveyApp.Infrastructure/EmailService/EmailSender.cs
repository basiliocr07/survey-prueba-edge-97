
using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using SurveyApp.Domain.Services;

namespace SurveyApp.Infrastructure.EmailService
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IConfiguration configuration, ILogger<EmailSender> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_configuration["EmailSettings:SenderName"], _configuration["EmailSettings:SenderEmail"]));
                message.To.Add(new MailboxAddress("", email));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = htmlMessage
                };

                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(
                    _configuration["EmailSettings:SmtpServer"],
                    int.Parse(_configuration["EmailSettings:Port"]),
                    bool.Parse(_configuration["EmailSettings:UseSsl"]));

                // Si se requiere autenticación
                if (!string.IsNullOrEmpty(_configuration["EmailSettings:Username"]) && 
                    !string.IsNullOrEmpty(_configuration["EmailSettings:Password"]))
                {
                    await client.AuthenticateAsync(
                        _configuration["EmailSettings:Username"],
                        _configuration["EmailSettings:Password"]);
                }

                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation($"Email enviado a {email} con asunto '{subject}'");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error enviando correo a {email}: {ex.Message}");
                throw;
            }
        }
    }
}
