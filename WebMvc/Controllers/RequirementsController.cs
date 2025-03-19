
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
        private readonly IRequirementService _requirementService;
        private readonly IKnowledgeBaseService _knowledgeBaseService;
        private readonly ILogger<RequirementsController> _logger;

        public RequirementsController(
            IRequirementService requirementService,
            IKnowledgeBaseService knowledgeBaseService,
            ILogger<RequirementsController> logger)
        {
            _requirementService = requirementService;
            _knowledgeBaseService = knowledgeBaseService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            // Los clientes son redirigidos a la página de creación
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Create");
            }

            try
            {
                var requirements = await _requirementService.GetAllRequirementsAsync();
                
                var viewModel = new RequirementsViewModel
                {
                    Requirements = requirements.Select(r => new SuggestionDto 
                    { 
                        Id = r.Id, 
                        Content = r.Title, 
                        CustomerName = "Sistema", 
                        Status = r.Status 
                    }).ToList(),
                    KnowledgeBase = new List<KnowledgeBaseItemDto>(),
                    Categories = new[] { "Frontend", "Backend", "UI/UX", "Base de datos", "General" },
                    TotalCount = requirements.Count,
                    ProposedCount = requirements.Count(r => r.Status.ToLower() == "pending" || r.Status.ToLower() == "proposed"),
                    InProgressCount = requirements.Count(r => r.Status.ToLower() == "in progress"),
                    TestingCount = requirements.Count(r => r.Status.ToLower() == "testing"),
                    CompletedCount = requirements.Count(r => r.Status.ToLower() == "completed"),
                    CriticalCount = requirements.Count(r => r.Priority.ToLower() == "alta" || r.Priority.ToLower() == "critical"),
                    HighCount = requirements.Count(r => r.Priority.ToLower() == "high"),
                    MediumCount = requirements.Count(r => r.Priority.ToLower() == "media" || r.Priority.ToLower() == "medium"),
                    LowCount = requirements.Count(r => r.Priority.ToLower() == "baja" || r.Priority.ToLower() == "low")
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
            ViewData["PageTitle"] = User.IsInRole("Admin") ? 
                "Gestión de Requerimientos" : "Enviar Requerimiento";
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
                    // Creamos el requerimiento usando el servicio de requerimientos
                    var dto = new CreateRequirementDto
                    {
                        Title = model.Title,
                        Description = model.Description,
                        Priority = model.Priority
                    };

                    await _requirementService.CreateRequirementAsync(dto);

                    // También lo agregamos como elemento de la base de conocimientos
                    var kbDto = new CreateKnowledgeBaseItemDto
                    {
                        Title = model.Title,
                        Content = model.Description,
                        Category = "Requerimiento",
                        Tags = new[] { model.Priority, model.ProjectArea }
                    };

                    await _knowledgeBaseService.CreateKnowledgeBaseItemAsync(kbDto);

                    TempData["SuccessMessage"] = "Requerimiento enviado correctamente.";
                    return RedirectToAction(User.IsInRole("Admin") ? "Index" : "Create");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al crear el requerimiento");
                    ModelState.AddModelError("", "Ocurrió un error al enviar el requerimiento.");
                }
            }

            ViewData["PageTitle"] = User.IsInRole("Admin") ? 
                "Gestión de Requerimientos" : "Enviar Requerimiento";
            return View(model);
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> ViewRequirements()
        {
            try
            {
                var requirements = await _requirementService.GetAllRequirementsAsync();
                var viewModel = new RequirementsListViewModel
                {
                    Requirements = requirements.Select(r => new SuggestionDto
                    {
                        Id = r.Id,
                        Content = r.Title + " - " + r.Description,
                        CustomerName = "Sistema",
                        CustomerEmail = "sistema@example.com",
                        CreatedAt = r.CreatedAt,
                        Status = r.Status,
                        Category = r.Priority
                    }).ToList(),
                    Categories = new[] { "Alta", "Media", "Baja" },
                    CategoryFilter = "",
                    SearchTerm = ""
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

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var requirement = await _requirementService.GetRequirementByIdAsync(id);
                if (requirement == null)
                {
                    return NotFound();
                }

                var viewModel = new NewRequirementViewModel
                {
                    Title = requirement.Title,
                    Description = requirement.Description,
                    Priority = requirement.Priority,
                    ProjectArea = "General"  // Por defecto si no existe
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el requerimiento con ID {id}");
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Edit(Guid id, NewRequirementViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var dto = new UpdateRequirementDto
                    {
                        Title = model.Title,
                        Description = model.Description,
                        Priority = model.Priority,
                        Status = "pending"  // Por defecto
                    };

                    await _requirementService.UpdateRequirementAsync(id, dto);

                    TempData["SuccessMessage"] = "Requerimiento actualizado correctamente.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error al actualizar el requerimiento con ID {id}");
                    ModelState.AddModelError("", "Ocurrió un error al actualizar el requerimiento.");
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _requirementService.DeleteRequirementAsync(id);
                TempData["SuccessMessage"] = "Requerimiento eliminado correctamente.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar el requerimiento con ID {id}");
                TempData["ErrorMessage"] = "Ocurrió un error al eliminar el requerimiento.";
            }

            return RedirectToAction("Index");
        }
    }
}
