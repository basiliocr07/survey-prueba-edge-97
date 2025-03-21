using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SurveyApp.Application.DTOs;
using SurveyApp.Application.Ports;
using SurveyApp.Application.Services;
using SurveyApp.WebMvc.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.Json;

namespace SurveyApp.WebMvc.Controllers
{
    public class SurveyController : Controller
    {
        private readonly ISurveyService _surveyService;
        private readonly IEmailService _emailService;
        private readonly ILogger<SurveyController> _logger;

        public SurveyController(
            ISurveyService surveyService, 
            IEmailService emailService,
            ILogger<SurveyController> logger)
        {
            _surveyService = surveyService;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var surveys = await _surveyService.GetAllSurveysAsync();
                var viewModels = surveys.Select(MapToSurveyListItemViewModel).ToList();

                // Set view bag properties for authentication status
                ViewBag.IsAuthenticated = User.Identity.IsAuthenticated;
                ViewBag.UserRole = User.IsInRole("Admin") ? "Admin" : "Client";
                ViewBag.Username = User.Identity.Name;

                return View(viewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving surveys: {Message}", ex.Message);
                
                // Create detailed error model
                var errorViewModel = ErrorViewModel.CreateDetailedError(ex, Activity.Current?.Id?.ToString(), 
                    "Error al recuperar las encuestas");
                errorViewModel.ControllerName = "Survey";
                errorViewModel.ActionName = "Index";
                errorViewModel.HttpMethod = Request.Method;
                errorViewModel.QueryString = Request.QueryString.ToString();
                errorViewModel.IsAuthenticated = User.Identity.IsAuthenticated;
                errorViewModel.Username = User.Identity.Name ?? string.Empty;
                errorViewModel.UserRole = User.IsInRole("Admin") ? "Admin" : "Client";
                
                return View("Error", errorViewModel);
            }
        }

        private SurveyListItemViewModel MapToSurveyListItemViewModel(SurveyDto dto)
        {
            return new SurveyListItemViewModel
            {
                Id = dto.Id,
                Title = dto.Title,
                Description = dto.Description,
                CreatedAt = dto.CreatedAt,
                ResponseCount = dto.Responses,
                CompletionRate = dto.CompletionRate,
                Status = dto.Status ?? "Active",
                DeliveryType = dto.DeliveryConfig?.Type ?? "Manual",
                QuestionCount = dto.Questions?.Count ?? 0,
                Responses = dto.Responses
            };
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
                
                var errorViewModel = ErrorViewModel.CreateDetailedError(ex, Activity.Current?.Id?.ToString(),
                    "Error al obtener detalles de la encuesta");
                errorViewModel.ControllerName = "Survey";
                errorViewModel.ActionName = "Details";
                errorViewModel.HttpMethod = Request.Method;
                errorViewModel.QueryString = Request.QueryString.ToString();
                errorViewModel.IsAuthenticated = User.Identity.IsAuthenticated;
                errorViewModel.Username = User.Identity.Name ?? string.Empty;
                errorViewModel.UserRole = User.IsInRole("Admin") ? "Admin" : "Client";
                
                return View("Error", errorViewModel);
            }
        }

        public IActionResult Create()
        {
            try
            {
                // Inicializar un nuevo modelo para el formulario
                var model = new SurveyCreateViewModel
                {
                    Id = Guid.NewGuid(),
                    Questions = new List<QuestionViewModel>
                    {
                        new QuestionViewModel
                        {
                            Id = Guid.NewGuid(),
                            Type = "single-choice",
                            Title = "¿Cómo calificaría nuestro servicio?",
                            Required = true,
                            Options = new List<string> { "Excelente", "Bueno", "Regular", "Malo" }
                        }
                    }
                };
                
                // Log para ayudar a depurar
                _logger.LogInformation("Accediendo a la vista de creación de encuestas");
                
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al preparar la vista de creación: {Message}", ex.Message);
                
                var errorViewModel = ErrorViewModel.CreateDetailedError(ex, Activity.Current?.Id?.ToString(),
                    "Error al preparar la vista de creación de encuestas");
                errorViewModel.ControllerName = "Survey";
                errorViewModel.ActionName = "Create";
                errorViewModel.HttpMethod = Request.Method;
                errorViewModel.QueryString = Request.QueryString.ToString();
                errorViewModel.IsAuthenticated = User.Identity?.IsAuthenticated ?? false;
                errorViewModel.Username = User.Identity?.Name ?? string.Empty;
                errorViewModel.UserRole = User.IsInRole("Admin") ? "Admin" : "Client";
                
                return View("Error", errorViewModel);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SurveyCreateViewModel viewModel)
        {
            _logger.LogInformation("Procesando formulario de creación de encuesta: {Title}", viewModel?.Title);
            
            // Capture form data for debugging
            var formDataSummary = new StringBuilder();
            foreach (var key in Request.Form.Keys)
            {
                formDataSummary.AppendLine($"{key}: {Request.Form[key]}");
            }
            
            _logger.LogInformation("Form data: {FormData}", formDataSummary.ToString());
            
            // Debug log for incoming view model
            _logger.LogInformation("ViewModel received: {@ViewModel}", 
                JsonSerializer.Serialize(viewModel, new JsonSerializerOptions { WriteIndented = true }));
            
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("ModelState is invalid. Errors follow:");
                    
                    var errorViewModel = new ErrorViewModel
                    {
                        Message = "El formulario contiene errores de validación",
                        ControllerName = "Survey",
                        ActionName = "Create",
                        HttpMethod = Request.Method,
                        ErrorTimestamp = DateTime.UtcNow,
                        FormDataSummary = formDataSummary.ToString(),
                        ErrorType = "ValidationError"
                    };
                    
                    foreach (var modelState in ModelState)
                    {
                        foreach (var error in modelState.Value.Errors)
                        {
                            errorViewModel.AddValidationError(modelState.Key, error.ErrorMessage);
                            _logger.LogWarning("Error de validación: {Field} - {ErrorMessage}", 
                                modelState.Key, error.ErrorMessage);
                        }
                    }
                    
                    return View("Error", errorViewModel);
                }
                
                // Check if Questions collection is null or empty
                if (viewModel.Questions == null || !viewModel.Questions.Any())
                {
                    _logger.LogWarning("Questions collection is null or empty");
                    
                    var errorViewModel = new ErrorViewModel
                    {
                        Message = "La encuesta debe contener al menos una pregunta",
                        ControllerName = "Survey",
                        ActionName = "Create",
                        HttpMethod = Request.Method,
                        ErrorTimestamp = DateTime.UtcNow,
                        FormDataSummary = formDataSummary.ToString(),
                        ErrorType = "ValidationError"
                    };
                    
                    errorViewModel.AddValidationError("Questions", "La encuesta debe contener al menos una pregunta");
                    return View("Error", errorViewModel);
                }
                
                // Mapear el viewModel a DTO
                _logger.LogInformation("Mapeando ViewModel a DTO con {QuestionCount} preguntas", 
                    viewModel.Questions?.Count ?? 0);
                
                var createSurveyDto = new CreateSurveyDto
                {
                    Title = viewModel.Title,
                    Description = viewModel.Description,
                    Questions = viewModel.Questions?.Select(q => new CreateQuestionDto
                    {
                        Title = q.Title,
                        Description = q.Description,
                        Type = q.Type,
                        Required = q.Required,
                        Options = q.Options,
                        Settings = q.Settings != null ? new QuestionSettingsDto
                        {
                            MinValue = q.Settings.MinValue,
                            MaxValue = q.Settings.MaxValue,
                            LowLabel = q.Settings.LowLabel,
                            MiddleLabel = q.Settings.MiddleLabel,
                            HighLabel = q.Settings.HighLabel
                        } : null
                    }).ToList() ?? new List<CreateQuestionDto>(),
                    DeliveryConfig = new DeliveryConfigDto
                    {
                        Type = viewModel.EnableEmailDelivery ? "Email" : "Manual",
                        EmailAddresses = viewModel.DeliveryConfig?.EmailAddresses ?? new List<string>(),
                        Schedule = viewModel.DeliveryConfig?.Schedule != null ? new ScheduleDto
                        {
                            Frequency = viewModel.DeliveryConfig.Schedule.Frequency,
                            DayOfMonth = viewModel.DeliveryConfig.Schedule.DayOfMonth,
                            DayOfWeek = viewModel.DeliveryConfig.Schedule.DayOfWeek,
                            Time = viewModel.DeliveryConfig.Schedule.Time,
                            StartDate = viewModel.DeliveryConfig.Schedule.StartDate
                        } : null,
                        Trigger = viewModel.DeliveryConfig?.Trigger != null ? new TriggerDto
                        {
                            Type = viewModel.DeliveryConfig.Trigger.Type,
                            DelayHours = viewModel.DeliveryConfig.Trigger.DelayHours,
                            SendAutomatically = viewModel.DeliveryConfig.Trigger.SendAutomatically
                        } : null
                    }
                };

                _logger.LogInformation("DTO preparado para crear encuesta: {@SurveyDto}", 
                    JsonSerializer.Serialize(createSurveyDto, new JsonSerializerOptions { WriteIndented = true }));
                
                try {
                    var result = await _surveyService.CreateSurveyAsync(createSurveyDto);
                    _logger.LogInformation("Encuesta creada exitosamente con ID: {SurveyId}", result.Id);
                }
                catch (Exception ex) {
                    _logger.LogError(ex, "Error específico al llamar a CreateSurveyAsync: {Message}", ex.Message);
                    throw; // Re-throw to be caught by the outer catch
                }
                
                TempData["SuccessMessage"] = "Encuesta creada correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la encuesta: {Message}, StackTrace: {StackTrace}", 
                    ex.Message, ex.StackTrace);
                
                // Get inner exception details if available
                var innerExMsg = ex.InnerException != null ? 
                    $"Inner exception: {ex.InnerException.Message}" : "No inner exception";
                _logger.LogError(innerExMsg);
                
                var errorViewModel = ErrorViewModel.CreateDetailedError(ex, Activity.Current?.Id?.ToString(),
                    "Error al crear la encuesta");
                errorViewModel.ControllerName = "Survey";
                errorViewModel.ActionName = "Create";
                errorViewModel.HttpMethod = Request.Method;
                errorViewModel.QueryString = Request.QueryString.ToString();
                errorViewModel.IsAuthenticated = User.Identity?.IsAuthenticated ?? false;
                errorViewModel.Username = User.Identity?.Name ?? string.Empty;
                errorViewModel.UserRole = User.IsInRole("Admin") ? "Admin" : "Client";
                errorViewModel.FormDataSummary = formDataSummary.ToString();
                errorViewModel.ErrorType = ex.GetType().Name;
                errorViewModel.IsDatabaseError = ex.Message.Contains("database") || 
                                               (ex.InnerException?.Message?.Contains("database") ?? false);
                
                return View("Error", errorViewModel);
            }
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
                    
                    var errorViewModel = ErrorViewModel.CreateDetailedError(ex, Activity.Current?.Id?.ToString(),
                        "Error al actualizar la encuesta");
                    errorViewModel.ControllerName = "Survey";
                    errorViewModel.ActionName = "Edit";
                    errorViewModel.HttpMethod = Request.Method;
                    errorViewModel.QueryString = Request.QueryString.ToString();
                    errorViewModel.IsAuthenticated = User.Identity.IsAuthenticated;
                    errorViewModel.Username = User.Identity.Name ?? string.Empty;
                    errorViewModel.UserRole = User.IsInRole("Admin") ? "Admin" : "Client";
                    
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
                
                var errorViewModel = ErrorViewModel.CreateDetailedError(ex, Activity.Current?.Id?.ToString(),
                    "Error al eliminar la encuesta");
                errorViewModel.ControllerName = "Survey";
                errorViewModel.ActionName = "Delete";
                errorViewModel.HttpMethod = Request.Method;
                errorViewModel.QueryString = Request.QueryString.ToString();
                errorViewModel.IsAuthenticated = User.Identity.IsAuthenticated;
                errorViewModel.Username = User.Identity.Name ?? string.Empty;
                errorViewModel.UserRole = User.IsInRole("Admin") ? "Admin" : "Client";
                
                return View("Error", errorViewModel);
            }
        }

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
        
                _logger.LogInformation($"Enviando encuesta '{survey.Title}' a {email} con enlace {surveyLink}");
                
                await _surveyService.SendSurveyEmailAsync(email, survey.Title, surveyLink);
        
                TempData["SuccessMessage"] = $"La encuesta ha sido enviada correctamente a {email}.";
                return RedirectToAction("Details", new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar la encuesta por email: {Message}", ex.Message);
                
                var errorViewModel = ErrorViewModel.CreateDetailedError(ex, Activity.Current?.Id?.ToString(),
                    "Error al enviar la encuesta por email");
                errorViewModel.ControllerName = "Survey";
                errorViewModel.ActionName = "SendSurveyEmail";
                errorViewModel.HttpMethod = Request.Method;
                errorViewModel.QueryString = Request.QueryString.ToString();
                errorViewModel.IsAuthenticated = User.Identity.IsAuthenticated;
                errorViewModel.Username = User.Identity.Name ?? string.Empty;
                errorViewModel.UserRole = User.IsInRole("Admin") ? "Admin" : "Client";
                
                return View("Error", errorViewModel);
            }
        }

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
                
                var errorViewModel = ErrorViewModel.CreateDetailedError(ex, Activity.Current?.Id?.ToString(),
                    "Error al enviar la encuesta a todos los destinatarios");
                errorViewModel.ControllerName = "Survey";
                errorViewModel.ActionName = "SendSurveyToAllRecipients";
                errorViewModel.HttpMethod = Request.Method;
                errorViewModel.QueryString = Request.QueryString.ToString();
                errorViewModel.IsAuthenticated = User.Identity.IsAuthenticated;
                errorViewModel.Username = User.Identity.Name ?? string.Empty;
                errorViewModel.UserRole = User.IsInRole("Admin") ? "Admin" : "Client";
                
                return View("Error", errorViewModel);
            }
        }

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
                        Schedule = ConvertToScheduleDto(deliveryConfig.Schedule),
                        Trigger = ConvertToTriggerDto(deliveryConfig.Trigger)
                    }
                };
        
                await _surveyService.UpdateSurveyAsync(id, updateDto);
        
                TempData["SuccessMessage"] = "La configuración de envío automático ha sido actualizada.";
                return RedirectToAction("Details", new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al configurar el envío automático: {Message}", ex.Message);
                
                var errorViewModel = ErrorViewModel.CreateDetailedError(ex, Activity.Current?.Id?.ToString(),
                    "Error al configurar el envío automático");
                errorViewModel.ControllerName = "Survey";
                errorViewModel.ActionName = "ConfigureAutomaticDelivery";
                errorViewModel.HttpMethod = Request.Method;
                errorViewModel.QueryString = Request.QueryString.ToString();
                errorViewModel.IsAuthenticated = User.Identity.IsAuthenticated;
                errorViewModel.Username = User.Identity.Name ?? string.Empty;
                errorViewModel.UserRole = User.IsInRole("Admin") ? "Admin" : "Client";
                
                return View("Error", errorViewModel);
            }
        }

        [HttpPost]
        public async Task<IActionResult> SendTestEmail(string email = "ubcruz2@gmail.com")
        {
            try
            {
                _logger.LogInformation($"Iniciando prueba de envío de email a {email}");
                
                var testResult = await _emailService.TestEmailServiceAsync(email);
                
                if (!testResult)
                {
                    _logger.LogError($"La prueba directa del servicio de email falló para {email}");
                    TempData["ErrorMessage"] = $"Prueba del servicio de email falló para {email}.";
                    return RedirectToAction("Index");
                }
                
                var surveys = await _surveyService.GetAllSurveysAsync();
                var survey = surveys.FirstOrDefault();
        
                if (survey == null)
                {
                    _logger.LogWarning("No se encontró ninguna encuesta para la prueba");
                    TempData["SuccessMessage"] = $"Se envió un correo de prueba básico a {email} exitosamente.";
                    return RedirectToAction("Index");
                }
        
                var surveyLink = $"{Request.Scheme}://{Request.Host}/survey/{survey.Id}";
                await _surveyService.SendSurveyEmailAsync(email, survey.Title, surveyLink);
        
                TempData["SuccessMessage"] = $"Se ha enviado un correo de prueba a {email} exitosamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al enviar el email de prueba a {email}: {ex.Message}");
                
                var errorViewModel = ErrorViewModel.CreateDetailedError(ex, Activity.Current?.Id?.ToString(),
                    "Error al enviar el email de prueba");
                errorViewModel.ControllerName = "Survey";
                errorViewModel.ActionName = "SendTestEmail";
                errorViewModel.HttpMethod = Request.Method;
                errorViewModel.QueryString = Request.QueryString.ToString();
                errorViewModel.IsAuthenticated = User.Identity.IsAuthenticated;
                errorViewModel.Username = User.Identity.Name ?? string.Empty;
                errorViewModel.UserRole = User.IsInRole("Admin") ? "Admin" : "Client";
                
                return View("Error", errorViewModel);
            }
        }

        private ScheduleDto ConvertToScheduleDto(ScheduleViewModel schedule)
        {
            if (schedule == null)
                return null;
                
            return new ScheduleDto
            {
                Frequency = schedule.Frequency,
                DayOfMonth = schedule.DayOfMonth,
                DayOfWeek = schedule.DayOfWeek,
                Time = schedule.Time,
                StartDate = schedule.StartDate
            };
        }
        
        private TriggerDto ConvertToTriggerDto(TriggerViewModel trigger)
        {
            if (trigger == null)
                return null;
                
            return new TriggerDto
            {
                Type = trigger.Type,
                DelayHours = trigger.DelayHours,
                SendAutomatically = trigger.SendAutomatically
            };
        }
    }
}
