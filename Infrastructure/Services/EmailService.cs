
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SurveyApp.Application.Ports;

namespace SurveyApp.Infrastructure.Services
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
    }

    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;

        public EmailService(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string to, string subject, string htmlMessage)
        {
            try
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(to);

                using (var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);
                    
                    await client.SendMailAsync(mailMessage);
                    Console.WriteLine($"Email enviado exitosamente a {to}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al enviar email: {ex.Message}");
                throw;
            }
        }

        public async Task SendSurveyInvitationAsync(string to, string surveyTitle, string surveyLink)
        {
            string subject = $"Invitación para completar encuesta: {surveyTitle}";
            string htmlMessage = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                    <h2>Te invitamos a completar una encuesta</h2>
                    <p>Hemos preparado una encuesta llamada <strong>{surveyTitle}</strong> y nos gustaría contar con tu opinión.</p>
                    <p>Para acceder a la encuesta, por favor haz clic en el siguiente botón:</p>
                    <div style='text-align: center; margin: 25px 0;'>
                        <a href='{surveyLink}' style='background-color: #3b82f6; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; font-weight: bold;'>
                            Completar Encuesta
                        </a>
                    </div>
                    <p>Si el botón no funciona, puedes copiar y pegar el siguiente enlace en tu navegador:</p>
                    <p style='word-break: break-all;'>{surveyLink}</p>
                    <p>Gracias por tu tiempo y colaboración.</p>
                </div>";

            await SendEmailAsync(to, subject, htmlMessage);
        }
        
        public async Task<bool> TestEmailServiceAsync(string toEmail)
        {
            try
            {
                string subject = "Prueba del servicio de correo - Sistema de Encuestas";
                string htmlMessage = @"
                    <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                        <h2>Prueba de envío de correo</h2>
                        <p>Este es un correo de prueba del sistema de encuestas.</p>
                        <p>Si has recibido este correo, la configuración de envío de correos está funcionando correctamente.</p>
                        <hr />
                        <p style='color: #888; font-size: 12px;'>Este es un correo automático, por favor no responda a este mensaje.</p>
                    </div>";
                
                await SendEmailAsync(toEmail, subject, htmlMessage);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en prueba de envío de email: {ex.Message}");
                return false;
            }
        }
    }
}
