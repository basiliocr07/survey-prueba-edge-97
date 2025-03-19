
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SurveyApp.Application.DTOs;
using SurveyApp.Application.Services;
using SurveyApp.Domain.Entities;
using SurveyApp.WebMvc.Models;

namespace SurveyApp.WebMvc.Controllers
{
    [Authorize(Policy = "ClientAccess")]
    public class SuggestionsController : Controller
    {
        private readonly ISuggestionService _suggestionService;
        private readonly ILogger<SuggestionsController> _logger;

        public SuggestionsController(
            ISuggestionService suggestionService,
            ILogger<SuggestionsController> logger)
        {
            _suggestionService = suggestionService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            // Solo administradores pueden ver el listado completo
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Create");
            }

            try
            {
                var suggestions = await _suggestionService.GetAllSuggestionsAsync();
                var viewModel = new SuggestionListViewModel
                {
                    Suggestions = suggestions
                };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las sugerencias");
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["PageTitle"] = User.IsInRole("Admin") ? 
                "Gestión de Sugerencias" : "Enviar Sugerencia";
            return View(new CreateSuggestionViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSuggestionViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var dto = new CreateSuggestionDto
                    {
                        Content = model.Content,
                        CustomerName = model.CustomerName,
                        CustomerEmail = model.CustomerEmail,
                        Category = model.Category
                    };

                    await _suggestionService.CreateSuggestionAsync(dto);

                    TempData["SuccessMessage"] = "Sugerencia enviada correctamente.";
                    return RedirectToAction(User.IsInRole("Admin") ? "Index" : "Create");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al crear la sugerencia");
                    ModelState.AddModelError("", "Ocurrió un error al enviar la sugerencia.");
                }
            }

            ViewData["PageTitle"] = User.IsInRole("Admin") ? 
                "Gestión de Sugerencias" : "Enviar Sugerencia";
            return View(model);
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> View(Guid id)
        {
            try
            {
                var suggestion = await _suggestionService.GetSuggestionByIdAsync(id);
                if (suggestion == null)
                {
                    return NotFound();
                }

                var viewModel = new SuggestionDetailViewModel
                {
                    Id = suggestion.Id,
                    Content = suggestion.Content,
                    CustomerName = suggestion.CustomerName,
                    CustomerEmail = suggestion.CustomerEmail,
                    CreatedAt = suggestion.CreatedAt,
                    Status = suggestion.Status.ToString(),
                    Category = suggestion.Category,
                    Response = suggestion.Response
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la sugerencia");
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateStatus(Guid id, string status, string response)
        {
            try
            {
                if (!Enum.TryParse<SuggestionStatus>(status, out var suggestionStatus))
                {
                    ModelState.AddModelError("", "Estado de sugerencia no válido.");
                    return RedirectToAction("View", new { id });
                }

                await _suggestionService.UpdateSuggestionStatusAsync(id, suggestionStatus, response);

                TempData["SuccessMessage"] = "Estado de la sugerencia actualizado correctamente.";
                return RedirectToAction("View", new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el estado de la sugerencia");
                TempData["ErrorMessage"] = "Ocurrió un error al actualizar el estado de la sugerencia.";
                return RedirectToAction("View", new { id });
            }
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Reports()
        {
            try
            {
                var suggestions = await _suggestionService.GetAllSuggestionsAsync();
                var viewModel = new SuggestionReportsViewModel
                {
                    Suggestions = suggestions
                };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los reportes de sugerencias");
                return View("Error");
            }
        }
    }
}
