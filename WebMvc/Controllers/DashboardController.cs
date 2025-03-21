
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
            var logReference = Guid.NewGuid().ToString("N").Substring(0, 8);
            _logger.LogInformation("[Ref:{LogRef}] Iniciando carga del dashboard", logReference);
            
            try
            {
                // Check user role
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                var username = User.Identity?.Name;
                
                _logger.LogInformation("[Ref:{LogRef}] Usuario {Username} con rol {Role} accediendo al dashboard", 
                    logReference, username, userRole);
                
                // If not admin or client, redirect to access denied
                if (userRole != "Admin" && userRole != "Client")
                {
                    _logger.LogWarning("[Ref:{LogRef}] Acceso denegado a usuario {Username} con rol {Role}", 
                        logReference, username, userRole);
                    return RedirectToAction("AccessDenied", "Account");
                }
                
                // Get latest survey and its most recent responses
                _logger.LogDebug("[Ref:{LogRef}] Obteniendo encuestas", logReference);
                var surveys = await _surveyService.GetAllSurveysAsync();
                var latestSurvey = surveys.OrderByDescending(s => s.CreatedAt).FirstOrDefault();
                
                if (latestSurvey != null)
                {
                    _logger.LogDebug("[Ref:{LogRef}] Última encuesta encontrada: {SurveyId} - {SurveyTitle}", 
                        logReference, latestSurvey.Id, latestSurvey.Title);
                }
                else
                {
                    _logger.LogInformation("[Ref:{LogRef}] No se encontraron encuestas", logReference);
                }
                
                // Get latest suggestion
                _logger.LogDebug("[Ref:{LogRef}] Obteniendo sugerencias", logReference);
                var suggestions = await _suggestionService.GetAllSuggestionsAsync();
                var latestSuggestion = suggestions.OrderByDescending(s => s.CreatedAt).FirstOrDefault();
                
                if (latestSuggestion != null)
                {
                    _logger.LogDebug("[Ref:{LogRef}] Última sugerencia encontrada: {SuggestionId}", 
                        logReference, latestSuggestion.Id);
                }
                
                // Get latest requirement
                _logger.LogDebug("[Ref:{LogRef}] Obteniendo requerimientos", logReference);
                var requirements = await _requirementService.GetAllRequirementsAsync();
                var latestRequirement = requirements.OrderByDescending(r => r.CreatedAt).FirstOrDefault();
                
                if (latestRequirement != null)
                {
                    _logger.LogDebug("[Ref:{LogRef}] Último requerimiento encontrado: {RequirementId} - {RequirementTitle}", 
                        logReference, latestRequirement.Id, latestRequirement.Title);
                }
                
                // Get recent responses, suggestions, and requirements
                _logger.LogDebug("[Ref:{LogRef}] Obteniendo respuestas recientes", logReference);
                var recentResponses = await _surveyService.GetRecentResponsesAsync(5);
                var recentSuggestions = suggestions.OrderByDescending(s => s.CreatedAt).Take(5).ToList();
                var recentRequirements = requirements.OrderByDescending(r => r.CreatedAt).Take(5).ToList();
                
                _logger.LogDebug("[Ref:{LogRef}] Respuestas recientes: {Count}, Sugerencias recientes: {SuggestionsCount}, Requerimientos recientes: {RequirementsCount}", 
                    logReference, recentResponses.Count, recentSuggestions.Count, recentRequirements.Count);
                
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
                
                _logger.LogInformation("[Ref:{LogRef}] Dashboard cargado exitosamente para el usuario {Username}", 
                    logReference, username);
                
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Ref:{LogRef}] Error al cargar el dashboard: {Message}", logReference, ex.Message);
                
                // Log stack trace for detailed debugging
                _logger.LogDebug("[Ref:{LogRef}] Stack trace: {StackTrace}", logReference, ex.StackTrace);
                
                if (ex.InnerException != null)
                {
                    _logger.LogDebug("[Ref:{LogRef}] Inner exception: {InnerExceptionMessage}", 
                        logReference, ex.InnerException.Message);
                }
                
                return View("Error", new ErrorViewModel { 
                    RequestId = HttpContext.TraceIdentifier, 
                    Message = "Error al cargar el dashboard: " + ex.Message,
                    Exception = ex,
                    LogReference = logReference,
                    IsAuthenticated = User.Identity.IsAuthenticated,
                    Username = User.Identity.IsAuthenticated ? User.Identity.Name : string.Empty,
                    UserRole = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty
                });
            }
        }
        
        // Método para actualizar el estado de un elemento
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(string itemType, Guid id, string status)
        {
            var logReference = Guid.NewGuid().ToString("N").Substring(0, 8);
            _logger.LogInformation("[Ref:{LogRef}] Actualizando estado de {ItemType} con ID {Id} a {Status}", 
                logReference, itemType, id, status);
            
            try
            {
                switch (itemType.ToLower())
                {
                    case "survey":
                        _logger.LogDebug("[Ref:{LogRef}] Actualizando estado de encuesta {Id} a {Status}", 
                            logReference, id, status);
                        await _surveyService.UpdateSurveyStatusAsync(id, status);
                        break;
                    case "suggestion":
                        // Convert the string status to SuggestionStatus enum
                        if (Enum.TryParse<SuggestionStatus>(status, true, out var suggestionStatus))
                        {
                            _logger.LogDebug("[Ref:{LogRef}] Actualizando estado de sugerencia {Id} a {Status}", 
                                logReference, id, suggestionStatus);
                            await _suggestionService.UpdateSuggestionStatusAsync(id, suggestionStatus);
                        }
                        else
                        {
                            _logger.LogWarning("[Ref:{LogRef}] Estado de sugerencia no válido: {Status} para ID {Id}", 
                                logReference, status, id);
                            return BadRequest($"Estado de sugerencia no válido: {status}");
                        }
                        break;
                    case "requirement":
                        _logger.LogDebug("[Ref:{LogRef}] Actualizando estado de requerimiento {Id} a {Status}", 
                            logReference, id, status);
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
                        _logger.LogWarning("[Ref:{LogRef}] Tipo de elemento no válido: {ItemType}", 
                            logReference, itemType);
                        return BadRequest("Tipo de elemento no válido");
                }
                
                _logger.LogInformation("[Ref:{LogRef}] Estado actualizado exitosamente: {ItemType} {Id} a {Status}", 
                    logReference, itemType, id, status);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Ref:{LogRef}] Error al actualizar el estado: {ItemType} {Id} a {Status}. Error: {Message}", 
                    logReference, itemType, id, status, ex.Message);
                
                if (ex.InnerException != null)
                {
                    _logger.LogDebug("[Ref:{LogRef}] Inner exception: {InnerExceptionMessage}", 
                        logReference, ex.InnerException.Message);
                }
                
                return StatusCode(500, new { success = false, message = ex.Message, logReference = logReference });
            }
        }
    }
}
