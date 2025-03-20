
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
                        Priority = model.Priority,
                        ProjectArea = model.ProjectArea,
                        CustomerName = model.CustomerName,
                        CustomerEmail = model.CustomerEmail,
                        IsAnonymous = model.IsAnonymous,
                        Category = model.Category
                    };

                    await _requirementService.CreateRequirementAsync(dto);

                    // También lo agregamos como elemento de la base de conocimientos
                    var kbDto = new CreateKnowledgeBaseItemDto
                    {
                        Title = model.Title,
                        Content = model.Description,
                        Category = "Requirement",
                        Tags = new[] { model.Priority, model.ProjectArea, model.Category }
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
        public async Task<IActionResult> ViewRequirements(string status = "", string priority = "", string category = "", string search = "", string projectArea = "")
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
                else if (!string.IsNullOrEmpty(search))
                {
                    requirements = await _requirementService.SearchRequirementsAsync(search);
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
                    SearchTerm = search
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
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var requirement = await _requirementService.GetRequirementByIdAsync(id);
                if (requirement == null)
                {
                    return NotFound();
                }
                
                // Obtener elementos relacionados de la base de conocimientos
                var relatedItems = await _knowledgeBaseService.GetItemsByTagsAsync(
                    new[] { requirement.Priority, requirement.ProjectArea, requirement.Category }.Where(t => !string.IsNullOrEmpty(t)).ToArray()
                );
                
                var viewModel = new RequirementDetailViewModel
                {
                    Requirement = requirement,
                    RelatedItems = relatedItems
                };
                
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener los detalles del requerimiento con ID {id}");
                return View("Error");
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

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> KnowledgeBase(string search = "")
        {
            try
            {
                var items = string.IsNullOrEmpty(search)
                    ? await _knowledgeBaseService.GetAllKnowledgeBaseItemsAsync()
                    : await _knowledgeBaseService.SearchKnowledgeBaseItemsAsync(search);
                
                var viewModel = new KnowledgeBaseViewModel
                {
                    KnowledgeBaseItems = items,
                    SearchTerm = search
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
                    ProjectArea = requirement.ProjectArea ?? "General",
                    Status = requirement.Status,
                    CustomerName = requirement.CustomerName,
                    CustomerEmail = requirement.CustomerEmail,
                    IsAnonymous = requirement.IsAnonymous,
                    Category = requirement.Category ?? "Feature",
                    AcceptanceCriteria = requirement.AcceptanceCriteria,
                    TargetDate = requirement.TargetDate
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
                        Status = model.Status,
                        ProjectArea = model.ProjectArea,
                        Category = model.Category,
                        AcceptanceCriteria = model.AcceptanceCriteria,
                        TargetDate = model.TargetDate
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
        public async Task<IActionResult> UpdateStatus(Guid id, string status, string response)
        {
            try
            {
                var statusUpdate = new RequirementStatusUpdateDto
                {
                    Status = status,
                    Response = response
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
