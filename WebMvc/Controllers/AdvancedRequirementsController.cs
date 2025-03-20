
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
                
                var viewModel = new AdvancedRequirementsViewModel
                {
                    Requirements = requirements,
                    Categories = new[] { "Feature", "Bug", "UI/UX", "Performance", "Security", "Other" },
                    ProjectAreas = new[] { "Web", "Mobile", "Backend", "Frontend", "API", "Database", "Infrastructure", "Other" },
                    TotalCount = requirements.Count,
                    ProposedCount = requirements.Count(r => r.Status.ToLower() == "proposed"),
                    InProgressCount = requirements.Count(r => r.Status.ToLower() == "in-progress"),
                    ImplementedCount = requirements.Count(r => r.Status.ToLower() == "implemented"),
                    RejectedCount = requirements.Count(r => r.Status.ToLower() == "rejected"),
                    CategoryDistribution = requirements
                        .GroupBy(r => r.Category)
                        .ToDictionary(g => g.Key, g => g.Count()),
                    ProjectAreaDistribution = requirements
                        .GroupBy(r => r.ProjectArea)
                        .ToDictionary(g => g.Key, g => g.Count()),
                    MonthlyRequirements = requirements
                        .GroupBy(r => $"{r.CreatedAt.Year}-{r.CreatedAt.Month:D2}")
                        .OrderBy(g => g.Key)
                        .ToDictionary(g => g.Key, g => g.Count())
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
                        Category = model.Category,
                        AcceptanceCriteria = model.AcceptanceCriteria,
                        TargetDate = model.TargetDate
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
