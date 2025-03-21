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
            try
            {
                var requirements = await _requirementService.GetAllRequirementsAsync();
                
                var viewModel = new RequirementsViewModel
                {
                    Requirements = requirements,
                    Categories = new[] { "Feature", "Bug", "UI/UX", "Performance", "Security", "Other" },
                    ProjectAreas = new[] { "Web", "Mobile", "Backend", "Frontend", "API", "Database", "Infrastructure", "Other" },
                    TotalCount = requirements.Count,
                    ProposedCount = requirements.Count(r => r.Status?.ToLower() == "proposed"),
                    InProgressCount = requirements.Count(r => r.Status?.ToLower() == "in-progress"),
                    TestingCount = requirements.Count(r => r.Status?.ToLower() == "testing"),
                    CompletedCount = requirements.Count(r => r.Status?.ToLower() == "implemented"),
                    ImplementedCount = requirements.Count(r => r.Status?.ToLower() == "implemented"), // Added this mapping
                    CriticalCount = requirements.Count(r => r.Priority?.ToLower() == "critical"),
                    HighCount = requirements.Count(r => r.Priority?.ToLower() == "high"),
                    MediumCount = requirements.Count(r => r.Priority?.ToLower() == "medium"),
                    LowCount = requirements.Count(r => r.Priority?.ToLower() == "low"),
                    CategoryDistribution = requirements
                        .GroupBy(r => r.Category)
                        .ToDictionary(g => g.Key ?? "Uncategorized", g => g.Count()),
                    ProjectAreaDistribution = requirements
                        .GroupBy(r => r.ProjectArea)
                        .ToDictionary(g => g.Key ?? "General", g => g.Count()),
                    MonthlyRequirements = requirements
                        .GroupBy(r => $"{r.CreatedAt.Year}-{r.CreatedAt.Month:D2}")
                        .OrderBy(g => g.Key)
                        .ToDictionary(g => g.Key, g => g.Count()),
                    TotalRequirements = requirements.Count,
                    ProposedRequirements = requirements.Count(r => r.Status?.ToLower() == "proposed"),
                    InProgressRequirements = requirements.Count(r => r.Status?.ToLower() == "in-progress"),
                    ImplementedRequirements = requirements.Count(r => r.Status?.ToLower() == "implemented"),
                    RejectedRequirements = requirements.Count(r => r.Status?.ToLower() == "rejected"),
                    PriorityDistribution = requirements
                        .GroupBy(r => r.Priority)
                        .ToDictionary(g => g.Key ?? "Unspecified", g => g.Count())
                };

                if (!User.IsInRole("Admin"))
                {
                    return RedirectToAction("Create");
                }
                
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar la página de requerimientos");
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
                "Nuevo Requerimiento" : "Enviar Requerimiento";
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
                    _logger.LogError(ex, "Error al crear el requerimiento");
                    ModelState.AddModelError("", "Ocurrió un error al enviar el requerimiento.");
                }
            }

            ViewData["PageTitle"] = User.IsInRole("Admin") ? 
                "Nuevo Requerimiento" : "Enviar Requerimiento";
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
                
                var viewModel = new RequirementsViewModel
                {
                    TotalRequirements = reportsData.TotalRequirements,
                    ProposedRequirements = reportsData.ProposedRequirements,
                    InProgressRequirements = reportsData.InProgressRequirements,
                    ImplementedRequirements = reportsData.ImplementedRequirements,
                    RejectedRequirements = reportsData.RejectedRequirements,
                    CategoryDistribution = reportsData.CategoryDistribution,
                    PriorityDistribution = reportsData.PriorityDistribution,
                    ProjectAreaDistribution = reportsData.ProjectAreaDistribution,
                    MonthlyRequirements = reportsData.MonthlyRequirements
                };
                
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los informes de requerimientos");
                return View("Error");
            }
        }
        
        [HttpGet]
        public async Task<IActionResult> List(string status = "", string priority = "", string category = "", string projectArea = "", string searchTerm = "")
        {
            try
            {
                List<RequirementDto> requirements;
                
                if (!string.IsNullOrEmpty(status))
                {
                    requirements = await _requirementService.GetRequirementsByStatusAsync(status);
                }
                else if (!string.IsNullOrEmpty(priority))
                {
                    requirements = await _requirementService.GetRequirementsByPriorityAsync(priority);
                }
                else if (!string.IsNullOrEmpty(category))
                {
                    requirements = await _requirementService.GetRequirementsByCategoryAsync(category);
                }
                else if (!string.IsNullOrEmpty(projectArea))
                {
                    requirements = await _requirementService.GetRequirementsByProjectAreaAsync(projectArea);
                }
                else if (!string.IsNullOrEmpty(searchTerm))
                {
                    requirements = await _requirementService.SearchRequirementsAsync(searchTerm);
                }
                else
                {
                    requirements = await _requirementService.GetAllRequirementsAsync();
                }
                
                var viewModel = new RequirementsListViewModel
                {
                    Requirements = requirements,
                    Categories = new[] { "Feature", "Bug", "UI/UX", "Performance", "Security", "Other" },
                    StatusFilter = status,
                    PriorityFilter = priority,
                    CategoryFilter = category,
                    ProjectAreaFilter = projectArea,
                    SearchTerm = searchTerm
                };
                
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de requerimientos");
                return View("Error");
            }
        }
    }
}
