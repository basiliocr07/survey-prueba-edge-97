
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using SurveyApp.Domain.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Text.Json;

namespace SurveyApp.Infrastructure.Services
{
    public class MailKitEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<MailKitEmailService> _logger;
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;
        private readonly string _fromName;
        private readonly bool _useSsl;

        public MailKitEmailService(IConfiguration configuration, ILogger<MailKitEmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            // Cargar configuración desde appsettings.json usando EmailSettings
            _smtpServer = _configuration["EmailSettings:SmtpServer"] ?? "smtp.gmail.com";
            _smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
            _smtpUsername = _configuration["EmailSettings:SmtpUsername"] ?? "";
            _smtpPassword = _configuration["EmailSettings:SmtpPassword"] ?? "";
            _fromEmail = _configuration["EmailSettings:SenderEmail"] ?? "";
            _fromName = _configuration["EmailSettings:SenderName"] ?? "Sistema de Encuestas";
            _useSsl = bool.Parse(_configuration["EmailSettings:UseSsl"] ?? "true");
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string htmlContent)
        {
            try
            {
                _logger.LogInformation($"Preparando para enviar correo a {to}");

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_fromName, _fromEmail));
                message.To.Add(new MailboxAddress("", to));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = htmlContent
                };

                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                
                // Conectar al servidor SMTP
                await client.ConnectAsync(_smtpServer, _smtpPort, _useSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);
                
                // Si hay credenciales, autenticar
                if (!string.IsNullOrEmpty(_smtpUsername) && !string.IsNullOrEmpty(_smtpPassword))
                {
                    await client.AuthenticateAsync(_smtpUsername, _smtpPassword);
                }
                
                // Enviar el correo
                await client.SendAsync(message);
                
                // Desconectar
                await client.DisconnectAsync(true);
                
                _logger.LogInformation($"Correo enviado correctamente a {to}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al enviar correo a {to}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SendBulkEmailAsync(List<string> recipients, string subject, string htmlContent)
        {
            if (recipients == null || recipients.Count == 0)
            {
                _logger.LogWarning("No hay destinatarios para enviar correos masivos");
                return false;
            }

            _logger.LogInformation($"Enviando correo masivo a {recipients.Count} destinatarios");
            
            bool allSucceeded = true;
            List<string> failedRecipients = new List<string>();

            foreach (var recipient in recipients)
            {
                var success = await SendEmailAsync(recipient, subject, htmlContent);
                if (!success)
                {
                    allSucceeded = false;
                    failedRecipients.Add(recipient);
                    _logger.LogWarning($"Falló al enviar correo a {recipient}");
                }
            }

            if (failedRecipients.Count > 0)
            {
                _logger.LogWarning($"Fallaron {failedRecipients.Count} de {recipients.Count} envíos. Destinatarios fallidos: {JsonSerializer.Serialize(failedRecipients)}");
            }
            
            return allSucceeded;
        }

        public async Task<(bool Success, string ErrorMessage)> TestConnectionAsync()
        {
            try
            {
                using var client = new SmtpClient();
                
                _logger.LogInformation($"Probando conexión a {_smtpServer}:{_smtpPort}");
                
                // Conectar al servidor SMTP
                await client.ConnectAsync(_smtpServer, _smtpPort, _useSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);
                
                // Si hay credenciales, autenticar
                if (!string.IsNullOrEmpty(_smtpUsername) && !string.IsNullOrEmpty(_smtpPassword))
                {
                    await client.AuthenticateAsync(_smtpUsername, _smtpPassword);
                }
                
                // Desconectar
                await client.DisconnectAsync(true);
                
                _logger.LogInformation("Conexión SMTP exitosa");
                return (true, "Conexión SMTP exitosa");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al probar conexión SMTP: {ex.Message}");
                return (false, $"Error de conexión: {ex.Message}");
            }
        }
    }
}
