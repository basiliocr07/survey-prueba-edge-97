
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
            _logger = logger;
            _emailSettings = emailSettings.Value;
        }

        public async Task SendSurveyInvitationAsync(string toEmail, string surveyTitle, string surveyLink)
        {
            try
            {
                // Crear el mensaje con los datos proporcionados
                var message = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                    Subject = $"Invitación para responder encuesta: {surveyTitle}",
                    Body = GetEmailTemplate(toEmail, surveyTitle, surveyLink),
                    IsBodyHtml = true
                };
                
                message.To.Add(new MailAddress(toEmail));

                // Configurar el cliente SMTP
                using (var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort))
                {
                    client.EnableSsl = _emailSettings.EnableSsl;
                    client.Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password);
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;

                    // Enviar el correo electrónico
                    await client.SendMailAsync(message);
                    _logger.LogInformation($"Correo enviado exitosamente a {toEmail} para la encuesta '{surveyTitle}'");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al enviar correo a {toEmail}: {ex.Message}");
                throw;
            }
        }

        private string GetEmailTemplate(string toEmail, string surveyTitle, string surveyLink)
        {
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
