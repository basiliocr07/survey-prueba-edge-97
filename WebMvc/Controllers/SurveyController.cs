using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SurveyApp.Application.DTOs;
using SurveyApp.Application.Services;
using SurveyApp.WebMvc.Models;
using System.Net.Mail;
using System.Collections.Generic;

namespace SurveyApp.WebMvc.Controllers
{
    public class SurveyController : Controller
    {
        private readonly ISurveyService _surveyService;
        private readonly ILogger<SurveyController> _logger;

        public SurveyController(ISurveyService surveyService, ILogger<SurveyController> logger)
        {
            _surveyService = surveyService ?? throw new ArgumentNullException(nameof(surveyService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            _logger.LogInformation("SurveyController initialized");
        }

        // GET: /Survey
        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("SurveyController.Index action invoked");
            
            try
            {
                var surveys = await _surveyService.GetAllSurveysAsync();
                _logger.LogInformation("Retrieved {Count} surveys for Index view", surveys.Count);
                return View(surveys);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving surveys for Index view: {ErrorMessage}", ex.Message);
                TempData["ErrorMessage"] = "Error retrieving surveys. Please try again later.";
                return View(new List<SurveyDto>());
            }
        }

        // GET: /Survey/Create
        public IActionResult Create()
        {
            _logger.LogInformation("SurveyController.Create GET action invoked");
            
            var viewModel = new CreateSurveyViewModel
            {
                Questions = new System.Collections.Generic.List<QuestionViewModel>
                {
                    new QuestionViewModel
                    {
                        Type = "SingleChoice",
                        Required = true,
                        Options = new System.Collections.Generic.List<string> { "Option 1", "Option 2", "Option 3" }
                    }
                },
                DeliveryConfig = new DeliveryConfigViewModel
                {
                    Type = "Manual",
                    EmailAddresses = new System.Collections.Generic.List<string>(),
                    Schedule = new ScheduleViewModel
                    {
                        Frequency = "monthly",
                        DayOfMonth = 1,
                        Time = "09:00"
                    },
                    Trigger = new TriggerViewModel
                    {
                        Type = "ticket-closed",
                        DelayHours = 24,
                        SendAutomatically = false
                    }
                }
            };
            
            _logger.LogDebug("Initialized Create view model with default question and delivery config");
            return View(viewModel);
        }

        // POST: /Survey/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSurveyViewModel model)
        {
            _logger.LogInformation("SurveyController.Create POST action invoked with title: {Title}", model?.Title);
            
            if (model == null)
            {
                _logger.LogWarning("Create survey model is null");
                ModelState.AddModelError("", "No survey data provided");
                return View(new CreateSurveyViewModel());
            }
            
            _logger.LogDebug("Create survey request with {QuestionCount} questions", model.Questions?.Count ?? 0);
            
            if (ModelState.IsValid)
            {
                try
                {
                    _logger.LogInformation("Creating survey: {Title}", model.Title);
                    
                    var createSurveyDto = new CreateSurveyDto
                    {
                        Title = model.Title,
                        Description = model.Description,
                        Questions = model.Questions?.ConvertAll(q => new CreateQuestionDto
                        {
                            Title = q.Title,
                            Description = q.Description,
                            Type = q.Type,
                            Required = q.Required,
                            Options = q.Options
                        }) ?? new List<CreateQuestionDto>(),
                        DeliveryConfig = new DeliveryConfigDto
                        {
                            Type = model.DeliveryConfig?.Type ?? "Manual",
                            EmailAddresses = model.DeliveryConfig?.EmailAddresses ?? new List<string>(),
                            Schedule = model.DeliveryConfig?.Schedule != null ? new ScheduleDto
                            {
                                Frequency = model.DeliveryConfig.Schedule.Frequency,
                                DayOfMonth = model.DeliveryConfig.Schedule.DayOfMonth,
                                DayOfWeek = model.DeliveryConfig.Schedule.DayOfWeek,
                                Time = model.DeliveryConfig.Schedule.Time,
                                StartDate = model.DeliveryConfig.Schedule.StartDate
                            } : new ScheduleDto(),
                            Trigger = model.DeliveryConfig?.Trigger != null ? new TriggerDto
                            {
                                Type = model.DeliveryConfig.Trigger.Type,
                                DelayHours = model.DeliveryConfig.Trigger.DelayHours,
                                SendAutomatically = model.DeliveryConfig.Trigger.SendAutomatically
                            } : new TriggerDto()
                        }
                    };

                    var survey = await _surveyService.CreateSurveyAsync(createSurveyDto);
                    _logger.LogInformation("Survey created successfully with ID: {SurveyId}", survey.Id);
                    
                    TempData["SuccessMessage"] = "Survey created successfully!";
                    return RedirectToAction(nameof(Details), new { id = survey.Id });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating survey: {ErrorMessage}", ex.Message);
                    ModelState.AddModelError("", $"Error creating survey: {ex.Message}");
                }
            }
            else
            {
                _logger.LogWarning("Invalid model state when creating survey. Errors: {Errors}", 
                    string.Join("; ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)));
            }

            return View(model);
        }

        // GET: /Survey/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            _logger.LogInformation("Edit action invoked for survey ID: {SurveyId}", id);
            try
            {
                var survey = await _surveyService.GetSurveyByIdAsync(id);
                if (survey == null)
                {
                    _logger.LogWarning("Survey not found for ID: {SurveyId}", id);
                    return NotFound();
                }

                var viewModel = new CreateSurveyViewModel
                {
                    Title = survey.Title,
                    Description = survey.Description,
                    Questions = survey.Questions.ConvertAll(q => new QuestionViewModel
                    {
                        Title = q.Title,
                        Description = q.Description,
                        Type = q.Type,
                        Required = q.Required,
                        Options = q.Options
                    }),
                    DeliveryConfig = new DeliveryConfigViewModel
                    {
                        Type = survey.DeliveryConfig.Type,
                        EmailAddresses = survey.DeliveryConfig.EmailAddresses,
                        Schedule = new ScheduleViewModel
                        {
                            Frequency = survey.DeliveryConfig.Schedule.Frequency,
                            DayOfMonth = survey.DeliveryConfig.Schedule.DayOfMonth,
                            DayOfWeek = survey.DeliveryConfig.Schedule.DayOfWeek,
                            Time = survey.DeliveryConfig.Schedule.Time,
                            StartDate = survey.DeliveryConfig.Schedule.StartDate
                        },
                        Trigger = new TriggerViewModel
                        {
                            Type = survey.DeliveryConfig.Trigger.Type,
                            DelayHours = survey.DeliveryConfig.Trigger.DelayHours,
                            SendAutomatically = survey.DeliveryConfig.Trigger.SendAutomatically
                        }
                    }
                };
                return View(viewModel);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Survey not found: {SurveyId}", id);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving survey for edit: {SurveyId}", id);
                TempData["ErrorMessage"] = "Error retrieving survey. Please try again later.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Survey/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CreateSurveyViewModel model)
        {
            _logger.LogInformation("Edit POST action invoked for survey ID: {SurveyId}", id);
            if (ModelState.IsValid)
            {
                try
                {
                    var updateSurveyDto = new CreateSurveyDto
                    {
                        Title = model.Title,
                        Description = model.Description,
                        Questions = model.Questions.ConvertAll(q => new CreateQuestionDto
                        {
                            Title = q.Title,
                            Description = q.Description,
                            Type = q.Type,
                            Required = q.Required,
                            Options = q.Options
                        }),
                        DeliveryConfig = new DeliveryConfigDto
                        {
                            Type = model.DeliveryConfig.Type,
                            EmailAddresses = model.DeliveryConfig.EmailAddresses,
                            Schedule = new ScheduleDto
                            {
                                Frequency = model.DeliveryConfig.Schedule.Frequency,
                                DayOfMonth = model.DeliveryConfig.Schedule.DayOfMonth,
                                DayOfWeek = model.DeliveryConfig.Schedule.DayOfWeek,
                                Time = model.DeliveryConfig.Schedule.Time,
                                StartDate = model.DeliveryConfig.Schedule.StartDate
                            },
                            Trigger = new TriggerDto
                            {
                                Type = model.DeliveryConfig.Trigger.Type,
                                DelayHours = model.DeliveryConfig.Trigger.DelayHours,
                                SendAutomatically = model.DeliveryConfig.Trigger.SendAutomatically
                            }
                        }
                    };

                    await _surveyService.UpdateSurveyAsync(id, updateSurveyDto);
                    TempData["SuccessMessage"] = "Survey updated successfully!";
                    return RedirectToAction(nameof(Details), new { id });
                }
                catch (KeyNotFoundException ex)
                {
                    _logger.LogWarning(ex, "Survey not found during update: {SurveyId}", id);
                    return NotFound();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating survey: {SurveyId}", id);
                    ModelState.AddModelError("", $"Error updating survey: {ex.Message}");
                }
            }

            return View(model);
        }

        // GET: /Survey/Details/{id}
        public async Task<IActionResult> Details(Guid id)
        {
            _logger.LogInformation("Details action invoked for survey ID: {SurveyId}", id);
            try
            {
                var survey = await _surveyService.GetSurveyByIdAsync(id);
                if (survey == null)
                {
                    _logger.LogWarning("Survey not found for ID: {SurveyId}", id);
                    return NotFound();
                }
                return View(survey);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Survey not found: {SurveyId}", id);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving survey details: {SurveyId}", id);
                TempData["ErrorMessage"] = "Error retrieving survey details. Please try again later.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: /Survey/Delete/{id}
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation("Delete action invoked for survey ID: {SurveyId}", id);
            try
            {
                var survey = await _surveyService.GetSurveyByIdAsync(id);
                if (survey == null)
                {
                    _logger.LogWarning("Survey not found for ID: {SurveyId}", id);
                    return NotFound();
                }
                return View(survey);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Survey not found for deletion: {SurveyId}", id);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving survey for deletion: {SurveyId}", id);
                TempData["ErrorMessage"] = "Error retrieving survey. Please try again later.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Survey/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            _logger.LogInformation("DeleteConfirmed action invoked for survey ID: {SurveyId}", id);
            try
            {
                await _surveyService.DeleteSurveyAsync(id);
                TempData["SuccessMessage"] = "Survey deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Survey not found during deletion: {SurveyId}", id);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting survey: {SurveyId}", id);
                TempData["ErrorMessage"] = $"Error deleting survey: {ex.Message}";
                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        // POST: /Survey/SendEmails/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendEmails(Guid id)
        {
            _logger.LogInformation("SendEmails action invoked for survey ID: {SurveyId}", id);
            
            try
            {
                await _surveyService.SendSurveyEmailsAsync(id);
                _logger.LogInformation("Emails sent successfully for survey {SurveyId}", id);
                
                TempData["SuccessMessage"] = "Emails sent successfully!";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Survey not found when sending emails: {SurveyId}", id);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending emails for survey {SurveyId}: {ErrorMessage}", id, ex.Message);
                TempData["ErrorMessage"] = $"Error sending emails: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // POST: /Survey/SendOnTicketClosed
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendOnTicketClosed(string customerEmail, Guid? surveyId = null)
        {
            _logger.LogInformation("SendOnTicketClosed action invoked for email: {Email}, SurveyId: {SurveyId}", 
                customerEmail, surveyId);
            
            if (string.IsNullOrWhiteSpace(customerEmail) || !IsValidEmail(customerEmail))
            {
                _logger.LogWarning("Invalid email provided for ticket-closed event: {Email}", customerEmail);
                TempData["ErrorMessage"] = "Please provide a valid email address.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _logger.LogInformation("Sending survey on ticket closed to {Email}", customerEmail);
                bool success = await _surveyService.SendSurveyOnTicketClosedAsync(customerEmail, surveyId);
                
                if (success)
                {
                    _logger.LogInformation("Successfully sent survey email on ticket closed event to {Email}", customerEmail);
                    TempData["SuccessMessage"] = "Survey email sent on ticket closed event!";
                }
                else
                {
                    _logger.LogWarning("No eligible surveys found for ticket closed event to {Email}", customerEmail);
                    TempData["WarningMessage"] = "No eligible surveys found for ticket closed event.";
                }
                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending survey on ticket closed event to {Email}: {ErrorMessage}", 
                    customerEmail, ex.Message);
                TempData["ErrorMessage"] = $"Error sending survey email: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: /Survey/SendTestEmail
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendTestEmail(Guid id, string email)
        {
            _logger.LogInformation("SendTestEmail action invoked for SurveyId: {SurveyId}, Email: {Email}", id, email);
            
            if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
            {
                _logger.LogWarning("Invalid email provided for test email: {Email}", email);
                TempData["ErrorMessage"] = "Please provide a valid email address.";
                return RedirectToAction(nameof(Details), new { id });
            }

            try
            {
                _logger.LogInformation("Sending test survey email to {Email} for survey {SurveyId}", email, id);
                bool success = await _surveyService.SendTestSurveyEmailAsync(email, id);
                
                if (success)
                {
                    _logger.LogInformation("Test survey email sent successfully to {Email} for survey {SurveyId}", email, id);
                    TempData["SuccessMessage"] = $"Test survey email sent to {email} successfully!";
                }
                else
                {
                    _logger.LogWarning("Test email could not be sent to {Email} for survey {SurveyId}", email, id);
                    TempData["WarningMessage"] = "Test email could not be sent. Please check your email settings.";
                }
                
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Survey not found when sending test email: {SurveyId}", id);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending test email for survey {SurveyId} to {Email}: {ErrorMessage}", 
                    id, email, ex.Message);
                TempData["ErrorMessage"] = $"Error sending test email: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // Ajax action to add question form
        [HttpGet]
        public IActionResult AddQuestion(int index)
        {
            _logger.LogInformation("AddQuestion action invoked for index: {Index}", index);
            
            var question = new QuestionViewModel
            {
                Type = "SingleChoice",
                Required = true,
                Options = new System.Collections.Generic.List<string> { "Option 1", "Option 2", "Option 3" }
            };
            
            _logger.LogDebug("Returning question partial for index {Index}", index);
            return PartialView("_QuestionPartial", new Tuple<QuestionViewModel, int>(question, index));
        }
        
        // Helper method to validate email format
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                _logger.LogDebug("Email validation failed: Email is null or empty");
                return false;
            }
            
            try
            {
                var mailAddress = new MailAddress(email);
                var isValid = mailAddress.Address == email;
                _logger.LogDebug("Email validation for {Email}: {IsValid}", email, isValid);
                return isValid;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Email validation exception for {Email}: {ErrorMessage}", email, ex.Message);
                return false;
            }
        }
    }
}
