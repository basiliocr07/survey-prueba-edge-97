using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SurveyApp.Application.Ports;

namespace SurveyApp.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly EmailSettings _emailSettings;

        public EmailService(ILogger<EmailService> logger, IOptions<EmailSettings> emailSettings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailSettings = emailSettings?.Value ?? throw new ArgumentNullException(nameof(emailSettings));
            
            _logger.LogInformation("EmailService initialized with SMTP server: {SmtpServer}:{SmtpPort}, Sender: {SenderEmail}",
                _emailSettings.SmtpServer, _emailSettings.SmtpPort, _emailSettings.SenderEmail);
        }

        public async Task SendSurveyInvitationAsync(string toEmail, string surveyTitle, string surveyLink)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
            {
                _logger.LogError("Cannot send email: Recipient email is null or empty");
                throw new ArgumentException("Recipient email cannot be null or empty", nameof(toEmail));
            }

            if (string.IsNullOrWhiteSpace(surveyTitle))
            {
                _logger.LogError("Cannot send email: Survey title is null or empty");
                throw new ArgumentException("Survey title cannot be null or empty", nameof(surveyTitle));
            }

            if (string.IsNullOrWhiteSpace(surveyLink))
            {
                _logger.LogError("Cannot send email: Survey link is null or empty");
                throw new ArgumentException("Survey link cannot be null or empty", nameof(surveyLink));
            }

            _logger.LogInformation("Preparing to send survey invitation email to {Email} for survey: {SurveyTitle}", 
                toEmail, surveyTitle);
                
            try
            {
                // Verificar la configuración de SMTP
                if (string.IsNullOrWhiteSpace(_emailSettings.SmtpServer))
                {
                    _logger.LogError("SMTP server is not configured");
                    throw new InvalidOperationException("SMTP server is not configured");
                }
                
                if (string.IsNullOrWhiteSpace(_emailSettings.SenderEmail))
                {
                    _logger.LogError("Sender email is not configured");
                    throw new InvalidOperationException("Sender email is not configured");
                }

                // Crear el mensaje con los datos proporcionados
                var message = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName ?? "Survey System"),
                    Subject = $"Invitación para responder encuesta: {surveyTitle}",
                    Body = GetEmailTemplate(toEmail, surveyTitle, surveyLink),
                    IsBodyHtml = true
                };
                
                message.To.Add(new MailAddress(toEmail));

                _logger.LogDebug("Email message created with subject: {Subject}", message.Subject);

                // Configurar el cliente SMTP
                using (var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort))
                {
                    _logger.LogDebug("Configuring SMTP client with SSL: {EnableSsl}", _emailSettings.EnableSsl);
                    
                    client.EnableSsl = _emailSettings.EnableSsl;
                    
                    // Verificar credenciales
                    if (string.IsNullOrWhiteSpace(_emailSettings.Username) || string.IsNullOrWhiteSpace(_emailSettings.Password))
                    {
                        _logger.LogWarning("SMTP username or password is not configured, attempting to send without authentication");
                    }
                    else
                    {
                        client.Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password);
                    }
                    
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;

                    // Enviar el correo electrónico
                    _logger.LogInformation("Sending email to {Email} through SMTP server {SmtpServer}:{SmtpPort}", 
                        toEmail, _emailSettings.SmtpServer, _emailSettings.SmtpPort);
                        
                    await client.SendMailAsync(message);
                    _logger.LogInformation("Email sent successfully to {Email} for survey '{SurveyTitle}'", 
                        toEmail, surveyTitle);
                }
            }
            catch (SmtpException ex)
            {
                _logger.LogError(ex, "SMTP error sending email to {Email}: {ErrorCode}, {Message}", 
                    toEmail, ex.StatusCode, ex.Message);
                throw new InvalidOperationException($"SMTP error sending email: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {Email}: {ErrorMessage}", toEmail, ex.Message);
                throw;
            }
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
            {
                _logger.LogError("Cannot send email: Recipient email is null or empty");
                throw new ArgumentException("Recipient email cannot be null or empty", nameof(toEmail));
            }

            if (string.IsNullOrWhiteSpace(subject))
            {
                _logger.LogError("Cannot send email: Subject is null or empty");
                throw new ArgumentException("Subject cannot be null or empty", nameof(subject));
            }

            if (string.IsNullOrWhiteSpace(body))
            {
                _logger.LogError("Cannot send email: Body is null or empty");
                throw new ArgumentException("Body cannot be null or empty", nameof(body));
            }

            _logger.LogInformation("Preparing to send email to {Email} with subject: {Subject}", 
                toEmail, subject);
                
            try
            {
                // Verificar la configuración de SMTP
                if (string.IsNullOrWhiteSpace(_emailSettings.SmtpServer))
                {
                    _logger.LogError("SMTP server is not configured");
                    throw new InvalidOperationException("SMTP server is not configured");
                }
                
                if (string.IsNullOrWhiteSpace(_emailSettings.SenderEmail))
                {
                    _logger.LogError("Sender email is not configured");
                    throw new InvalidOperationException("Sender email is not configured");
                }

                // Crear el mensaje con los datos proporcionados
                var message = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName ?? "Survey System"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                
                message.To.Add(new MailAddress(toEmail));

                _logger.LogDebug("Email message created with subject: {Subject}", message.Subject);

                // Configurar el cliente SMTP
                using (var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort))
                {
                    _logger.LogDebug("Configuring SMTP client with SSL: {EnableSsl}", _emailSettings.EnableSsl);
                    
                    client.EnableSsl = _emailSettings.EnableSsl;
                    
                    // Verificar credenciales
                    if (string.IsNullOrWhiteSpace(_emailSettings.Username) || string.IsNullOrWhiteSpace(_emailSettings.Password))
                    {
                        _logger.LogWarning("SMTP username or password is not configured, attempting to send without authentication");
                    }
                    else
                    {
                        client.Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password);
                    }
                    
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;

                    // Enviar el correo electrónico
                    _logger.LogInformation("Sending email to {Email} through SMTP server {SmtpServer}:{SmtpPort}", 
                        toEmail, _emailSettings.SmtpServer, _emailSettings.SmtpPort);
                        
                    await client.SendMailAsync(message);
                    _logger.LogInformation("Email sent successfully to {Email} with subject '{Subject}'", 
                        toEmail, subject);
                }
            }
            catch (SmtpException ex)
            {
                _logger.LogError(ex, "SMTP error sending email to {Email}: {ErrorCode}, {Message}", 
                    toEmail, ex.StatusCode, ex.Message);
                throw new InvalidOperationException($"SMTP error sending email: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {Email}: {ErrorMessage}", toEmail, ex.Message);
                throw;
            }
        }

        private string GetEmailTemplate(string toEmail, string surveyTitle, string surveyLink)
        {
            _logger.LogDebug("Generating email template for recipient {Email}, survey: {SurveyTitle}", 
                toEmail, surveyTitle);
                
            return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset='UTF-8'>
                <title>Invitación a Encuesta</title>
                <style>
                    body {{
                        font-family: 'Segoe UI', Arial, sans-serif;
                        line-height: 1.6;
                        color: #333;
                        max-width: 600px;
                        margin: 0 auto;
                        padding: 20px;
                    }}
                    .header {{
                        background-color: #1e40af;
                        color: white;
                        padding: a15px;
                        text-align: center;
                        border-radius: 5px 5px 0 0;
                    }}
                    .content {{
                        padding: 20px;
                        background-color: #f8fafc;
                        border: 1px solid #e2e8f0;
                    }}
                    .button {{
                        display: inline-block;
                        background-color: #3b82f6;
                        color: white;
                        padding: 12px 24px;
                        text-decoration: none;
                        border-radius: 4px;
                        margin: 20px 0;
                    }}
                    .footer {{
                        font-size: 12px;
                        text-align: center;
                        margin-top: 20px;
                        color: #64748b;
                    }}
                </style>
            </head>
            <body>
                <div class='header'>
                    <h1>Invitación a Encuesta</h1>
                </div>
                <div class='content'>
                    <p>Estimado/a usuario,</p>
                    <p>Le invitamos a participar en nuestra encuesta: <strong>{surveyTitle}</strong></p>
                    <p>Su opinión es muy importante para nosotros y nos ayudará a mejorar nuestros servicios.</p>
                    <div style='text-align: center;'>
                        <a href='{surveyLink}' class='button'>Responder Encuesta</a>
                    </div>
                    <p>Si el botón no funciona, puede copiar y pegar el siguiente enlace en su navegador:</p>
                    <p>{surveyLink}</p>
                    <p>Gracias por su tiempo y colaboración.</p>
                </div>
                <div class='footer'>
                    <p>Este correo ha sido enviado automáticamente, por favor no responda a este mensaje.</p>
                </div>
            </body>
            </html>";
        }
    }

    public class EmailSettings
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public bool EnableSsl { get; set; }
    }
}
