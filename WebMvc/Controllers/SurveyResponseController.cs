
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SurveyApp.Application.DTOs;
using SurveyApp.Application.Services;
using SurveyApp.WebMvc.Models;

namespace SurveyApp.WebMvc.Controllers
{
    public class SurveyResponseController : Controller
    {
        private readonly ISurveyService _surveyService;
        private readonly IKnowledgeBaseService _knowledgeBaseService;
        private readonly ILogger<SurveyResponseController> _logger;

        public SurveyResponseController(
            ISurveyService surveyService,
            IKnowledgeBaseService knowledgeBaseService,
            ILogger<SurveyResponseController> logger)
        {
            _surveyService = surveyService;
            _knowledgeBaseService = knowledgeBaseService;
            _logger = logger;
        }

        [HttpGet("respond/{id}")]
        public async Task<IActionResult> Respond(Guid id)
        {
            try
            {
                var survey = await _surveyService.GetSurveyByIdAsync(id);
                if (survey == null)
                {
                    TempData["ErrorMessage"] = "La encuesta solicitada no existe.";
                    return RedirectToAction("Index", "Home");
                }
                
                var viewModel = new SurveyResponseViewModel
                {
                    SurveyId = survey.Id,
                    Title = survey.Title,
                    Description = survey.Description,
                    Questions = survey.Questions.Select(q => new QuestionViewModel
                    {
                        Id = q.Id,
                        Title = q.Title,
                        Description = q.Description,
                        Type = q.Type,
                        Required = q.Required,
                        Options = q.Options
                    }).ToList(),
                    // Configuración UI desde el survey
                    Theme = survey.Theme,
                    BackgroundColor = survey.BackgroundColor,
                    LogoUrl = survey.LogoUrl,
                    ShowProgressBar = survey.ShowProgressBar,
                    SubmitButtonText = survey.SubmitButtonText
                };
                
                // Obtener artículos relacionados de la base de conocimientos
                var relatedArticles = await _knowledgeBaseService.GetRelatedItemsAsync(survey.Category, 3);
                viewModel.RelatedKnowledgeItems = relatedArticles.Select(a => new KnowledgeItemViewModel
                {
                    Id = a.Id,
                    Title = a.Title,
                    Content = a.Content,
                    Category = a.Category,
                    Tags = a.Tags,
                    Summary = a.Summary,
                    ImageUrl = a.ImageUrl
                }).ToList();
                
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar la encuesta para responder");
                TempData["ErrorMessage"] = "Ocurrió un error al cargar la encuesta.";
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost("respond/{id}")]
        public async Task<IActionResult> Submit(Guid id, [FromForm] SurveyResponseViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Modelo inválido en envío de respuesta de encuesta");
                TempData["ErrorMessage"] = "Por favor complete todos los campos requeridos.";
                return RedirectToAction(nameof(Respond), new { id });
            }

            try
            {
                if (model.Answers == null)
                {
                    model.Answers = new Dictionary<string, object>();
                }

                var survey = await _surveyService.GetSurveyByIdAsync(id);
                
                foreach (var question in survey.Questions.Where(q => q.Required))
                {
                    var questionIdStr = question.Id.ToString();
                    
                    if (!model.Answers.ContainsKey(questionIdStr) && 
                        !Request.Form.Keys.Any(k => k.StartsWith($"Answers[{questionIdStr}]")))
                    {
                        ModelState.AddModelError("", $"La pregunta '{question.Title}' es obligatoria.");
                        TempData["ErrorMessage"] = "Por favor complete todas las preguntas obligatorias.";
                        return RedirectToAction(nameof(Respond), new { id });
                    }
                }

                ProcessMultipleChoiceAnswers(model);

                // Capturar información del cliente y navegador
                var userAgent = Request.Headers["User-Agent"].ToString();
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var referrer = Request.Headers["Referer"].ToString();
                
                // Extraer información del dispositivo
                var deviceInfo = GetDeviceInfo(userAgent);

                var responseDto = new CreateSurveyResponseDto
                {
                    SurveyId = id,
                    RespondentName = model.RespondentName,
                    RespondentEmail = model.RespondentEmail,
                    RespondentPhone = model.RespondentPhone,
                    RespondentCompany = model.RespondentCompany,
                    Answers = model.Answers,
                    IsExistingClient = model.IsExistingClient,
                    ExistingClientId = model.ExistingClientId,
                    CompletionTime = model.CompletionTime,
                    DeviceType = deviceInfo.DeviceType,
                    Browser = deviceInfo.Browser,
                    OperatingSystem = deviceInfo.OperatingSystem,
                    Location = GetLocationFromIp(ipAddress),
                    IpAddress = ipAddress,
                    Source = referrer,
                    UserAgent = userAgent,
                    PageViews = model.PageViews > 0 ? model.PageViews : 1,
                    ReferrerUrl = referrer
                };

                var response = await _surveyService.SubmitSurveyResponseAsync(responseDto);

                // Crear el modelo para la página de agradecimiento
                var thankYouModel = new ThankYouViewModel
                {
                    SurveyTitle = survey.Title,
                    RespondentName = model.RespondentName,
                    ThankYouMessage = !string.IsNullOrEmpty(survey.ThankYouMessage) 
                        ? survey.ThankYouMessage 
                        : "¡Gracias por completar nuestra encuesta!",
                    SurveyCategory = survey.Category,
                    SubmittedAt = DateTime.UtcNow,
                    ResponseCount = await _surveyService.GetResponseCountAsync(id)
                };
                
                // Obtener encuestas relacionadas
                var relatedSurveys = await _surveyService.GetRelatedSurveysAsync(survey.Category, id, 3);
                thankYouModel.RelatedSurveys = relatedSurveys.Select(s => new SurveyListItemViewModel
                {
                    Id = s.Id,
                    Title = s.Title,
                    Description = s.Description?.Length > 100 
                        ? s.Description.Substring(0, 97) + "..." 
                        : s.Description,
                    Category = s.Category,
                    ResponseCount = s.ResponseCount,
                    ImageUrl = s.ImageUrl,
                    IsFeatured = s.IsFeatured
                }).ToList();

                return View("ThankYou", thankYouModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar respuesta de encuesta: {Message}", ex.Message);
                TempData["ErrorMessage"] = "Ocurrió un error al procesar su respuesta.";
                return RedirectToAction(nameof(Respond), new { id });
            }
        }

        private void ProcessMultipleChoiceAnswers(SurveyResponseViewModel model)
        {
            foreach (var key in Request.Form.Keys)
            {
                if (key.StartsWith("Answers[") && key.EndsWith("]") && Request.Form[key].Count > 1)
                {
                    var questionIdStr = key.Substring(8, key.Length - 9);
                    var values = Request.Form[key].ToList();
                    
                    model.Answers[questionIdStr] = values;
                }
                else if (key.StartsWith("Answers[") && key.EndsWith("]") && !model.Answers.ContainsKey(key.Substring(8, key.Length - 9)))
                {
                    var questionIdStr = key.Substring(8, key.Length - 9);
                    var value = Request.Form[key].ToString();
                    model.Answers[questionIdStr] = value;
                }
            }
        }

        private (string DeviceType, string Browser, string OperatingSystem) GetDeviceInfo(string userAgent)
        {
            string deviceType = "Desktop";
            string browser = "Unknown";
            string operatingSystem = "Unknown";
            
            // Detectar tipo de dispositivo
            if (userAgent.Contains("Mobile") || userAgent.Contains("Android") && !userAgent.Contains("Tablet"))
            {
                deviceType = "Mobile";
            }
            else if (userAgent.Contains("iPad") || userAgent.Contains("Tablet"))
            {
                deviceType = "Tablet";
            }
            
            // Detectar navegador
            if (userAgent.Contains("Chrome") && !userAgent.Contains("Edg"))
            {
                browser = "Chrome";
            }
            else if (userAgent.Contains("Firefox"))
            {
                browser = "Firefox";
            }
            else if (userAgent.Contains("Safari") && !userAgent.Contains("Chrome"))
            {
                browser = "Safari";
            }
            else if (userAgent.Contains("Edge") || userAgent.Contains("Edg"))
            {
                browser = "Edge";
            }
            else if (userAgent.Contains("MSIE") || userAgent.Contains("Trident"))
            {
                browser = "Internet Explorer";
            }
            
            // Detectar sistema operativo
            if (userAgent.Contains("Windows"))
            {
                operatingSystem = "Windows";
            }
            else if (userAgent.Contains("Mac"))
            {
                operatingSystem = "macOS";
            }
            else if (userAgent.Contains("Linux") || userAgent.Contains("X11"))
            {
                operatingSystem = "Linux";
            }
            else if (userAgent.Contains("Android"))
            {
                operatingSystem = "Android";
            }
            else if (userAgent.Contains("iPhone") || userAgent.Contains("iPad") || userAgent.Contains("iPod"))
            {
                operatingSystem = "iOS";
            }
            
            return (deviceType, browser, operatingSystem);
        }
        
        private string GetLocationFromIp(string ipAddress)
        {
            // En una aplicación real, se usaría un servicio de geolocalización
            // Para este ejemplo, devolvemos un valor predeterminado
            return "Unknown";
        }
    }
}
