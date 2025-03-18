
using System;
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
            // En un entorno de producción, aquí se implementaría el código
            // para enviar correos electrónicos utilizando la información
            // de configuración proporcionada en _emailSettings.
            
            // Por ahora, solo registramos la información en consola
            Console.WriteLine($"Email enviado a {to}");
            Console.WriteLine($"Asunto: {subject}");
            Console.WriteLine($"Contenido: {htmlMessage}");
            
            await Task.CompletedTask;
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
    }
}
