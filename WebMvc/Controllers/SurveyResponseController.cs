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
        private readonly ILogger<SurveyResponseController> _logger;

        public SurveyResponseController(
            ISurveyService surveyService,
            ILogger<SurveyResponseController> logger)
        {
            _surveyService = surveyService;
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

                var responseDto = new CreateSurveyResponseDto
                {
                    SurveyId = id,
                    RespondentName = model.RespondentName,
                    RespondentEmail = model.RespondentEmail,
                    RespondentPhone = model.RespondentPhone,
                    RespondentCompany = model.RespondentCompany,
                    Answers = model.Answers,
                    IsExistingClient = model.IsExistingClient,
                    ExistingClientId = model.ExistingClientId
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

        [HttpGet("respond/{id}/thankyou")]
        public async Task<IActionResult> ThankYou(Guid id)
        {
            try
            {
                var survey = await _surveyService.GetSurveyByIdAsync(id);
                ViewBag.SurveyTitle = survey.Title;
                ViewBag.ThankYouMessage = "¡Gracias por completar nuestra encuesta!";
                
                return View();
            }
            catch
            {
                return View(); // Mostrar un mensaje genérico si no se puede cargar la encuesta
            }
        }
        
        [HttpGet("responses")]
        public async Task<IActionResult> List()
        {
            try
            {
                var recentResponses = await _surveyService.GetRecentResponsesAsync(20);
                
                return View(recentResponses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar respuestas");
                TempData["ErrorMessage"] = "Error al cargar las respuestas.";
                return RedirectToAction("Index", "Dashboard");
            }
        }
        
        [HttpGet("responses/{id}")]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var response = await _surveyService.GetResponseByIdAsync(id);
                
                if (response == null)
                {
                    TempData["ErrorMessage"] = "La respuesta solicitada no existe.";
                    return RedirectToAction(nameof(List));
                }
                
                var viewModel = new SurveyResponseAnalyticsViewModel
                {
                    Id = response.Id,
                    SurveyId = response.SurveyId,
                    SurveyTitle = response.SurveyTitle,
                    RespondentName = response.RespondentName,
                    RespondentEmail = response.RespondentEmail,
                    RespondentPhone = response.RespondentPhone,
                    RespondentCompany = response.RespondentCompany,
                    SubmittedAt = response.SubmittedAt,
                    IsValidated = response.Answers.All(a => a.IsValid),
                    CompletionTime = 0,
                    QuestionCount = response.Answers.Count,
                    ValidAnswersCount = response.Answers.Count(a => a.IsValid),
                    Answers = response.Answers.Select(a => new QuestionAnswerViewModel
                    {
                        QuestionId = a.QuestionId,
                        QuestionTitle = a.QuestionTitle,
                        QuestionType = a.QuestionType,
                        Answer = a.Answer,
                        MultipleAnswers = a.MultipleAnswers,
                        IsValid = a.IsValid,
                        ScoreValue = 0,
                        CompletionTimeSeconds = 0
                    }).ToList()
                };
                
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener detalles de respuesta: {Message}", ex.Message);
                TempData["ErrorMessage"] = "Ocurrió un error al cargar los detalles de la respuesta.";
                return RedirectToAction(nameof(List));
            }
        }
    }
}
