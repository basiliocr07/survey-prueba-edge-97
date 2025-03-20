
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
    public class AdvancedRequirementsController : Controller
    {
        private readonly IRequirementService _requirementService;
        private readonly IKnowledgeBaseService _knowledgeBaseService;
        private readonly ILogger<AdvancedRequirementsController> _logger;

        public AdvancedRequirementsController(
            IRequirementService requirementService,
            IKnowledgeBaseService knowledgeBaseService,
            ILogger<AdvancedRequirementsController> logger)
        {
            _requirementService = requirementService;
            _knowledgeBaseService = knowledgeBaseService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var requirements = await _requirementService.GetAllRequirementsAsync();
                
                var viewModel = new RequirementsViewModel
                {
                    Requirements = requirements,
                    KnowledgeBase = new List<KnowledgeBaseItemDto>(),
                    Categories = new[] { "Feature", "Bug", "UI/UX", "Performance", "Security", "Other" },
                    TotalCount = requirements.Count,
                    ProposedCount = requirements.Count(r => r.Status.ToLower() == "proposed"),
                    InProgressCount = requirements.Count(r => r.Status.ToLower() == "in-progress"),
                    TestingCount = requirements.Count(r => r.Status.ToLower() == "testing"),
                    CompletedCount = requirements.Count(r => r.Status.ToLower() == "implemented"),
                    CriticalCount = requirements.Count(r => r.Priority.ToLower() == "critical"),
                    HighCount = requirements.Count(r => r.Priority.ToLower() == "high"),
                    MediumCount = requirements.Count(r => r.Priority.ToLower() == "medium"),
                    LowCount = requirements.Count(r => r.Priority.ToLower() == "low")
                };
                
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar la página de requerimientos avanzados");
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var requirement = await _requirementService.GetRequirementByIdAsync(id);
                if (requirement == null)
                {
                    return NotFound();
                }
                
                var viewModel = new RequirementDetailViewModel
                {
                    Requirement = requirement
                };
                
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener detalles del requerimiento con ID {id}");
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            var viewModel = new NewRequirementViewModel();
            ViewData["PageTitle"] = User.IsInRole("Admin") ? 
                "Nuevo Requerimiento Avanzado" : "Enviar Requerimiento Avanzado";
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
                    var dto = new CreateRequirementDto
                    {
                        Title = model.Title,
                        Description = model.Description,
                        Priority = model.Priority,
                        ProjectArea = model.ProjectArea,
                        CustomerName = model.CustomerName,
                        CustomerEmail = model.CustomerEmail,
                        IsAnonymous = model.IsAnonymous,
                        Category = model.Category
                    };

                    await _requirementService.CreateRequirementAsync(dto);

                    TempData["SuccessMessage"] = "Requerimiento enviado correctamente.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al crear el requerimiento avanzado");
                    ModelState.AddModelError("", "Ocurrió un error al enviar el requerimiento.");
                }
            }

            ViewData["PageTitle"] = User.IsInRole("Admin") ? 
                "Nuevo Requerimiento Avanzado" : "Enviar Requerimiento Avanzado";
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateStatus(Guid id, string status, string response, int? completionPercentage)
        {
            try
            {
                var statusUpdate = new RequirementStatusUpdateDto
                {
                    Status = status,
                    Response = response,
                    CompletionPercentage = completionPercentage
                };
                
                await _requirementService.UpdateRequirementStatusAsync(id, statusUpdate);
                
                TempData["SuccessMessage"] = "Estado del requerimiento actualizado correctamente.";
                return RedirectToAction("Details", new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el estado del requerimiento con ID {id}");
                TempData["ErrorMessage"] = "Ocurrió un error al actualizar el estado del requerimiento.";
                return RedirectToAction("Details", new { id });
            }
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Reports()
        {
            try
            {
                var reportsData = await _requirementService.GetRequirementReportsAsync();
                return View(reportsData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los informes de requerimientos");
                return View("Error");
            }
        }
    }
}
