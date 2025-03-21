
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
                    }).ToList()
                };
                
                // Get related knowledge base articles that might help the respondent
                var relatedArticles = await _knowledgeBaseService.GetRelatedItemsAsync(survey.Category, 3);
                viewModel.RelatedKnowledgeItems = relatedArticles.Select(a => new KnowledgeItemViewModel
                {
                    Id = a.Id,
                    Title = a.Title,
                    Category = a.Category,
                    Tags = a.Tags
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
        public async Task<IActionResult> Submit(Guid id, [FromForm] SurveyResponseInputModel model)
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

                var startTime = DateTime.Now;
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var userAgent = Request.Headers["User-Agent"].ToString();
                
                // Get device info from user agent
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
                    CompletionTime = (DateTime.Now - startTime).TotalSeconds,
                    DeviceType = deviceInfo.DeviceType,
                    Browser = deviceInfo.Browser,
                    Location = GetLocationFromIp(ipAddress)
                };

                await _surveyService.SubmitSurveyResponseAsync(responseDto);

                return RedirectToAction("ThankYou", new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar respuesta de encuesta: {Message}", ex.Message);
                TempData["ErrorMessage"] = "Ocurrió un error al procesar su respuesta.";
                return RedirectToAction(nameof(Respond), new { id });
            }
        }

        private void ProcessMultipleChoiceAnswers(SurveyResponseInputModel model)
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

        private (string DeviceType, string Browser) GetDeviceInfo(string userAgent)
        {
            string deviceType = "Desktop";
            string browser = "Unknown";
            
            // Very simple device detection
            if (userAgent.Contains("Mobile") || userAgent.Contains("Android"))
            {
                deviceType = "Mobile";
            }
            else if (userAgent.Contains("iPad") || userAgent.Contains("Tablet"))
            {
                deviceType = "Tablet";
            }
            
            // Simple browser detection
            if (userAgent.Contains("Chrome"))
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
            
            return (deviceType, browser);
        }
        
        private string GetLocationFromIp(string ipAddress)
        {
            // In a real application, you would use a geolocation service
            // For now, we'll just return a default value
            return "Unknown";
        }

        [HttpGet("respond/{id}/thankyou")]
        public async Task<IActionResult> ThankYou(Guid id)
        {
            try
            {
                var survey = await _surveyService.GetSurveyByIdAsync(id);
                
                var viewModel = new ThankYouViewModel
                {
                    SurveyTitle = survey.Title,
                    ThankYouMessage = !string.IsNullOrEmpty(survey.ThankYouMessage) 
                        ? survey.ThankYouMessage 
                        : "¡Gracias por completar nuestra encuesta!",
                    RelatedSurveys = new List<SurveyListItemViewModel>()
                };
                
                // Get other surveys in the same category
                var allSurveys = await _surveyService.GetAllSurveysAsync();
                viewModel.RelatedSurveys = allSurveys
                    .Where(s => s.Category == survey.Category && s.Id != id)
                    .Take(3)
                    .Select(s => new SurveyListItemViewModel
                    {
                        Id = s.Id,
                        Title = s.Title,
                        Description = s.Description?.Length > 100 
                            ? s.Description.Substring(0, 97) + "..." 
                            : s.Description,
                        Category = s.Category,
                        ResponseCount = s.Responses
                    })
                    .ToList();
                
                return View(viewModel);
            }
            catch
            {
                // Fallback to a simple thank you message
                return View(new ThankYouViewModel 
                { 
                    ThankYouMessage = "¡Gracias por completar nuestra encuesta!" 
                });
            }
        }
    }
}
