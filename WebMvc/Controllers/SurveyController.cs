using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SurveyApp.Application.DTOs;
using SurveyApp.Application.Services;
using SurveyApp.WebMvc.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyApp.WebMvc.Controllers
{
    public class SurveyController : Controller
    {
        private readonly ISurveyService _surveyService;
        private readonly ILogger<SurveyController> _logger;

        public SurveyController(ISurveyService surveyService, ILogger<SurveyController> logger)
        {
            _surveyService = surveyService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var surveys = await _surveyService.GetAllSurveysAsync();
            return View(surveys);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var survey = await _surveyService.GetSurveyByIdAsync(id);
                if (survey == null)
                {
                    TempData["ErrorMessage"] = "Encuesta no encontrada.";
                    return RedirectToAction("Index");
                }

                ViewData["Survey"] = survey;
                return View(survey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener detalles de la encuesta: {Message}", ex.Message);
                TempData["ErrorMessage"] = "Error al obtener detalles de la encuesta: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSurveyDto surveyDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _surveyService.CreateSurveyAsync(surveyDto);
                    TempData["SuccessMessage"] = "Encuesta creada correctamente.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al crear la encuesta: {Message}", ex.Message);
                    TempData["ErrorMessage"] = "Error al crear la encuesta: " + ex.Message;
                    return View(surveyDto);
                }
            }
            return View(surveyDto);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var survey = await _surveyService.GetSurveyByIdAsync(id);
            if (survey == null)
            {
                TempData["ErrorMessage"] = "Encuesta no encontrada.";
                return RedirectToAction("Index");
            }
            return View(survey);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Guid id, UpdateSurveyDto surveyDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _surveyService.UpdateSurveyAsync(id, surveyDto);
                    TempData["SuccessMessage"] = "Encuesta actualizada correctamente.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al actualizar la encuesta: {Message}", ex.Message);
                    TempData["ErrorMessage"] = "Error al actualizar la encuesta: " + ex.Message;
                    return View(surveyDto);
                }
            }
            return View(surveyDto);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            var survey = await _surveyService.GetSurveyByIdAsync(id);
            if (survey == null)
            {
                TempData["ErrorMessage"] = "Encuesta no encontrada.";
                return RedirectToAction("Index");
            }
            return View(survey);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                await _surveyService.DeleteSurveyAsync(id);
                TempData["SuccessMessage"] = "Encuesta eliminada correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la encuesta: {Message}", ex.Message);
                TempData["ErrorMessage"] = "Error al eliminar la encuesta: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // Método para enviar encuestas por email a un destinatario específico
        [HttpPost]
        public async Task<IActionResult> SendSurveyEmail(Guid id, string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "El email del destinatario es requerido.";
                return RedirectToAction("Details", new { id });
            }

            try
            {
                var surveyLink = $"{Request.Scheme}://{Request.Host}/survey/{id}";
                var survey = await _surveyService.GetSurveyByIdAsync(id);
        
                if (survey == null)
                {
                    TempData["ErrorMessage"] = "No se encontró la encuesta especificada.";
                    return RedirectToAction("Index");
                }
        
                // Enviar la encuesta al email proporcionado
                await _surveyService.SendSurveyEmailAsync(email, survey.Title, surveyLink);
        
                TempData["SuccessMessage"] = $"La encuesta ha sido enviada correctamente a {email}.";
                return RedirectToAction("Details", new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar la encuesta por email: {Message}", ex.Message);
                TempData["ErrorMessage"] = "Ocurrió un error al enviar la encuesta por email: " + ex.Message;
                return RedirectToAction("Details", new { id });
            }
        }

        // Método para enviar encuestas a todos los destinatarios configurados
        [HttpPost]
        public async Task<IActionResult> SendSurveyToAllRecipients(Guid id)
        {
            try
            {
                await _surveyService.SendSurveyEmailsAsync(id);
        
                TempData["SuccessMessage"] = "La encuesta ha sido enviada a todos los destinatarios configurados.";
                return RedirectToAction("Details", new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar la encuesta a todos los destinatarios: {Message}", ex.Message);
                TempData["ErrorMessage"] = "Ocurrió un error al enviar la encuesta a los destinatarios: " + ex.Message;
                return RedirectToAction("Details", new { id });
            }
        }

        // Método para configurar el envío automático
        [HttpPost]
        public async Task<IActionResult> ConfigureAutomaticDelivery(Guid id, DeliveryConfigViewModel deliveryConfig)
        {
            try
            {
                var survey = await _surveyService.GetSurveyByIdAsync(id);
                var updateDto = new UpdateSurveyDto
                {
                    Title = survey.Title,
                    Description = survey.Description,
                    Questions = survey.Questions.Select(q => new CreateQuestionDto
                    {
                        Title = q.Title,
                        Description = q.Description,
                        Type = q.Type,
                        Required = q.Required,
                        Options = q.Options,
                        Settings = q.Settings
                    }).ToList(),
                    DeliveryConfig = new DeliveryConfigDto
                    {
                        Type = deliveryConfig.Type,
                        EmailAddresses = deliveryConfig.EmailAddresses,
                        Schedule = new ScheduleDto
                        {
                            Frequency = deliveryConfig.Schedule.Frequency,
                            DayOfMonth = deliveryConfig.Schedule.DayOfMonth,
                            DayOfWeek = deliveryConfig.Schedule.DayOfWeek,
                            Time = deliveryConfig.Schedule.Time,
                            StartDate = deliveryConfig.Schedule.StartDate
                        },
                        Trigger = new TriggerDto
                        {
                            Type = deliveryConfig.Trigger.Type,
                            DelayHours = deliveryConfig.Trigger.DelayHours,
                            SendAutomatically = deliveryConfig.Trigger.SendAutomatically
                        }
                    }
                };
        
                await _surveyService.UpdateSurveyAsync(id, updateDto);
        
                TempData["SuccessMessage"] = "La configuración de envío automático ha sido actualizada.";
                return RedirectToAction("Details", new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al configurar el envío automático: {Message}", ex.Message);
                TempData["ErrorMessage"] = "Ocurrió un error al configurar el envío automático: " + ex.Message;
                return RedirectToAction("Details", new { id });
            }
        }

        // Nuevo método para enviar prueba de email específico
        [HttpPost]
        public async Task<IActionResult> SendTestEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "El email de prueba es requerido.";
                return RedirectToAction("Index");
            }
    
            try
            {
                // Enviamos un email de prueba a ubcruz2@gmail.com usando una encuesta existente
                var surveys = await _surveyService.GetAllSurveysAsync();
                var survey = surveys.FirstOrDefault();
        
                if (survey == null)
                {
                    TempData["ErrorMessage"] = "No se encontró ninguna encuesta para la prueba.";
                    return RedirectToAction("Index");
                }
        
                var surveyLink = $"{Request.Scheme}://{Request.Host}/survey/{survey.Id}";
                await _surveyService.SendSurveyEmailAsync(email, survey.Title, surveyLink);
        
                TempData["SuccessMessage"] = $"Se ha enviado un correo de prueba a {email} exitosamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar el email de prueba: {Message}", ex.Message);
                TempData["ErrorMessage"] = "Error al enviar el email de prueba: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
    }
}
