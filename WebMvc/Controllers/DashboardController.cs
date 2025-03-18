
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SurveyApp.Application.Services;
using SurveyApp.Domain.Entities;
using SurveyApp.WebMvc.Models;

namespace SurveyApp.WebMvc.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ISurveyService _surveyService;
        private readonly ISuggestionService _suggestionService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(
            ISurveyService surveyService,
            ISuggestionService suggestionService,
            ILogger<DashboardController> logger)
        {
            _surveyService = surveyService;
            _suggestionService = suggestionService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var viewModel = new DashboardViewModel();

            try
            {
                // Obtener la última encuesta
                var surveys = await _surveyService.GetAllSurveysAsync();
                if (surveys.Count > 0)
                {
                    viewModel.LatestSurvey = new SurveyListItemViewModel
                    {
                        Id = surveys[0].Id,
                        Title = surveys[0].Title,
                        Description = surveys[0].Description,
                        CreatedAt = surveys[0].CreatedAt,
                        ResponseCount = surveys[0].Responses,
                        Status = surveys[0].Status ?? "pending"
                    };
                }

                // Obtener la última sugerencia
                var suggestions = await _suggestionService.GetAllSuggestionsAsync();
                if (suggestions.Count > 0)
                {
                    viewModel.LatestSuggestion = new SuggestionListItemViewModel
                    {
                        Id = suggestions[0].Id,
                        Content = suggestions[0].Content,
                        CustomerName = suggestions[0].CustomerName,
                        CreatedAt = suggestions[0].CreatedAt,
                        Status = suggestions[0].Status
                    };
                }

                // Para el último requerimiento, podríamos usar algún servicio específico
                // Como no tenemos uno directamente, usaremos una sugerencia con prioridad alta como ejemplo
                var requirements = suggestions.FindAll(s => s.Priority?.ToLower() == "high");
                if (requirements.Count > 0)
                {
                    viewModel.LatestRequirement = new RequirementListItemViewModel
                    {
                        Id = requirements[0].Id,
                        Title = requirements[0].Title ?? requirements[0].Content,
                        Description = requirements[0].Description,
                        Priority = requirements[0].Priority ?? "medium",
                        CreatedAt = requirements[0].CreatedAt,
                        Status = requirements[0].Status
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar datos del dashboard");
                TempData["ErrorMessage"] = "No se pudieron cargar todos los datos del dashboard.";
            }

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSurveyStatus(Guid id, string newStatus)
        {
            try
            {
                // Actualizar el estado de la encuesta
                await _surveyService.UpdateSurveyStatusAsync(id, newStatus);
                TempData["SuccessMessage"] = $"Estado de encuesta actualizado a: {GetStatusLabel(newStatus)}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar estado de encuesta");
                TempData["ErrorMessage"] = "No se pudo actualizar el estado de la encuesta.";
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSuggestionStatus(Guid id, string newStatus)
        {
            try
            {
                // Convertir el estado de string a enum
                SuggestionStatus status = newStatus.ToLower() switch
                {
                    "pending" => SuggestionStatus.New,
                    "in-progress" => SuggestionStatus.Reviewed,
                    "closed" => SuggestionStatus.Implemented,
                    _ => SuggestionStatus.New
                };

                // Actualizar el estado de la sugerencia
                await _suggestionService.UpdateSuggestionStatusAsync(id, status);
                TempData["SuccessMessage"] = $"Estado de sugerencia actualizado a: {GetStatusLabel(newStatus)}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar estado de sugerencia");
                TempData["ErrorMessage"] = "No se pudo actualizar el estado de la sugerencia.";
            }

            return RedirectToAction(nameof(Index));
        }

        // Para requisitos podríamos implementar otro método similar

        private string GetStatusLabel(string status)
        {
            return status.ToLower() switch
            {
                "pending" => "Pendiente",
                "in-progress" => "En curso",
                "closed" => "Cerrada",
                _ => status
            };
        }
    }
}
