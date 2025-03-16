
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SurveyApp.Application.DTOs;
using SurveyApp.Application.Services;
using SurveyApp.WebMvc.Models;

namespace SurveyApp.WebMvc.Controllers
{
    public class SurveyController : Controller
    {
        private readonly ISurveyService _surveyService;

        public SurveyController(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        // GET: /Survey
        public async Task<IActionResult> Index()
        {
            var surveys = await _surveyService.GetAllSurveysAsync();
            return View(surveys);
        }

        // GET: /Survey/Create
        public IActionResult Create()
        {
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
            return View(viewModel);
        }

        // POST: /Survey/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSurveyViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var createSurveyDto = new CreateSurveyDto
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

                    var survey = await _surveyService.CreateSurveyAsync(createSurveyDto);
                    TempData["SuccessMessage"] = "Survey created successfully!";
                    return RedirectToAction(nameof(Details), new { id = survey.Id });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error creating survey: {ex.Message}");
                }
            }

            return View(model);
        }

        // GET: /Survey/Edit/{id}
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var survey = await _surveyService.GetSurveyByIdAsync(id);
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
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // POST: /Survey/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, CreateSurveyViewModel model)
        {
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
                catch (KeyNotFoundException)
                {
                    return NotFound();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error updating survey: {ex.Message}");
                }
            }

            return View(model);
        }

        // GET: /Survey/Details/{id}
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var survey = await _surveyService.GetSurveyByIdAsync(id);
                return View(survey);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // GET: /Survey/Delete/{id}
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var survey = await _surveyService.GetSurveyByIdAsync(id);
                return View(survey);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // POST: /Survey/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                await _surveyService.DeleteSurveyAsync(id);
                TempData["SuccessMessage"] = "Survey deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting survey: {ex.Message}";
                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        // POST: /Survey/SendEmails/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendEmails(Guid id)
        {
            try
            {
                await _surveyService.SendSurveyEmailsAsync(id);
                TempData["SuccessMessage"] = "Emails sent successfully!";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error sending emails: {ex.Message}";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // POST: /Survey/SendOnTicketClosed
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendOnTicketClosed(string customerEmail, Guid? surveyId = null)
        {
            try
            {
                await _surveyService.SendSurveyOnTicketClosedAsync(customerEmail, surveyId);
                TempData["SuccessMessage"] = "Survey email sent on ticket closed event!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error sending survey email: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // Ajax action to add question form
        [HttpGet]
        public IActionResult AddQuestion(int index)
        {
            var question = new QuestionViewModel
            {
                Type = "SingleChoice",
                Required = true,
                Options = new System.Collections.Generic.List<string> { "Option 1", "Option 2", "Option 3" }
            };
            
            return PartialView("_QuestionPartial", new Tuple<QuestionViewModel, int>(question, index));
        }
    }
}
