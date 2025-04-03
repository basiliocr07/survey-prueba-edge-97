
using Microsoft.AspNetCore.Mvc;
using SurveyApp.Application.Interfaces;
using SurveyApp.Domain.Models;
using SurveyApp.Web.Models;
using SurveyApp.Domain.Repositories;
using SurveyApp.Domain.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using MediatR;
using SurveyApp.Application.Customers.Queries.GetCustomerEmails;

namespace SurveyApp.Web.Controllers
{
    public class EmailSettingsController : Controller
    {
        private readonly ISurveyService _surveyService;
        private readonly ICustomerRepository _customerRepository;
        private readonly IEmailService _emailService;
        private readonly IMediator _mediator;

        public EmailSettingsController(
            ISurveyService surveyService, 
            ICustomerRepository customerRepository,
            IEmailService emailService,
            IMediator mediator)
        {
            _surveyService = surveyService;
            _customerRepository = customerRepository;
            _emailService = emailService;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? surveyId = null)
        {
            var surveys = await _surveyService.GetAllSurveysAsync();
            var customers = await _customerRepository.GetAllCustomersAsync();
            
            // Obtener tipos de clientes únicos para el filtro
            var customerTypes = customers
                .Select(c => c.CustomerType)
                .Where(t => !string.IsNullOrEmpty(t))
                .Distinct()
                .ToList();
                
            var config = surveyId.HasValue 
                ? await GetSurveyDeliveryConfig(surveyId.Value) 
                : GetGlobalDeliveryConfig();
                
            // Si está configurado para incluir todos los clientes, cargar emails automáticamente
            if (config.IncludeAllCustomers)
            {
                await config.LoadCustomerEmails(_customerRepository);
            }
            
            var model = new EmailSettingsViewModel
            {
                Surveys = surveys.Select(s => new SurveyListItemViewModel
                {
                    Id = s.Id,
                    Title = s.Title,
                    CreatedAt = s.CreatedAt,
                    HasCustomDeliveryConfig = s.DeliveryConfig != null
                }).ToList(),
                SelectedSurveyId = surveyId,
                DeliveryConfig = config,
                Customers = customers.Select(c => new CustomerViewModel
                {
                    Id = c.Id,
                    Name = c.ContactName,
                    Email = c.ContactEmail
                }).ToList(),
                CustomerTypes = customerTypes
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetSurveyConfig(int surveyId)
        {
            var config = await GetSurveyDeliveryConfig(surveyId);
            return Json(config);
        }

        [HttpPost]
        public async Task<IActionResult> SaveGlobalConfig([FromBody] DeliveryConfigViewModel config)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            // Si se configura para incluir todos los clientes, cargar los emails
            if (config.IncludeAllCustomers)
            {
                await config.LoadCustomerEmails(_customerRepository);
            }

            TempData["GlobalEmailConfig"] = JsonSerializer.Serialize(config);
            TempData.Keep("GlobalEmailConfig");

            return Json(new { success = true, message = "Configuración global guardada exitosamente" });
        }

        [HttpPost]
        public async Task<IActionResult> SaveSurveyConfig(int surveyId, [FromBody] DeliveryConfigViewModel config)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var survey = await _surveyService.GetSurveyByIdAsync(surveyId);
            if (survey == null)
            {
                return NotFound();
            }
            
            // Si se configura para incluir todos los clientes, cargar los emails
            if (config.IncludeAllCustomers)
            {
                await config.LoadCustomerEmails(_customerRepository);
            }

            var deliveryConfig = config.ToDeliveryConfiguration();
            bool success = await _surveyService.UpdateSurveyDeliveryConfigAsync(surveyId, deliveryConfig);

            return Json(new { success, message = success ? "Configuración guardada exitosamente" : "Error al guardar la configuración" });
        }

        [HttpPost]
        public async Task<IActionResult> SendSurveyEmails(int surveyId, [FromBody] List<string> emailAddresses)
        {
            if (surveyId <= 0 || emailAddresses == null || !emailAddresses.Any())
            {
                return BadRequest(new { success = false, message = "Parámetros inválidos" });
            }

            try
            {
                var success = await _surveyService.SendSurveyEmailsAsync(surveyId, emailAddresses);
                return Json(new { 
                    success, 
                    message = success ? 
                        $"Correos enviados exitosamente a {emailAddresses.Count} destinatarios" : 
                        "Error al enviar los correos"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomers(string search = "")
        {
            var allCustomers = await _customerRepository.GetAllCustomersAsync();
            var customers = allCustomers.Select(c => new CustomerViewModel
            {
                Id = c.Id,
                Name = c.ContactName,
                Email = c.ContactEmail,
                CustomerType = c.CustomerType
            }).ToList();
            
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                customers = customers.Where(c => 
                    c.Name.ToLower().Contains(search) || 
                    c.Email.ToLower().Contains(search)
                ).ToList();
            }
            
            return Json(customers);
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomerEmails(string customerType = null)
        {
            try
            {
                var query = new GetCustomerEmailsQuery { CustomerType = customerType };
                var emails = await _mediator.Send(query);
                return Json(emails);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error obteniendo emails: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> TestEmailConnection()
        {
            try
            {
                var result = await _emailService.TestConnectionAsync();
                return Json(new { 
                    success = result.Success, 
                    message = result.Success ? "Conexión SMTP exitosa" : result.ErrorMessage 
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
        
        [HttpGet]
        public async Task<IActionResult> CheckEmails()
        {
            try
            {
                var result = await _emailService.CheckEmailsAsync();
                
                if (result.Success && result.Messages.Count > 0)
                {
                    foreach (var message in result.Messages)
                    {
                        await _emailService.SendNotificationAsync(message);
                    }
                }
                
                return Json(new { 
                    success = result.Success, 
                    count = result.Messages.Count,
                    messages = result.Messages.Select(m => new {
                        from = m.From,
                        subject = m.Subject,
                        date = m.Date,
                        hasAttachments = m.Attachments.Count > 0
                    }),
                    message = result.Success ? 
                        $"Se encontraron {result.Messages.Count} mensajes" : 
                        result.ErrorMessage
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }
        
        [HttpGet]
        public async Task<IActionResult> SendTestEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return BadRequest(new { success = false, message = "Email inválido" });
            }

            try
            {
                string subject = "Prueba de conexión SMTP";
                string htmlContent = @"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body { font-family: Arial, sans-serif; line-height: 1.6; }
        .container { max-width: 600px; margin: 0 auto; padding: 20px; }
        .header { background-color: #4F46E5; color: white; padding: 10px 20px; text-align: center; }
        .content { padding: 20px; border: 1px solid #ddd; }
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h2>Correo de prueba</h2>
        </div>
        <div class='content'>
            <p>Este es un correo de prueba enviado desde tu sistema de encuestas.</p>
            <p>La configuración SMTP está funcionando correctamente.</p>
            <p>Fecha y hora: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + @"</p>
        </div>
    </div>
</body>
</html>";

                var success = await _emailService.SendEmailAsync(email, subject, htmlContent);
                
                return Json(new { 
                    success, 
                    message = success ? "Correo de prueba enviado correctamente" : "Error al enviar el correo de prueba"
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        private DeliveryConfigViewModel GetGlobalDeliveryConfig()
        {
            if (TempData.TryGetValue("GlobalEmailConfig", out var serializedConfig))
            {
                try
                {
                    var config = JsonSerializer.Deserialize<DeliveryConfigViewModel>(serializedConfig.ToString());
                    TempData.Keep("GlobalEmailConfig");
                    return config;
                }
                catch
                {
                }
            }

            return new DeliveryConfigViewModel
            {
                Type = "manual",
                EmailAddresses = new List<string>(),
                Schedule = new ScheduleSettingsViewModel 
                { 
                    Frequency = "weekly", 
                    DayOfWeek = 1, 
                    Time = "09:00" 
                },
                Trigger = new TriggerSettingsViewModel
                {
                    Type = "ticket-closed",
                    DelayHours = 24,
                    SendAutomatically = true
                }
            };
        }

        private async Task<DeliveryConfigViewModel> GetSurveyDeliveryConfig(int surveyId)
        {
            var survey = await _surveyService.GetSurveyByIdAsync(surveyId);
            if (survey == null || survey.DeliveryConfig == null)
            {
                return GetGlobalDeliveryConfig();
            }

            return DeliveryConfigViewModel.FromDeliveryConfiguration(survey.DeliveryConfig);
        }

        private DeliveryConfiguration MapViewModelToDeliveryConfig(DeliveryConfigViewModel viewModel)
        {
            return viewModel.ToDeliveryConfiguration();
        }
    }
}
