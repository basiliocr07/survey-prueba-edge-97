
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SurveyApp.Application.Services;
using SurveyApp.WebMvc.Models;

namespace SurveyApp.WebMvc.Controllers
{
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
                        Description = latestSurvey.Description,
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
                    }).ToList()
                };
                
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar el dashboard");
                return View("Error", new ErrorViewModel { Message = "Error al cargar el dashboard" });
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
                        await _suggestionService.UpdateSuggestionStatusAsync(id, status);
                        break;
                    case "requirement":
                        await _requirementService.UpdateRequirementStatusAsync(id, status);
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
