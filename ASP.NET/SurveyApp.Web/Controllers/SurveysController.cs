using Microsoft.AspNetCore.Mvc;
using SurveyApp.Application.Interfaces;
using SurveyApp.Domain.Models;
using SurveyApp.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyApp.Web.Controllers
{
    public class SurveysController : Controller
    {
        private readonly ISurveyService _surveyService;

        public SurveysController(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        // GET: Surveys
        public async Task<IActionResult> Index()
        {
            var surveys = await GetSurveys();
            ViewBag.FilterActive = "all"; // Default filter
            
            return View(surveys);
        }

        // GET: Surveys/Filter
        [HttpGet]
        public async Task<ActionResult> Filter(string filter)
        {
            var allSurveys = await GetSurveys();
            var filteredSurveys = filter switch
            {
                "active" => allSurveys.Where(s => s.Status == "active").ToList(),
                "draft" => allSurveys.Where(s => s.Status == "draft").ToList(),
                "archived" => allSurveys.Where(s => s.Status == "archived").ToList(),
                _ => allSurveys
            };

            ViewBag.FilterActive = filter ?? "all";
            return View("Index", filteredSurveys);
        }

        // GET: Surveys/Results/{id}
        public async Task<IActionResult> Results(string id)
        {
            var survey = await _surveyService.GetSurveyByIdAsync(int.Parse(id));
            if (survey == null)
            {
                return NotFound();
            }
            
            // Get survey statistics
            var statistics = await _surveyService.GetSurveyStatisticsAsync(int.Parse(id));
            
            var model = new SurveyResultsViewModel
            {
                Survey = new SurveyViewModel
                {
                    Id = survey.Id.ToString(),
                    Title = survey.Title,
                    Description = survey.Description,
                    CreatedAt = survey.CreatedAt,
                    Status = survey.Status,
                    Responses = statistics?.TotalResponses ?? 0,
                    CompletionRate = statistics?.CompletionRate ?? 0
                },
                Statistics = statistics
            };
            
            return View(model);
        }

        // GET: Surveys/Create
        public IActionResult Create()
        {
            return View(new CreateSurveyViewModel
            {
                Id = 0, // New survey
                Questions = new List<QuestionViewModel>
                {
                    new QuestionViewModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        Type = "single-choice",
                        Title = "",
                        Required = true,
                        Options = new List<string> { "Option 1", "Option 2", "Option 3" }
                    }
                },
                DeliveryConfig = new DeliveryConfigViewModel
                {
                    Type = "manual",
                    EmailAddresses = new List<string>(),
                    Schedule = new ScheduleSettingsViewModel(),
                    Trigger = new TriggerSettingsViewModel()
                }
            });
        }

        // POST: Surveys/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSurveyViewModel model)
        {
            if (ModelState.IsValid)
            {
                var survey = new Survey
                {
                    Title = model.Title,
                    Description = model.Description,
                    CreatedAt = DateTime.Now,
                    Status = model.Status,
                    Questions = model.Questions.Select(q => new Question
                    {
                        Text = q.Title,
                        Type = q.Type,
                        Required = q.Required,
                        Description = q.Description,
                        Options = q.Options,
                        Settings = q.Settings != null 
                            ? new QuestionSettings 
                            { 
                                Min = q.Settings.Min, 
                                Max = q.Settings.Max 
                            } 
                            : null
                    }).ToList(),
                    DeliveryConfig = new DeliveryConfiguration
                    {
                        Type = model.DeliveryConfig.Type,
                        EmailAddresses = model.DeliveryConfig.EmailAddresses,
                        Schedule = model.DeliveryConfig.Schedule != null 
                            ? new ScheduleSettings
                            {
                                Frequency = model.DeliveryConfig.Schedule.Frequency,
                                DayOfMonth = model.DeliveryConfig.Schedule.DayOfMonth,
                                Time = model.DeliveryConfig.Schedule.Time
                            }
                            : null,
                        Trigger = model.DeliveryConfig.Trigger != null
                            ? new TriggerSettings
                            {
                                Type = model.DeliveryConfig.Trigger.Type,
                                DelayHours = model.DeliveryConfig.Trigger.DelayHours,
                                SendAutomatically = model.DeliveryConfig.Trigger.SendAutomatically
                            }
                            : null
                    }
                };

                bool success;
                if (model.Id > 0)
                {
                    survey.Id = model.Id;
                    success = await _surveyService.UpdateSurveyAsync(survey);
                    if (success)
                        TempData["SuccessMessage"] = "Survey updated successfully.";
                    else
                        TempData["ErrorMessage"] = "Failed to update survey.";
                }
                else
                {
                    success = await _surveyService.CreateSurveyAsync(survey);
                    if (success)
                        TempData["SuccessMessage"] = "Survey created successfully.";
                    else
                        TempData["ErrorMessage"] = "Failed to create survey.";
                }

                if (success)
                    return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Surveys/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var survey = await _surveyService.GetSurveyByIdAsync(id);
            if (survey == null)
            {
                return NotFound();
            }

            var model = new CreateSurveyViewModel
            {
                Id = survey.Id,
                Title = survey.Title,
                Description = survey.Description,
                Status = survey.Status,
                Questions = survey.Questions.Select(q => new QuestionViewModel
                {
                    Id = q.Id.ToString(),
                    Title = q.Text,
                    Type = q.Type,
                    Required = q.Required,
                    Description = q.Description,
                    Options = q.Options,
                    Settings = q.Settings != null 
                        ? new QuestionSettingsViewModel
                        {
                            Min = q.Settings.Min,
                            Max = q.Settings.Max
                        }
                        : null
                }).ToList(),
                DeliveryConfig = survey.DeliveryConfig != null
                    ? new DeliveryConfigViewModel
                    {
                        Type = survey.DeliveryConfig.Type,
                        EmailAddresses = survey.DeliveryConfig.EmailAddresses,
                        Schedule = survey.DeliveryConfig.Schedule != null
                            ? new ScheduleSettingsViewModel
                            {
                                Frequency = survey.DeliveryConfig.Schedule.Frequency,
                                DayOfMonth = survey.DeliveryConfig.Schedule.DayOfMonth,
                                Time = survey.DeliveryConfig.Schedule.Time
                            }
                            : new ScheduleSettingsViewModel(),
                        Trigger = survey.DeliveryConfig.Trigger != null
                            ? new TriggerSettingsViewModel
                            {
                                Type = survey.DeliveryConfig.Trigger.Type,
                                DelayHours = survey.DeliveryConfig.Trigger.DelayHours,
                                SendAutomatically = survey.DeliveryConfig.Trigger.SendAutomatically
                            }
                            : new TriggerSettingsViewModel()
                    }
                    : new DeliveryConfigViewModel()
            };

            return View("Create", model);
        }

        // POST: Surveys/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _surveyService.DeleteSurveyAsync(id);
            if (result)
                TempData["SuccessMessage"] = "Survey deleted successfully.";
            else
                TempData["ErrorMessage"] = "Failed to delete survey.";
                
            return RedirectToAction(nameof(Index));
        }

        // POST: Surveys/SendEmails
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendEmails(int surveyId, List<string> emailAddresses)
        {
            var success = await _surveyService.SendSurveyEmailsAsync(surveyId, emailAddresses);
            if (success)
            {
                TempData["SuccessMessage"] = "Survey emails have been queued for delivery.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to send survey emails. Please try again.";
            }
            
            return RedirectToAction(nameof(Edit), new { id = surveyId });
        }

        // NEW: API endpoint to check if a survey exists
        [HttpGet]
        [Route("api/surveys/{id}/exists")]
        public async Task<IActionResult> SurveyExists(int id)
        {
            var survey = await _surveyService.GetSurveyByIdAsync(id);
            return Json(new { exists = survey != null });
        }

        // NEW: Preview Survey
        [HttpGet]
        public async Task<IActionResult> Preview(int id)
        {
            var survey = await _surveyService.GetSurveyByIdAsync(id);
            if (survey == null)
            {
                return NotFound();
            }

            // Create a view model for the preview
            var model = new SurveyPreviewViewModel
            {
                Id = survey.Id,
                Title = survey.Title,
                Description = survey.Description,
                Questions = survey.Questions.Select(q => new QuestionViewModel
                {
                    Id = q.Id.ToString(),
                    Title = q.Text,
                    Type = q.Type,
                    Required = q.Required,
                    Description = q.Description,
                    Options = q.Options,
                    Settings = q.Settings != null 
                        ? new QuestionSettingsViewModel
                        {
                            Min = q.Settings.Min,
                            Max = q.Settings.Max
                        }
                        : null
                }).ToList()
            };

            return View(model);
        }

        // NEW: Share Survey
        [HttpGet]
        public async Task<IActionResult> Share(int id)
        {
            var survey = await _surveyService.GetSurveyByIdAsync(id);
            if (survey == null)
            {
                return NotFound();
            }

            // Create sharing URL
            string surveyUrl = Url.Action("Take", "SurveyResponses", new { id = survey.Id }, Request.Scheme);
            
            ViewBag.SurveyUrl = surveyUrl;
            ViewBag.SurveyTitle = survey.Title;

            return View();
        }

        // Helper method to get real surveys from database
        private async Task<List<SurveyViewModel>> GetSurveys()
        {
            try
            {
                var surveys = await _surveyService.GetAllSurveysAsync();
                return surveys.Select(s => new SurveyViewModel
                {
                    Id = s.Id.ToString(),
                    Title = s.Title,
                    Description = s.Description,
                    CreatedAt = s.CreatedAt,
                    Responses = s.ResponseCount, 
                    CompletionRate = s.CompletionRate,
                    Status = s.Status
                }).ToList();
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Error getting surveys: {ex.Message}");
                
                // If there's an error, return sample data temporarily
                return GetSampleSurveys();
            }
        }

        // Helper method to get sample surveys while we're developing
        private List<SurveyViewModel> GetSampleSurveys()
        {
            // This would normally come from the database via the service
            // For now, we're creating sample data that matches the React version
            return new List<SurveyViewModel>
            {
                new SurveyViewModel
                {
                    Id = "1",
                    Title = "Customer Satisfaction Survey",
                    Description = "Gather feedback about our customer service quality",
                    CreatedAt = DateTime.Parse("2023-10-15T12:00:00Z"),
                    Responses = 42,
                    CompletionRate = 78,
                    Status = "active"
                },
                new SurveyViewModel
                {
                    Id = "2",
                    Title = "Product Feedback Survey",
                    Description = "Help us improve our product offerings",
                    CreatedAt = DateTime.Parse("2023-09-22T15:30:00Z"),
                    Responses = 103,
                    CompletionRate = 89,
                    Status = "active"
                },
                new SurveyViewModel
                {
                    Id = "3",
                    Title = "Website Usability Survey",
                    Description = "Evaluate the user experience of our new website",
                    CreatedAt = DateTime.Parse("2023-11-05T09:15:00Z"),
                    Responses = 28,
                    CompletionRate = 65,
                    Status = "draft"
                },
                new SurveyViewModel
                {
                    Id = "4",
                    Title = "Employee Satisfaction Survey",
                    Description = "Annual survey for employee feedback",
                    CreatedAt = DateTime.Parse("2023-08-10T14:20:00Z"),
                    Responses = 56,
                    CompletionRate = 92,
                    Status = "archived"
                }
            };
        }
    }
}
