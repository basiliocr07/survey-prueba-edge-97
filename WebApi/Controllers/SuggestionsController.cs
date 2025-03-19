using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly ICustomerService _customerService;
        private readonly ILogger<SuggestionsController> _logger;

        public SuggestionsController(
            ISuggestionService suggestionService,
            ICustomerService customerService,
            ILogger<SuggestionsController> logger)
        {
            _suggestionService = suggestionService;
            _customerService = customerService;
            _logger = logger;
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
                _logger.LogWarning("Modelo inválido al crear sugerencia. Errores: {@Errors}", 
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(ModelState);
            }
            
            try
            {
                // Verificar si existe un cliente con este email
                if (!string.IsNullOrWhiteSpace(suggestionDto.CustomerEmail) && !suggestionDto.IsAnonymous)
                {
                    try
                    {
                        var customers = await _customerService.GetCustomerByEmailAsync(suggestionDto.CustomerEmail);
                        var existingCustomer = customers.Count > 0 ? customers[0] : null;
                        
                        if (existingCustomer != null)
                        {
                            _logger.LogInformation("Cliente existente encontrado para sugerencia: {CustomerId}", existingCustomer.Id);
                            suggestionDto.CustomerId = existingCustomer.Id;
                        }
                        else if (!string.IsNullOrWhiteSpace(suggestionDto.CustomerCompany))
                        {
                            // Crear nuevo cliente si proporcionó info de compañía
                            var newCustomer = new CreateCustomerDto
                            {
                                BrandName = suggestionDto.CustomerCompany,
                                ContactEmail = suggestionDto.CustomerEmail,
                                ContactPhone = suggestionDto.CustomerPhone,
                                ContactName = suggestionDto.CustomerName,
                                AcquiredServices = new List<string> { "Suggestion" }
                            };
                            
                            var createdCustomer = await _customerService.CreateCustomerAsync(newCustomer);
                            _logger.LogInformation("Nuevo cliente creado para sugerencia: {CustomerId}", createdCustomer.Id);
                            suggestionDto.CustomerId = createdCustomer.Id;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error al verificar cliente existente: {Email}", suggestionDto.CustomerEmail);
                        // Continuamos aunque no podamos verificar el cliente
                    }
                }
                
                var createdSuggestion = await _suggestionService.CreateSuggestionAsync(suggestionDto);
                _logger.LogInformation("Sugerencia API creada exitosamente: {Id}", createdSuggestion.Id);
                
                return CreatedAtAction(nameof(GetById), new { id = createdSuggestion.Id }, createdSuggestion);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear sugerencia API");
                return BadRequest(new { message = ex.Message });
            }
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
