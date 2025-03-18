
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SurveyApp.Application.DTOs;
using SurveyApp.Application.Services;
using SurveyApp.Domain.Entities;
using SurveyApp.WebMvc.Models;

namespace SurveyApp.WebMvc.Controllers
{
    public class SuggestionsController : Controller
    {
        private readonly ISuggestionService _suggestionService;
        private readonly IKnowledgeBaseService _knowledgeBaseService;

        public SuggestionsController(ISuggestionService suggestionService, IKnowledgeBaseService knowledgeBaseService)
        {
            _suggestionService = suggestionService;
            _knowledgeBaseService = knowledgeBaseService;
        }

        public async Task<IActionResult> Index()
        {
            var suggestions = await _suggestionService.GetAllSuggestionsAsync();
            
            var viewModel = new SuggestionIndexViewModel
            {
                Suggestions = suggestions,
                TotalSuggestions = suggestions.Count,
                NewSuggestions = suggestions.Count(s => s.Status.ToLower() == "new"),
                InProgressSuggestions = suggestions.Count(s => s.Status.ToLower() == "in progress"),
                CompletedSuggestions = suggestions.Count(s => s.Status.ToLower() == "completed"),
                Categories = new[] {
                    "UI/UX", 
                    "Features", 
                    "Performance", 
                    "Integrations", 
                    "Reporting", 
                    "Mobile App", 
                    "Other"
                }
            };
            
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSuggestionDto suggestionDto)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please correct the errors in the form.";
                return RedirectToAction(nameof(Index));
            }

            await _suggestionService.CreateSuggestionAsync(suggestionDto);
            TempData["SuccessMessage"] = "Your suggestion has been submitted successfully.";
            
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("suggestions/view")]
        public async Task<IActionResult> View(string status = null, string category = null, string search = null)
        {
            var suggestions = await _suggestionService.GetAllSuggestionsAsync();
            
            // Apply filters if provided
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<SuggestionStatus>(status, true, out var statusFilter))
            {
                suggestions = await _suggestionService.GetSuggestionsByStatusAsync(statusFilter);
            }
            
            if (!string.IsNullOrEmpty(category))
            {
                suggestions = await _suggestionService.GetSuggestionsByCategoryAsync(category);
            }
            
            if (!string.IsNullOrEmpty(search))
            {
                suggestions = await _suggestionService.SearchSuggestionsAsync(search);
            }
            
            var viewModel = new SuggestionListViewModel
            {
                Suggestions = suggestions,
                Categories = new[] {
                    "UI/UX", 
                    "Features", 
                    "Performance", 
                    "Integrations", 
                    "Reporting", 
                    "Mobile App", 
                    "Other"
                },
                StatusFilter = status,
                CategoryFilter = category,
                SearchTerm = search
            };
            
            return View(viewModel);
        }

        [HttpGet("suggestions/reports")]
        public async Task<IActionResult> Reports(int months = 3)
        {
            if (months <= 0 || months > 12)
            {
                months = 3;
            }
            
            var report = await _suggestionService.GenerateMonthlyReportAsync(months);
            
            var viewModel = new SuggestionReportViewModel
            {
                Report = report,
                MonthsRange = months
            };
            
            return View(viewModel);
        }

        [HttpPost("suggestions/{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, UpdateSuggestionStatusDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please provide valid status information.";
                return RedirectToAction(nameof(View));
            }

            if (!Enum.TryParse<SuggestionStatus>(updateDto.Status, true, out var status))
            {
                TempData["ErrorMessage"] = "Invalid status value.";
                return RedirectToAction(nameof(View));
            }

            await _suggestionService.UpdateSuggestionStatusAsync(id, status, updateDto.Response);
            TempData["SuccessMessage"] = "Suggestion status updated successfully.";
            
            return RedirectToAction(nameof(View));
        }
        
        [HttpGet("suggestions/similar")]
        public async Task<IActionResult> FindSimilar(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                return Json(new { success = false, message = "Content is required" });
            }
            
            var suggestions = await _suggestionService.FindSimilarSuggestionsAsync(content);
            var knowledgeItems = await _knowledgeBaseService.SearchItemsAsync(content);
            
            return Json(new
            {
                success = true,
                suggestions,
                knowledgeItems
            });
        }
    }
}
