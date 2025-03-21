using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SurveyApp.Application.Services;
using SurveyApp.WebMvc.Models;
using SurveyApp.Domain.Entities;
using SurveyApp.Application.DTOs;

namespace SurveyApp.WebMvc.Controllers
{
    [Authorize] // Require authentication
    public class DashboardController : Controller
    {
        private readonly ISurveyService _surveyService;
        private readonly ISuggestionService _suggestionService;
        private readonly IRequirementService _requirementService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(
            ISurveyService surveyService,
            ISuggestionService suggestionService,
            IRequirementService requirementService,
            ILogger<DashboardController> logger)
        {
            _surveyService = surveyService;
            _suggestionService = suggestionService;
            _requirementService = requirementService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Check user role
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                
                // If not admin or client, redirect to access denied
                if (userRole != "Admin" && userRole != "Client")
                {
                    return RedirectToAction("AccessDenied", "Account");
                }
                
                // Get latest survey and its most recent responses
                var surveys = await _surveyService.GetAllSurveysAsync();
                var latestSurvey = surveys.OrderByDescending(s => s.CreatedAt).FirstOrDefault();
                
                // Get latest suggestion
                var suggestions = await _suggestionService.GetAllSuggestionsAsync();
                var latestSuggestion = suggestions.OrderByDescending(s => s.CreatedAt).FirstOrDefault();
                
                // Get latest requirement
                var requirements = await _requirementService.GetAllRequirementsAsync();
                var latestRequirement = requirements.OrderByDescending(r => r.CreatedAt).FirstOrDefault();
                
                // Get recent responses, suggestions, and requirements
                var recentResponses = await _surveyService.GetRecentResponsesAsync(5);
                var recentSuggestions = suggestions.OrderByDescending(s => s.CreatedAt).Take(5).ToList();
                var recentRequirements = requirements.OrderByDescending(r => r.CreatedAt).Take(5).ToList();
                
                // Map to view models
                var viewModel = new DashboardViewModel
                {
                    LatestSurvey = latestSurvey != null ? new SurveyListItemViewModel
                    {
                        Id = latestSurvey.Id,
                        Title = latestSurvey.Title,
                        Description = latestSurvey.Description ?? "No description available", // Ensure Description is always set
                        CreatedAt = latestSurvey.CreatedAt,
                        Responses = latestSurvey.Responses,
                        Status = latestSurvey.Status
                    } : null,
                    
                    LatestSuggestion = latestSuggestion != null ? new SuggestionListItemViewModel
                    {
                        Id = latestSuggestion.Id,
                        Content = latestSuggestion.Content,
                        CustomerName = latestSuggestion.CustomerName,
                        CreatedAt = latestSuggestion.CreatedAt,
                        Status = latestSuggestion.Status
                    } : null,
                    
                    LatestRequirement = latestRequirement != null ? new RequirementListItemViewModel
                    {
                        Id = latestRequirement.Id,
                        Title = latestRequirement.Title,
                        Description = latestRequirement.Description,
                        Priority = latestRequirement.Priority,
                        CreatedAt = latestRequirement.CreatedAt,
                        Status = latestRequirement.Status
                    } : null,
                    
                    // Map recent responses
                    RecentSurveyResponses = recentResponses.Select(r => new SurveyResponseItemViewModel
                    {
                        Id = r.Id,
                        SurveyId = r.SurveyId,
                        SurveyTitle = r.SurveyTitle,
                        RespondentName = r.RespondentName,
                        SubmittedAt = r.SubmittedAt
                    }).ToList(),
                    
                    // Map recent suggestions
                    RecentSuggestions = recentSuggestions.Select(s => new SuggestionListItemViewModel
                    {
                        Id = s.Id,
                        Content = s.Content,
                        CustomerName = s.CustomerName,
                        CreatedAt = s.CreatedAt,
                        Status = s.Status
                    }).ToList(),
                    
                    // Map recent requirements
                    RecentRequirements = recentRequirements.Select(r => new RequirementListItemViewModel
                    {
                        Id = r.Id,
                        Title = r.Title,
                        Description = r.Description,
                        Priority = r.Priority,
                        CreatedAt = r.CreatedAt,
                        Status = r.Status
                    }).ToList(),
                    
                    // Add user authentication and role information
                    IsAuthenticated = User.Identity.IsAuthenticated,
                    Username = User.Identity.IsAuthenticated ? User.Identity.Name : string.Empty,
                    UserRole = userRole ?? string.Empty
                };
                
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar el dashboard");
                return View("Error", new ErrorViewModel { RequestId = HttpContext.TraceIdentifier, Message = "Error al cargar el dashboard" });
            }
        }
        
        // Método para actualizar el estado de un elemento
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(string itemType, Guid id, string status)
        {
            try
            {
                switch (itemType.ToLower())
                {
                    case "survey":
                        await _surveyService.UpdateSurveyStatusAsync(id, status);
                        break;
                    case "suggestion":
                        // Convert the string status to SuggestionStatus enum
                        if (Enum.TryParse<SuggestionStatus>(status, true, out var suggestionStatus))
                        {
                            await _suggestionService.UpdateSuggestionStatusAsync(id, suggestionStatus);
                        }
                        else
                        {
                            return BadRequest($"Estado de sugerencia no válido: {status}");
                        }
                        break;
                    case "requirement":
                        // Create a new RequirementStatusUpdateDto to pass to the service
                        var requirementStatusUpdate = new RequirementStatusUpdateDto
                        {
                            Status = status,
                            Response = "", // Changed from Comment to Response to match the DTO property
                            CompletionPercentage = null // Default null percentage
                        };
                        await _requirementService.UpdateRequirementStatusAsync(id, requirementStatusUpdate);
                        break;
                    default:
                        return BadRequest("Tipo de elemento no válido");
                }
                
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el estado");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
