
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SurveyApp.Application.DTOs;
using SurveyApp.Application.Services;
using SurveyApp.WebMvc.Models;

namespace SurveyApp.WebMvc.Controllers
{
    [Authorize(Policy = "ClientAccess")]
    public class RequirementsController : Controller
    {
        private readonly IKnowledgeBaseService _knowledgeBaseService;
        private readonly ILogger<RequirementsController> _logger;

        public RequirementsController(
            IKnowledgeBaseService knowledgeBaseService,
            ILogger<RequirementsController> logger)
        {
            _knowledgeBaseService = knowledgeBaseService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Los clientes son redirigidos a la página de creación
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Create");
            }

            try
            {
                var viewModel = new RequirementsViewModel
                {
                    Title = "Gestión de Requerimientos",
                    Description = "Visualiza y gestiona los requerimientos del sistema."
                };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar la página de requerimientos");
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new NewRequirementViewModel();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(NewRequirementViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Convertimos el requerimiento en un artículo de la base de conocimientos
                    var dto = new CreateKnowledgeBaseItemDto
                    {
                        Title = model.Title,
                        Content = model.Description,
                        Category = "Requerimiento",
                        Tags = new[] { model.Priority, model.ProjectArea }
                    };

                    await _knowledgeBaseService.CreateKnowledgeBaseItemAsync(dto);

                    TempData["SuccessMessage"] = "Requerimiento enviado correctamente.";
                    return RedirectToAction(User.IsInRole("Admin") ? "Index" : "Create");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al crear el requerimiento");
                    ModelState.AddModelError("", "Ocurrió un error al enviar el requerimiento.");
                }
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> ViewRequirements()
        {
            try
            {
                var items = await _knowledgeBaseService.GetKnowledgeBaseItemsByCategoryAsync("Requerimiento");
                var viewModel = new RequirementsListViewModel
                {
                    Requirements = items.Select(i => new RequirementViewModel
                    {
                        Id = i.Id,
                        Title = i.Title,
                        Description = i.Content,
                        CreatedAt = i.CreatedAt,
                        UpdatedAt = i.UpdatedAt,
                        Priority = i.Tags.FirstOrDefault(t => t == "Alta" || t == "Media" || t == "Baja") ?? "Normal",
                        ProjectArea = i.Tags.FirstOrDefault(t => t != "Alta" && t != "Media" && t != "Baja") ?? "General"
                    }).ToList()
                };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los requerimientos");
                return View("Error");
            }
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> KnowledgeBase()
        {
            try
            {
                var items = await _knowledgeBaseService.GetAllKnowledgeBaseItemsAsync();
                var viewModel = new KnowledgeBaseViewModel
                {
                    KnowledgeBaseItems = items
                };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la base de conocimientos");
                return View("Error");
            }
        }
    }
}
