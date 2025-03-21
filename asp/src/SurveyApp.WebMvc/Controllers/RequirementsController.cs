
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

        public async Task<IActionResult> Index(string tab = "view")
        {
            try
            {
                // Get all requirements
                var requirements = await _requirementService.GetAllRequirementsAsync();
                
                // Get reports data for charts
                var reports = await _requirementService.GetRequirementReportsAsync();
                
                // Create view model
                var viewModel = new RequirementsViewModel
                {
                    Requirements = requirements,
                    ActiveTab = tab,
                    TotalRequirements = reports.TotalRequirements,
                    ProposedRequirements = reports.ProposedRequirements,
                    InProgressRequirements = reports.InProgressRequirements,
                    ImplementedRequirements = reports.ImplementedRequirements,
                    RejectedRequirements = reports.RejectedRequirements,
                    StatusDistribution = new Dictionary<string, int>
                    {
                        { "Proposed", reports.ProposedRequirements },
                        { "In Progress", reports.InProgressRequirements },
                        { "Implemented", reports.ImplementedRequirements },
                        { "Rejected", reports.RejectedRequirements }
                    },
                    PriorityDistribution = reports.PriorityDistribution,
                    CategoryDistribution = reports.CategoryDistribution,
                    ProjectAreaDistribution = reports.ProjectAreaDistribution,
                    MonthlyRequirements = reports.MonthlyRequirements
                };
                
                // Redirect non-admin users to the submit form directly
                if (!User.IsInRole("Admin"))
                {
                    viewModel.ActiveTab = "submit";
                }
                
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading requirements dashboard");
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
                
                // Get related knowledge base items
                var relatedItems = await _knowledgeBaseService.GetRelatedItemsAsync(requirement.Title, 3);
                
                var viewModel = new RequirementDetailViewModel
                {
                    Requirement = requirement,
                    RelatedItems = relatedItems
                };
                
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting requirement details for ID: {id}");
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new NewRequirementViewModel());
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

                    var newRequirement = await _requirementService.CreateRequirementAsync(dto);

                    TempData["SuccessMessage"] = "Requirement submitted successfully.";
                    
                    if (User.IsInRole("Admin"))
                    {
                        return RedirectToAction(nameof(Details), new { id = newRequirement.Id });
                    }
                    else
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating requirement");
                    ModelState.AddModelError("", "An error occurred while submitting the requirement.");
                }
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Edit(Guid id)
        {
            var requirement = await _requirementService.GetRequirementByIdAsync(id);
            if (requirement == null)
            {
                return NotFound();
            }
            
            var viewModel = new UpdateRequirementViewModel
            {
                Id = requirement.Id,
                Title = requirement.Title,
                Description = requirement.Description,
                Priority = requirement.Priority,
                Status = requirement.Status,
                ProjectArea = requirement.ProjectArea,
                Response = requirement.Response,
                CompletionPercentage = requirement.CompletionPercentage,
                Category = requirement.Category,
                AcceptanceCriteria = requirement.AcceptanceCriteria,
                TargetDate = requirement.TargetDate
            };
            
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Edit(UpdateRequirementViewModel model)
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
                        Response = model.Response,
                        CompletionPercentage = model.CompletionPercentage,
                        Category = model.Category,
                        AcceptanceCriteria = model.AcceptanceCriteria,
                        TargetDate = model.TargetDate
                    };

                    await _requirementService.UpdateRequirementAsync(model.Id, dto);

                    TempData["SuccessMessage"] = "Requirement updated successfully.";
                    return RedirectToAction(nameof(Details), new { id = model.Id });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error updating requirement ID: {model.Id}");
                    ModelState.AddModelError("", "An error occurred while updating the requirement.");
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateStatus(Guid id, string status, string response, int? completionPercentage)
        {
            try
            {
                var dto = new RequirementStatusUpdateDto
                {
                    Status = status,
                    Comment = response,
                    CompletionPercentage = completionPercentage
                };

                await _requirementService.UpdateRequirementStatusAsync(id, dto);

                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = true });
                }
                
                TempData["SuccessMessage"] = "Requirement status updated successfully.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating requirement status for ID: {id}");
                
                if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                {
                    return Json(new { success = false, message = "An error occurred while updating the status." });
                }
                
                TempData["ErrorMessage"] = "An error occurred while updating the requirement status.";
                return RedirectToAction(nameof(Details), new { id });
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
                
                TempData["SuccessMessage"] = "Requirement deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting requirement ID: {id}");
                TempData["ErrorMessage"] = "An error occurred while deleting the requirement.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ViewRequirements(string category = "", string search = "")
        {
            try
            {
                var requirements = await _requirementService.GetAllRequirementsAsync();
                
                // Filter by category if provided
                if (!string.IsNullOrEmpty(category))
                {
                    requirements = requirements.Where(r => r.Category?.Equals(category, StringComparison.OrdinalIgnoreCase) == true).ToList();
                }
                
                // Filter by search term if provided
                if (!string.IsNullOrEmpty(search))
                {
                    requirements = requirements.Where(r => 
                        r.Title.Contains(search, StringComparison.OrdinalIgnoreCase) || 
                        r.Description.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
                }
                
                var viewModel = new RequirementsListViewModel
                {
                    Requirements = requirements,
                    Categories = new[] { "Feature", "Bug", "UI/UX", "Performance", "Security", "Other" },
                    CategoryFilter = category,
                    SearchTerm = search
                };
                
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading requirements list view");
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Reports()
        {
            try
            {
                var reports = await _requirementService.GetRequirementReportsAsync();
                
                var viewModel = new RequirementsViewModel
                {
                    TotalRequirements = reports.TotalRequirements,
                    ProposedRequirements = reports.ProposedRequirements,
                    InProgressRequirements = reports.InProgressRequirements,
                    ImplementedRequirements = reports.ImplementedRequirements,
                    RejectedRequirements = reports.RejectedRequirements,
                    CategoryDistribution = reports.CategoryDistribution,
                    PriorityDistribution = reports.PriorityDistribution,
                    ProjectAreaDistribution = reports.ProjectAreaDistribution,
                    MonthlyRequirements = reports.MonthlyRequirements
                };
                
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading requirements reports");
                return View("Error");
            }
        }
    }
}
