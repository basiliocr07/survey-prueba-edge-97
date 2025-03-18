using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SurveyApp.Application.DTOs;
using SurveyApp.Application.Services;
using SurveyApp.WebMvc.Models;

namespace SurveyApp.WebMvc.Controllers
{
    public class RequirementsController : Controller
    {
        private readonly ISuggestionService _suggestionService;
        private readonly IKnowledgeBaseService _knowledgeBaseService;

        public RequirementsController(ISuggestionService suggestionService, IKnowledgeBaseService knowledgeBaseService)
        {
            _suggestionService = suggestionService;
            _knowledgeBaseService = knowledgeBaseService;
        }

        public async Task<IActionResult> Index()
        {
            var suggestions = await _suggestionService.GetAllSuggestionsAsync();
            var knowledgeBase = await _knowledgeBaseService.GetAllItemsAsync();
            
            var viewModel = new RequirementsViewModel
            {
                Requirements = suggestions,
                KnowledgeBase = knowledgeBase,
                Categories = new[] {
                    "Feature Request", 
                    "Bug Fix", 
                    "Integration", 
                    "Performance", 
                    "Documentation", 
                    "Security", 
                    "Other"
                },
                ProposedCount = suggestions.Count(s => s.Status.ToLower() == "proposed" || s.Status.ToLower() == "new"),
                InProgressCount = suggestions.Count(s => s.Status.ToLower() == "in progress"),
                TestingCount = suggestions.Count(s => s.Status.ToLower() == "testing"),
                CompletedCount = suggestions.Count(s => s.Status.ToLower() == "completed"),
                TotalCount = suggestions.Count,
                
                CriticalCount = suggestions.Count(s => s.Priority.ToLower() == "critical"),
                HighCount = suggestions.Count(s => s.Priority.ToLower() == "high"),
                MediumCount = suggestions.Count(s => s.Priority.ToLower() == "medium"),
                LowCount = suggestions.Count(s => s.Priority.ToLower() == "low")
            };
            
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Submit(CreateSuggestionDto requirementDto)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please correct the errors in the form.";
                return RedirectToAction(nameof(Index));
            }

            await _suggestionService.CreateSuggestionAsync(requirementDto);
            TempData["SuccessMessage"] = "Your requirement has been submitted successfully.";
            
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("requirements/view")]
        public async Task<IActionResult> ViewRequirements(string category = null, string search = null)
        {
            var requirements = await _suggestionService.GetAllSuggestionsAsync();
            
            // Apply filters if provided
            if (!string.IsNullOrEmpty(category))
            {
                requirements = await _suggestionService.GetSuggestionsByCategoryAsync(category);
            }
            
            if (!string.IsNullOrEmpty(search))
            {
                requirements = await _suggestionService.SearchSuggestionsAsync(search);
            }
            
            var viewModel = new RequirementsListViewModel
            {
                Requirements = requirements,
                Categories = new[] {
                    "Feature Request", 
                    "Bug Fix", 
                    "Integration", 
                    "Performance", 
                    "Documentation", 
                    "Security", 
                    "Other"
                },
                CategoryFilter = category,
                SearchTerm = search
            };
            
            return View(viewModel);
        }

        [HttpGet("requirements/knowledge")]
        public async Task<IActionResult> KnowledgeBase(string search = null)
        {
            var knowledgeBaseItems = string.IsNullOrEmpty(search)
                ? await _knowledgeBaseService.GetAllItemsAsync()
                : await _knowledgeBaseService.SearchItemsAsync(search);
            
            var viewModel = new KnowledgeBaseViewModel
            {
                KnowledgeBaseItems = knowledgeBaseItems,
                SearchTerm = search
            };
            
            return View(viewModel);
        }
        
        [HttpGet("requirements/similar")]
        public async Task<IActionResult> FindSimilar(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return Json(new { success = false, message = "Content is required" });
            }
            
            var requirements = await _suggestionService.FindSimilarSuggestionsAsync(content);
            var knowledgeItems = await _knowledgeBaseService.SearchItemsAsync(content);
            
            return Json(new
            {
                success = true,
                requirements,
                knowledgeItems
            });
        }
    }
}
