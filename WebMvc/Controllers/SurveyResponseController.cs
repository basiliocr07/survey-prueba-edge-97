
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
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Modelo inválido en envío de respuesta de encuesta");
                TempData["ErrorMessage"] = "Por favor complete todos los campos requeridos.";
                return RedirectToAction(nameof(Respond), new { id });
            }

            try
            {
                // Inicializar el diccionario de respuestas si es nulo
                if (model.Answers == null)
                {
                    model.Answers = new Dictionary<string, object>();
                }

                // Obtener la encuesta para validación
                var survey = await _surveyService.GetSurveyByIdAsync(id);
                
                // Validar respuestas requeridas
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

                // Procesar formulario para preguntas de opción múltiple
                foreach (var key in Request.Form.Keys)
                {
                    if (key.StartsWith("Answers[") && key.EndsWith("]") && Request.Form[key].Count > 1)
                    {
                        // Extraer ID de pregunta
                        var questionId = key.Substring(8, key.Length - 9);
                        
                        // Obtener todos los valores para esta pregunta (múltiple opción)
                        var values = Request.Form[key].ToList();
                        
                        // Añadir al modelo.Answers
                        model.Answers[questionId] = values;
                    }
                }

                // Convertir todas las claves de questionId a Guid
                var createResponseDto = new CreateSurveyResponseDto
                {
                    SurveyId = id,
                    RespondentName = model.RespondentName?.Trim(),
                    RespondentEmail = model.RespondentEmail?.Trim(),
                    Answers = new Dictionary<string, object>()
                };

                // Asegurar que todas las respuestas estén correctamente formateadas
                foreach (var answer in model.Answers)
                {
                    try
                    {
                        // Verificar si es un ID de pregunta válido
                        if (Guid.TryParse(answer.Key, out Guid questionId))
                        {
                            // Verificar que la pregunta pertenece a esta encuesta
                            if (survey.Questions.Any(q => q.Id == questionId))
                            {
                                createResponseDto.Answers[answer.Key] = answer.Value;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error procesando respuesta para pregunta {QuestionId}", answer.Key);
                    }
                }

                // Guardar la respuesta en la base de datos
                await _surveyService.SubmitSurveyResponseAsync(createResponseDto);
                
                _logger.LogInformation("Respuesta de encuesta guardada exitosamente. SurveyId: {SurveyId}, Email: {Email}", 
                    id, model.RespondentEmail);
                
                TempData["SuccessMessage"] = "¡Gracias! Tu respuesta ha sido enviada correctamente.";
                return RedirectToAction("ThankYou");
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Encuesta no encontrada al enviar respuesta. SurveyId: {SurveyId}", id);
                TempData["ErrorMessage"] = "La encuesta solicitada no existe.";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar la respuesta de la encuesta. SurveyId: {SurveyId}", id);
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
