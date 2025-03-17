
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SurveyApp.Application.DTOs;
using SurveyApp.Application.Services;
using SurveyApp.Domain.Entities;

namespace SurveyApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SuggestionsController : ControllerBase
    {
        private readonly ISuggestionService _suggestionService;

        public SuggestionsController(ISuggestionService suggestionService)
        {
            _suggestionService = suggestionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var suggestions = await _suggestionService.GetAllSuggestionsAsync();
            return Ok(suggestions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var suggestion = await _suggestionService.GetSuggestionByIdAsync(id);
            if (suggestion == null)
            {
                return NotFound();
            }
            return Ok(suggestion);
        }

        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetByStatus(string status)
        {
            if (!Enum.TryParse<SuggestionStatus>(status, true, out var suggestionStatus))
            {
                return BadRequest("Invalid status value");
            }
            
            var suggestions = await _suggestionService.GetSuggestionsByStatusAsync(suggestionStatus);
            return Ok(suggestions);
        }

        [HttpGet("category/{category}")]
        public async Task<IActionResult> GetByCategory(string category)
        {
            var suggestions = await _suggestionService.GetSuggestionsByCategoryAsync(category);
            return Ok(suggestions);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string term)
        {
            var suggestions = await _suggestionService.SearchSuggestionsAsync(term);
            return Ok(suggestions);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSuggestionDto suggestionDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var createdSuggestion = await _suggestionService.CreateSuggestionAsync(suggestionDto);
            return CreatedAtAction(nameof(GetById), new { id = createdSuggestion.Id }, createdSuggestion);
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateSuggestionStatusDto statusDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!Enum.TryParse<SuggestionStatus>(statusDto.Status, true, out var status))
            {
                return BadRequest("Invalid status value");
            }
            
            await _suggestionService.UpdateSuggestionStatusAsync(id, status, statusDto.Response);
            return NoContent();
        }

        [HttpGet("find-similar")]
        public async Task<IActionResult> FindSimilar([FromQuery] string content)
        {
            var suggestions = await _suggestionService.FindSimilarSuggestionsAsync(content);
            return Ok(suggestions);
        }

        [HttpGet("monthly-report/{months}")]
        public async Task<IActionResult> GetMonthlyReport(int months)
        {
            if (months <= 0 || months > 24)
            {
                return BadRequest("Months parameter must be between 1 and 24");
            }
            
            var report = await _suggestionService.GenerateMonthlyReportAsync(months);
            return Ok(report);
        }
    }
}
