
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SurveyApp.Application.DTOs;
using SurveyApp.Application.Services;
using SurveyApp.Domain.Entities;
using System.Collections.Generic;

namespace SurveyApp.WebMvc.Controllers
{
    public class SuggestionsController : Controller
    {
        private readonly ISuggestionService _suggestionService;
        private readonly ILogger<SuggestionsController> _logger;

        public SuggestionsController(
            ISuggestionService suggestionService,
            ILogger<SuggestionsController> logger)
        {
            _suggestionService = suggestionService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Obtener datos de sugerencias (mock para demostración)
                var suggestions = await GetMockSuggestions();
                
                // Calcular contadores para el dashboard
                var totalCount = suggestions.Count;
                var newCount = suggestions.Count(s => s.Status.ToLower() == "new");
                var inProgressCount = suggestions.Count(s => s.Status.ToLower() == "reviewed");
                var completedCount = suggestions.Count(s => s.Status.ToLower() == "implemented");
                
                ViewData["IsAdmin"] = GetIsAdminFromUser();
                
                return View(new SuggestionsViewModel
                {
                    Suggestions = suggestions,
                    TotalCount = totalCount,
                    NewCount = newCount,
                    InProgressCount = inProgressCount,
                    CompletedCount = completedCount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener sugerencias");
                return View("Error");
            }
        }
        
        private bool GetIsAdminFromUser()
        {
            // En una implementación real, esto verificaría los roles del usuario
            // Por ahora, usamos un valor de demostración
            return true; // User.IsInRole("Admin");
        }
        
        private async Task<List<SuggestionDto>> GetMockSuggestions()
        {
            // Datos mockeados para demostración - en una implementación real, vendrían del servicio
            return new List<SuggestionDto>
            {
                new SuggestionDto
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                    Title = "Add dark mode to the dashboard",
                    Content = "It would be great to have a dark mode option for the dashboard to reduce eye strain when working at night.",
                    CustomerName = "John Doe",
                    CustomerEmail = "john@example.com",
                    CreatedAt = DateTime.Parse("2023-06-15T10:30:00Z"),
                    Status = "implemented",
                    Category = "UI/UX",
                    IsAnonymous = false,
                    Response = "Great suggestion! We've implemented dark mode and it will be available in the next release.",
                    ResponseDate = DateTime.Parse("2023-06-20T14:15:00Z"),
                    CompletionPercentage = 100
                },
                new SuggestionDto
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                    Title = "Improve mobile responsiveness",
                    Content = "The application could be more responsive on mobile devices, especially the survey form.",
                    CustomerName = "Jane Smith",
                    CustomerEmail = "jane@example.com",
                    CreatedAt = DateTime.Parse("2023-06-17T15:45:00Z"),
                    Status = "reviewed",
                    Category = "Mobile App",
                    IsAnonymous = false,
                    Response = "We're currently working on improving mobile responsiveness. Thank you for your feedback!",
                    ResponseDate = DateTime.Parse("2023-06-21T09:20:00Z"),
                    CompletionPercentage = 50
                },
                new SuggestionDto
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                    Title = "Add export to PDF option",
                    Content = "Please add an option to export survey results as PDF files for easier sharing.",
                    CustomerName = "Anonymous",
                    CustomerEmail = "anonymous@example.com",
                    CreatedAt = DateTime.Parse("2023-06-19T12:10:00Z"),
                    Status = "new",
                    Category = "Features",
                    IsAnonymous = true,
                    CompletionPercentage = 0
                },
                new SuggestionDto
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000004"),
                    Title = "Fix login issues on Firefox",
                    Content = "There are some login issues when using the Firefox browser. The login button sometimes doesn't respond.",
                    CustomerName = "Carlos Rodriguez",
                    CustomerEmail = "carlos@example.com",
                    CreatedAt = DateTime.Parse("2023-06-20T08:30:00Z"),
                    Status = "rejected",
                    Category = "Bug",
                    IsAnonymous = false,
                    Response = "We were unable to reproduce this issue after testing on multiple Firefox versions. Please provide more details if possible.",
                    ResponseDate = DateTime.Parse("2023-06-22T11:05:00Z"),
                    CompletionPercentage = 100
                },
                new SuggestionDto
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000005"),
                    Title = "Add integration with Google Forms",
                    Content = "It would be helpful to have an integration with Google Forms to import existing surveys.",
                    CustomerName = "Sarah Johnson",
                    CustomerEmail = "sarah@example.com",
                    CreatedAt = DateTime.Parse("2023-06-21T16:20:00Z"),
                    Status = "new",
                    Category = "Integrations",
                    IsAnonymous = false,
                    CompletionPercentage = 0
                }
            };
        }
    }
    
    public class SuggestionsViewModel
    {
        public List<SuggestionDto> Suggestions { get; set; }
        public int TotalCount { get; set; }
        public int NewCount { get; set; }
        public int InProgressCount { get; set; }
        public int CompletedCount { get; set; }
    }
}
