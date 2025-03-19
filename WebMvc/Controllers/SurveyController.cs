
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SurveyApp.Application.DTOs;
using SurveyApp.Application.Services;
using SurveyApp.WebMvc.Models;

namespace SurveyApp.WebMvc.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class SurveyController : Controller
    {
        private readonly ISurveyService _surveyService;
        private readonly ILogger<SurveyController> _logger;

        public SurveyController(ISurveyService surveyService, ILogger<SurveyController> logger)
        {
            _surveyService = surveyService;
            _logger = logger;
        }

        // GET: Survey
        public async Task<IActionResult> Index()
        {
            try
            {
                // Check user role
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                
                // If not admin, redirect to access denied
                if (userRole != "Admin")
                {
                    return RedirectToAction("AccessDenied", "Account");
                }
                
                var surveys = await _surveyService.GetAllSurveysAsync();
                var viewModel = surveys.Select(MapToListItemViewModel).ToList();
                
                // Set authentication and role information 
                ViewBag.IsAuthenticated = User.Identity.IsAuthenticated;
                ViewBag.UserRole = userRole;
                ViewBag.Username = User.Identity.Name;
                
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar las encuestas");
                return View(new List<SurveyListItemViewModel>());
            }
        }

        // GET: Survey/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                
                // If not admin, redirect to access denied
                if (userRole != "Admin")
                {
                    return RedirectToAction("AccessDenied", "Account");
                }
                
                var survey = await _surveyService.GetSurveyByIdAsync(id);
                if (survey == null)
                {
                    return NotFound();
                }

                var viewModel = MapToViewModel(survey);
                
                // Set authentication and role information
                viewModel.IsAuthenticated = User.Identity.IsAuthenticated;
                viewModel.UserRole = userRole ?? string.Empty;
                viewModel.Username = User.Identity.IsAuthenticated ? User.Identity.Name : string.Empty;
                
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar los detalles de la encuesta");
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Survey/Create
        public IActionResult Create()
        {
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                
            // If not admin, redirect to access denied
            if (userRole != "Admin")
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            
            var viewModel = new SurveyCreateViewModel
            {
                Questions = new List<QuestionViewModel>(),
                IsAuthenticated = User.Identity.IsAuthenticated,
                UserRole = userRole ?? string.Empty,
                Username = User.Identity.IsAuthenticated ? User.Identity.Name : string.Empty
            };
            return View(viewModel);
        }

        // POST: Survey/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SurveyCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                
                    // If not admin, redirect to access denied
                    if (userRole != "Admin")
                    {
                        return RedirectToAction("AccessDenied", "Account");
                    }
                    
                    var surveyDto = new CreateSurveyDto
                    {
                        Title = viewModel.Title,
                        Description = viewModel.Description,
                        Questions = viewModel.Questions?
                            .Select(q => new CreateQuestionDto
                            {
                                Title = q.Title,
                                Description = q.Description,
                                Type = q.Type,
                                Required = q.Required,
                                Options = q.Options ?? new List<string>()
                            })
                            .ToList() ?? new List<CreateQuestionDto>(),
                        DeliveryConfig = MapToDeliveryConfigDto(viewModel.DeliveryConfig)
                    };

                    var createdSurvey = await _surveyService.CreateSurveyAsync(surveyDto);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al crear la encuesta");
                    ModelState.AddModelError("", "Error al crear la encuesta: " + ex.Message);
                }
            }

            // Set authentication and role information if ModelState is invalid
            viewModel.IsAuthenticated = User.Identity.IsAuthenticated;
            viewModel.UserRole = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
            viewModel.Username = User.Identity.IsAuthenticated ? User.Identity.Name : string.Empty;
            
            return View(viewModel);
        }

        // GET: Survey/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                
                // If not admin, redirect to access denied
                if (userRole != "Admin")
                {
                    return RedirectToAction("AccessDenied", "Account");
                }
                
                var survey = await _surveyService.GetSurveyByIdAsync(id);
                if (survey == null)
                {
                    return NotFound();
                }

                var viewModel = MapToCreateSurveyViewModel(survey);
                
                // Set authentication and role information
                viewModel.IsAuthenticated = User.Identity.IsAuthenticated;
                viewModel.UserRole = userRole ?? string.Empty;
                viewModel.Username = User.Identity.IsAuthenticated ? User.Identity.Name : string.Empty;
                
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar la encuesta para editar");
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Survey/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, SurveyCreateViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                
                    // If not admin, redirect to access denied
                    if (userRole != "Admin")
                    {
                        return RedirectToAction("AccessDenied", "Account");
                    }
                    
                    var surveyDto = new CreateSurveyDto
                    {
                        Title = viewModel.Title,
                        Description = viewModel.Description,
                        Questions = viewModel.Questions.Select(q => new CreateQuestionDto
                        {
                            Title = q.Title,
                            Description = q.Description,
                            Type = q.Type,
                            Required = q.Required,
                            Options = q.Options
                        }).ToList(),
                        DeliveryConfig = MapToDeliveryConfigDto(viewModel.DeliveryConfig)
                    };

                    await _surveyService.UpdateSurveyAsync(id, surveyDto);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al actualizar la encuesta");
                    ModelState.AddModelError("", "Error al actualizar la encuesta: " + ex.Message);
                }
            }

            // Set authentication and role information if ModelState is invalid
            viewModel.IsAuthenticated = User.Identity.IsAuthenticated;
            viewModel.UserRole = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
            viewModel.Username = User.Identity.IsAuthenticated ? User.Identity.Name : string.Empty;
            
            return View(viewModel);
        }

        // GET: Survey/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                
                // If not admin, redirect to access denied
                if (userRole != "Admin")
                {
                    return RedirectToAction("AccessDenied", "Account");
                }
                
                var survey = await _surveyService.GetSurveyByIdAsync(id);
                if (survey == null)
                {
                    return NotFound();
                }

                var viewModel = MapToViewModel(survey);
                
                // Set authentication and role information
                viewModel.IsAuthenticated = User.Identity.IsAuthenticated;
                viewModel.UserRole = userRole ?? string.Empty;
                viewModel.Username = User.Identity.IsAuthenticated ? User.Identity.Name : string.Empty;
                
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar la encuesta para eliminar");
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Survey/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
                
                // If not admin, redirect to access denied
                if (userRole != "Admin")
                {
                    return RedirectToAction("AccessDenied", "Account");
                }
                
                await _surveyService.DeleteSurveyAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la encuesta");
                ModelState.AddModelError("", "Error al eliminar la encuesta: " + ex.Message);
                var survey = await _surveyService.GetSurveyByIdAsync(id);
                var viewModel = MapToViewModel(survey);
                
                // Set authentication and role information
                viewModel.IsAuthenticated = User.Identity.IsAuthenticated;
                viewModel.UserRole = User.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
                viewModel.Username = User.Identity.IsAuthenticated ? User.Identity.Name : string.Empty;
                
                return View(viewModel);
            }
        }

        // Métodos auxiliares para mapear entre DTOs y ViewModels
        private SurveyViewModel MapToViewModel(SurveyDto survey)
        {
            return new SurveyViewModel
            {
                Id = survey.Id,
                Title = survey.Title,
                Description = survey.Description,
                CreatedAt = survey.CreatedAt,
                Responses = survey.Responses,
                CompletionRate = survey.CompletionRate,
                Questions = survey.Questions.Select(q => new QuestionViewModel
                {
                    Id = q.Id,
                    Title = q.Title,
                    Description = q.Description,
                    Type = q.Type,
                    Required = q.Required,
                    Options = q.Options
                }).ToList(),
                DeliveryConfig = new DeliveryConfigViewModel
                {
                    Type = survey.DeliveryConfig?.Type ?? "Manual",
                    EmailAddresses = survey.DeliveryConfig?.EmailAddresses ?? new List<string>(),
                    Schedule = new ScheduleViewModel
                    {
                        Frequency = survey.DeliveryConfig?.Schedule?.Frequency ?? "monthly",
                        DayOfMonth = survey.DeliveryConfig?.Schedule?.DayOfMonth ?? 1,
                        DayOfWeek = survey.DeliveryConfig?.Schedule?.DayOfWeek,
                        Time = survey.DeliveryConfig?.Schedule?.Time ?? "09:00",
                        StartDate = survey.DeliveryConfig?.Schedule?.StartDate
                    },
                    Trigger = new TriggerViewModel
                    {
                        Type = survey.DeliveryConfig?.Trigger?.Type ?? "ticket-closed",
                        DelayHours = survey.DeliveryConfig?.Trigger?.DelayHours ?? 24,
                        SendAutomatically = survey.DeliveryConfig?.Trigger?.SendAutomatically ?? false
                    }
                }
            };
        }

        private SurveyCreateViewModel MapToCreateSurveyViewModel(SurveyDto survey)
        {
            return new SurveyCreateViewModel
            {
                Id = survey.Id,
                Title = survey.Title,
                Description = survey.Description,
                Questions = survey.Questions.Select(q => new QuestionViewModel
                {
                    Id = q.Id,
                    Title = q.Title,
                    Description = q.Description,
                    Type = q.Type,
                    Required = q.Required,
                    Options = q.Options
                }).ToList(),
                DeliveryConfig = new DeliveryConfigViewModel
                {
                    Type = survey.DeliveryConfig?.Type ?? "Manual",
                    EmailAddresses = survey.DeliveryConfig?.EmailAddresses ?? new List<string>(),
                    Schedule = new ScheduleViewModel
                    {
                        Frequency = survey.DeliveryConfig?.Schedule?.Frequency ?? "monthly",
                        DayOfMonth = survey.DeliveryConfig?.Schedule?.DayOfMonth ?? 1,
                        DayOfWeek = survey.DeliveryConfig?.Schedule?.DayOfWeek,
                        Time = survey.DeliveryConfig?.Schedule?.Time ?? "09:00",
                        StartDate = survey.DeliveryConfig?.Schedule?.StartDate
                    },
                    Trigger = new TriggerViewModel
                    {
                        Type = survey.DeliveryConfig?.Trigger?.Type ?? "ticket-closed",
                        DelayHours = survey.DeliveryConfig?.Trigger?.DelayHours ?? 24,
                        SendAutomatically = survey.DeliveryConfig?.Trigger?.SendAutomatically ?? false
                    }
                }
            };
        }

        private SurveyListItemViewModel MapToListItemViewModel(SurveyDto survey)
        {
            return new SurveyListItemViewModel
            {
                Id = survey.Id,
                Title = survey.Title,
                Description = survey.Description,
                CreatedAt = survey.CreatedAt,
                ResponseCount = survey.Responses,
                CompletionRate = survey.CompletionRate,
                Status = DeterminarEstado(survey)
            };
        }

        private DeliveryConfigDto MapToDeliveryConfigDto(DeliveryConfigViewModel viewModel)
        {
            if (viewModel == null) return null;
            
            return new DeliveryConfigDto
            {
                Type = viewModel.Type,
                EmailAddresses = viewModel.EmailAddresses,
                Schedule = new ScheduleDto
                {
                    Frequency = viewModel.Schedule?.Frequency,
                    DayOfMonth = viewModel.Schedule?.DayOfMonth,
                    DayOfWeek = viewModel.Schedule?.DayOfWeek,
                    Time = viewModel.Schedule?.Time,
                    StartDate = viewModel.Schedule?.StartDate
                },
                Trigger = new TriggerDto
                {
                    Type = viewModel.Trigger?.Type,
                    DelayHours = viewModel.Trigger?.DelayHours ?? 24,
                    SendAutomatically = viewModel.Trigger?.SendAutomatically ?? false
                }
            };
        }

        private string DeterminarEstado(SurveyDto survey)
        {
            // Aquí puedes implementar lógica para determinar el estado de la encuesta
            // Por ejemplo, basado en fechas, respuestas, etc.
            return "Active"; // Por defecto todas activas
        }
    }
}
