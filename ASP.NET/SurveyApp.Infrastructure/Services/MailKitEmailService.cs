
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using SurveyApp.Domain.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Net.Imap;
using MailKit.Net.Pop3;
using MailKit.Security;
using System.Text.Json;
using System.Linq;
using System.IO;

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
        
        // Configuración para recibir emails
        private readonly string _imapServer;
        private readonly int _imapPort;
        private readonly bool _useImapSsl;
        private readonly string _popServer;
        private readonly int _popPort;
        private readonly bool _usePopSsl;
        private readonly string _protocol;

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
            
            // Configuración para recibir emails
            _protocol = _configuration["EmailSettings:IncomingProtocol"] ?? "imap";
            _imapServer = _configuration["EmailSettings:ImapServer"] ?? "imap.gmail.com";
            _imapPort = int.Parse(_configuration["EmailSettings:ImapPort"] ?? "993");
            _useImapSsl = bool.Parse(_configuration["EmailSettings:UseImapSsl"] ?? "true");
            _popServer = _configuration["EmailSettings:PopServer"] ?? "pop.gmail.com";
            _popPort = int.Parse(_configuration["EmailSettings:PopPort"] ?? "995");
            _usePopSsl = bool.Parse(_configuration["EmailSettings:UsePopSsl"] ?? "true");
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
                // Probar conexión SMTP para envío
                using var smtpClient = new SmtpClient();
                
                _logger.LogInformation($"Probando conexión a {_smtpServer}:{_smtpPort}");
                
                await smtpClient.ConnectAsync(_smtpServer, _smtpPort, _useSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);
                
                if (!string.IsNullOrEmpty(_smtpUsername) && !string.IsNullOrEmpty(_smtpPassword))
                {
                    await smtpClient.AuthenticateAsync(_smtpUsername, _smtpPassword);
                }
                
                await smtpClient.DisconnectAsync(true);
                _logger.LogInformation("Conexión SMTP exitosa");
                
                // Probar conexión para recepción según protocolo
                if (_protocol.ToLower() == "imap")
                {
                    using var imapClient = new ImapClient();
                    _logger.LogInformation($"Probando conexión IMAP a {_imapServer}:{_imapPort}");
                    
                    await imapClient.ConnectAsync(_imapServer, _imapPort, _useImapSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.Auto);
                    
                    if (!string.IsNullOrEmpty(_smtpUsername) && !string.IsNullOrEmpty(_smtpPassword))
                    {
                        await imapClient.AuthenticateAsync(_smtpUsername, _smtpPassword);
                    }
                    
                    await imapClient.DisconnectAsync(true);
                    _logger.LogInformation("Conexión IMAP exitosa");
                }
                else if (_protocol.ToLower() == "pop3")
                {
                    using var popClient = new Pop3Client();
                    _logger.LogInformation($"Probando conexión POP3 a {_popServer}:{_popPort}");
                    
                    await popClient.ConnectAsync(_popServer, _popPort, _usePopSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.Auto);
                    
                    if (!string.IsNullOrEmpty(_smtpUsername) && !string.IsNullOrEmpty(_smtpPassword))
                    {
                        await popClient.AuthenticateAsync(_smtpUsername, _smtpPassword);
                    }
                    
                    await popClient.DisconnectAsync(true);
                    _logger.LogInformation("Conexión POP3 exitosa");
                }
                
                return (true, "Conexiones de envío y recepción exitosas");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al probar conexiones: {ex.Message}");
                return (false, $"Error de conexión: {ex.Message}");
            }
        }
        
        public async Task<(bool Success, List<EmailMessage> Messages, string ErrorMessage)> CheckEmailsAsync()
        {
            var messages = new List<EmailMessage>();
            
            try
            {
                _logger.LogInformation($"Revisando correos entrantes usando {_protocol}");
                
                if (_protocol.ToLower() == "imap")
                {
                    using var client = new ImapClient();
                    await client.ConnectAsync(_imapServer, _imapPort, _useImapSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.Auto);
                    
                    if (!string.IsNullOrEmpty(_smtpUsername) && !string.IsNullOrEmpty(_smtpPassword))
                    {
                        await client.AuthenticateAsync(_smtpUsername, _smtpPassword);
                    }
                    
                    var inbox = client.Inbox;
                    await inbox.OpenAsync(MailKit.FolderAccess.ReadOnly);
                    
                    _logger.LogInformation($"IMAP: Total de mensajes en bandeja: {inbox.Count}");
                    
                    // Obtener los 10 mensajes más recientes
                    int count = Math.Min(10, inbox.Count);
                    int startIndex = Math.Max(0, inbox.Count - count);
                    
                    for (int i = startIndex; i < inbox.Count; i++)
                    {
                        var message = await inbox.GetMessageAsync(i);
                        var emailMessage = new EmailMessage
                        {
                            From = message.From.ToString(),
                            Subject = message.Subject,
                            Date = message.Date.DateTime,
                            MessageId = message.MessageId
                        };
                        
                        if (message.HtmlBody != null)
                            emailMessage.HtmlBody = message.HtmlBody;
                        
                        if (message.TextBody != null)
                            emailMessage.TextBody = message.TextBody;
                        
                        emailMessage.Body = emailMessage.HtmlBody ?? emailMessage.TextBody ?? "";
                        
                        // Procesar adjuntos si hay
                        foreach (var attachment in message.Attachments)
                        {
                            if (attachment is MimePart mimePart)
                            {
                                emailMessage.Attachments.Add(mimePart.FileName);
                            }
                        }
                        
                        messages.Add(emailMessage);
                    }
                    
                    await client.DisconnectAsync(true);
                }
                else if (_protocol.ToLower() == "pop3")
                {
                    using var client = new Pop3Client();
                    await client.ConnectAsync(_popServer, _popPort, _usePopSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.Auto);
                    
                    if (!string.IsNullOrEmpty(_smtpUsername) && !string.IsNullOrEmpty(_smtpPassword))
                    {
                        await client.AuthenticateAsync(_smtpUsername, _smtpPassword);
                    }
                    
                    _logger.LogInformation($"POP3: Total de mensajes en bandeja: {client.Count}");
                    
                    // Obtener los 10 mensajes más recientes
                    int count = Math.Min(10, client.Count);
                    int startIndex = Math.Max(0, client.Count - count);
                    
                    for (int i = startIndex; i < client.Count; i++)
                    {
                        var message = await client.GetMessageAsync(i);
                        var emailMessage = new EmailMessage
                        {
                            From = message.From.ToString(),
                            Subject = message.Subject,
                            Date = message.Date.DateTime,
                            MessageId = message.MessageId
                        };
                        
                        if (message.HtmlBody != null)
                            emailMessage.HtmlBody = message.HtmlBody;
                        
                        if (message.TextBody != null)
                            emailMessage.TextBody = message.TextBody;
                        
                        emailMessage.Body = emailMessage.HtmlBody ?? emailMessage.TextBody ?? "";
                        
                        foreach (var attachment in message.Attachments)
                        {
                            if (attachment is MimePart mimePart)
                            {
                                emailMessage.Attachments.Add(mimePart.FileName);
                            }
                        }
                        
                        messages.Add(emailMessage);
                    }
                    
                    await client.DisconnectAsync(true);
                }
                
                return (true, messages, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al revisar correos: {ex.Message}");
                return (false, messages, $"Error al revisar correos: {ex.Message}");
            }
        }
        
        public async Task<bool> SendNotificationAsync(EmailMessage message)
        {
            try
            {
                string subject = $"Nuevo correo recibido: {message.Subject}";
                string htmlContent = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background-color: #4F46E5; color: white; padding: 10px 20px; border-radius: 5px 5px 0 0; }}
        .content {{ background-color: #f9f9f9; padding: 20px; border: 1px solid #ddd; }}
        .message-details {{ background-color: white; padding: 15px; border: 1px solid #eee; margin-top: 20px; }}
        .footer {{ text-align: center; margin-top: 20px; font-size: 0.8em; color: #666; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h2>Notificación: Nuevo correo recibido</h2>
        </div>
        <div class='content'>
            <p>Has recibido un nuevo correo en tu sistema de encuestas.</p>
            
            <div class='message-details'>
                <p><strong>De:</strong> {message.From}</p>
                <p><strong>Asunto:</strong> {message.Subject}</p>
                <p><strong>Fecha:</strong> {message.Date}</p>
                <p><strong>Mensaje:</strong></p>
                <div>{message.Body}</div>
                
                {(message.Attachments.Count > 0 ? $"<p><strong>Adjuntos:</strong> {string.Join(", ", message.Attachments)}</p>" : "")}
            </div>
            
            <p>Este es un mensaje automático, por favor no responda directamente a este correo.</p>
        </div>
        <div class='footer'>
            <p>© {DateTime.Now.Year} Sistema de Encuestas</p>
        </div>
    </div>
</body>
</html>";

                return await SendEmailAsync(_fromEmail, subject, htmlContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al enviar notificación: {ex.Message}");
                return false;
            }
        }
    }
}
