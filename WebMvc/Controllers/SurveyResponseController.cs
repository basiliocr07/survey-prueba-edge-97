
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SurveyApp.Application.DTOs;
using SurveyApp.Application.Services;
using SurveyApp.WebMvc.Models;
using System.Linq;

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
            catch (KeyNotFoundException)
            {
                TempData["ErrorMessage"] = "La encuesta solicitada no existe.";
                return RedirectToAction("Index", "Home");
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
            try
            {
                // Ensure model.Answers is initialized
                if (model.Answers == null)
                {
                    model.Answers = new Dictionary<string, object>();
                }

                // Process form collection for multiple-choice questions
                foreach (var key in Request.Form.Keys)
                {
                    if (key.StartsWith("Answers[") && key.EndsWith("]") && Request.Form[key].Count > 1)
                    {
                        // Extract question ID
                        var questionId = key.Substring(8, key.Length - 9);
                        
                        // Get all values for this question (for multiple-choice)
                        var values = Request.Form[key].ToList();
                        
                        // Add to model.Answers
                        model.Answers[questionId] = values;
                    }
                }

                var createResponseDto = new CreateSurveyResponseDto
                {
                    SurveyId = id,
                    RespondentName = model.RespondentName,
                    RespondentEmail = model.RespondentEmail,
                    Answers = model.Answers
                };

                await _surveyService.SubmitSurveyResponseAsync(createResponseDto);
                
                TempData["SuccessMessage"] = "¡Gracias! Tu respuesta ha sido enviada correctamente.";
                return RedirectToAction("ThankYou");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar la respuesta de la encuesta");
                TempData["ErrorMessage"] = "Ocurrió un error al enviar tu respuesta.";
                return RedirectToAction(nameof(Respond), new { id });
            }
        }

        [HttpGet("thank-you")]
        public IActionResult ThankYou()
        {
            return View();
        }
    }
}
