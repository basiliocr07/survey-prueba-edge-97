
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SurveyApp.Application.DTOs;
using SurveyApp.Application.Services;
using SurveyApp.Domain.Entities;
using SurveyApp.WebMvc.Models;

namespace SurveyApp.WebMvc.Controllers
{
    [Authorize(Policy = "ClientAccess")]
    public class SuggestionsController : Controller
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

        public async Task<IActionResult> Index()
        {
            // Solo administradores pueden ver el listado completo
            if (!User.IsInRole("Admin"))
            {
                return RedirectToAction("Create");
            }

            try
            {
                var suggestions = await _suggestionService.GetAllSuggestionsAsync();
                var viewModel = new SuggestionListViewModel
                {
                    Suggestions = suggestions
                };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las sugerencias");
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewData["PageTitle"] = User.IsInRole("Admin") ? 
                "Gestión de Sugerencias" : "Enviar Sugerencia";
            return View(new CreateSuggestionViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSuggestionViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar si ya existe un cliente con este email
                    CustomerDto existingCustomer = null;
                    try
                    {
                        var customers = await _customerService.GetCustomerByEmailAsync(model.CustomerEmail);
                        existingCustomer = customers.Count > 0 ? customers[0] : null;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Error al buscar cliente por email: {Email}", model.CustomerEmail);
                        // Continuamos el proceso aunque no encontremos cliente
                    }

                    // Si no existe el cliente y se proporcionó información de compañía, crear nuevo cliente
                    if (existingCustomer == null && !string.IsNullOrWhiteSpace(model.Company))
                    {
                        try
                        {
                            var newCustomer = new CreateCustomerDto
                            {
                                BrandName = model.Company,
                                ContactEmail = model.CustomerEmail,
                                ContactPhone = model.Phone,
                                ContactName = model.CustomerName,
                                AcquiredServices = new List<string> { "Suggestion" }
                            };
                            
                            existingCustomer = await _customerService.CreateCustomerAsync(newCustomer);
                            _logger.LogInformation("Nuevo cliente creado: {CustomerId}", existingCustomer.Id);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error al crear un nuevo cliente");
                            // No falla el proceso si no se puede crear el cliente
                        }
                    }

                    var dto = new CreateSuggestionDto
                    {
                        Content = model.Content,
                        CustomerName = model.IsAnonymous ? "Anonymous" : model.CustomerName,
                        CustomerEmail = model.CustomerEmail,
                        Category = model.Category ?? "Other",
                        IsAnonymous = model.IsAnonymous,
                        // Asignamos valores adicionales útiles para el seguimiento
                        Title = model.Content.Length > 50 ? model.Content.Substring(0, 47) + "..." : model.Content,
                        Description = model.Content,
                        Source = existingCustomer != null ? "ExistingCustomer" : "WebForm",
                        // Guardamos información de contacto adicional
                        CustomerPhone = model.Phone,
                        CustomerCompany = model.Company
                    };

                    if (existingCustomer != null)
                    {
                        dto.CustomerId = existingCustomer.Id;
                    }

                    await _suggestionService.CreateSuggestionAsync(dto);

                    _logger.LogInformation("Sugerencia creada exitosamente por {CustomerName}, Email: {CustomerEmail}, Categoría: {Category}",
                        model.CustomerName, model.CustomerEmail, model.Category);

                    TempData["SuccessMessage"] = "Sugerencia enviada correctamente.";
                    return RedirectToAction(User.IsInRole("Admin") ? "Index" : "Create");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al crear la sugerencia");
                    ModelState.AddModelError("", "Ocurrió un error al enviar la sugerencia.");
                }
            }
            else
            {
                _logger.LogWarning("Modelo inválido al crear sugerencia. Errores: {@Errors}", 
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
            }

            ViewData["PageTitle"] = User.IsInRole("Admin") ? 
                "Gestión de Sugerencias" : "Enviar Sugerencia";
            return View(model);
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> View(Guid id)
        {
            try
            {
                var suggestion = await _suggestionService.GetSuggestionByIdAsync(id);
                if (suggestion == null)
                {
                    return NotFound();
                }

                var viewModel = new SuggestionDetailViewModel
                {
                    Id = suggestion.Id,
                    Content = suggestion.Content,
                    CustomerName = suggestion.CustomerName,
                    CustomerEmail = suggestion.CustomerEmail,
                    CreatedAt = suggestion.CreatedAt,
                    Status = suggestion.Status.ToString(),
                    Category = suggestion.Category,
                    Response = suggestion.Response
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la sugerencia");
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> UpdateStatus(Guid id, string status, string response)
        {
            try
            {
                if (!Enum.TryParse<SuggestionStatus>(status, out var suggestionStatus))
                {
                    ModelState.AddModelError("", "Estado de sugerencia no válido.");
                    return RedirectToAction("View", new { id });
                }

                await _suggestionService.UpdateSuggestionStatusAsync(id, suggestionStatus, response);

                TempData["SuccessMessage"] = "Estado de la sugerencia actualizado correctamente.";
                return RedirectToAction("View", new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el estado de la sugerencia");
                TempData["ErrorMessage"] = "Ocurrió un error al actualizar el estado de la sugerencia.";
                return RedirectToAction("View", new { id });
            }
        }

        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Reports()
        {
            try
            {
                var suggestions = await _suggestionService.GetAllSuggestionsAsync();
                var viewModel = new SuggestionReportsViewModel
                {
                    Suggestions = suggestions
                };
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los reportes de sugerencias");
                return View("Error");
            }
        }
    }
}
