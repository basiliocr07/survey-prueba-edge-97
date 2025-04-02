using Microsoft.AspNetCore.Mvc;
using MediatR;
using SurveyApp.Application.Interfaces;
using SurveyApp.Application.Surveys.Commands.CreateSurvey;
using SurveyApp.Application.Surveys.Commands.UpdateSurvey;
using SurveyApp.Application.Surveys.Queries.GetAllSurveys;
using SurveyApp.Application.Surveys.Queries.GetSurveyById;
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
        private readonly IMediator _mediator;

        public SurveysController(ISurveyService surveyService, IMediator mediator)
        {
            _surveyService = surveyService;
            _mediator = mediator;
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
            
            var statistics = await _surveyService.GetSurveyStatisticsAsync(int.Parse(id));
            var statisticsViewModel = new SurveyStatisticsViewModel();
            
            if (statistics != null)
            {
                statisticsViewModel = new SurveyStatisticsViewModel
                {
                    TotalResponses = statistics.TotalResponses,
                    AverageCompletionTime = statistics.AverageCompletionTime,
                    CompletionRate = statistics.CompletionRate,
                    QuestionStats = statistics.QuestionStats?.Select(q => new QuestionStatisticViewModel
                    {
                        QuestionId = q.QuestionId,
                        QuestionTitle = q.QuestionTitle,
                        QuestionText = q.QuestionText,
                        Responses = q.Responses?.Select(r => new ResponseViewModel
                        {
                            Answer = r.Answer,
                            Count = r.Count,
                            Percentage = r.Percentage
                        }).ToList() ?? new List<ResponseViewModel>(),
                        ResponseDistribution = q.ResponseDistribution?.ToDictionary(
                            kvp => kvp.Key,
                            kvp => new ResponseDistributionViewModel
                            {
                                Count = kvp.Value.Count,
                                Percentage = kvp.Value.Percentage
                            }
                        ) ?? new Dictionary<string, ResponseDistributionViewModel>()
                    }).ToList() ?? new List<QuestionStatisticViewModel>()
                };
            }
            
            var model = new SurveyResultsViewModel
            {
                Survey = new SurveyViewModel
                {
                    Id = survey.Id.ToString(),
                    Title = survey.Title,
                    Description = survey.Description,
                    CreatedAt = survey.CreatedAt,
                    Status = survey.Status,
                    ResponseCount = statistics?.TotalResponses ?? 0,
                    CompletionRate = statistics?.CompletionRate ?? 0
                },
                Statistics = statisticsViewModel
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
                        Text = "",
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
                try
                {
                    // Ensure all questions have valid settings based on their type
                    foreach (var question in model.Questions)
                    {
                        // If options-based question type but no options provided, add default options
                        if (new[] { "multiple-choice", "single-choice", "dropdown", "ranking" }.Contains(question.Type) 
                            && (question.Options == null || question.Options.Count == 0))
                        {
                            question.Options = new List<string> { "Option 1", "Option 2" };
                        }

                        // If rating type but no settings, add default settings
                        if (question.Type == "rating" && question.Settings == null)
                        {
                            question.Settings = new QuestionSettingsViewModel { Min = 1, Max = 5 };
                        }
                        
                        // If NPS type but no settings, add default settings
                        if (question.Type == "nps" && question.Settings == null)
                        {
                            question.Settings = new QuestionSettingsViewModel { Min = 0, Max = 10 };
                        }
                    }

                    bool success;
                    
                    if (model.Id > 0)
                    {
                        // Update existing survey through CQRS command
                        var updateCommand = new UpdateSurveyCommand
                        {
                            Id = model.Id,
                            Title = model.Title,
                            Description = model.Description,
                            Status = model.Status,
                            Questions = model.Questions.Select(q => q.ToDomainModel()).ToList(),
                            DeliveryConfig = model.DeliveryConfig != null
                                ? new DeliveryConfiguration
                                {
                                    Type = model.DeliveryConfig.Type,
                                    EmailAddresses = model.DeliveryConfig.EmailAddresses ?? new List<string>(),
                                    Schedule = model.DeliveryConfig.Schedule != null
                                        ? new ScheduleSettings
                                        {
                                            Frequency = model.DeliveryConfig.Schedule.Frequency,
                                            DayOfMonth = model.DeliveryConfig.Schedule.DayOfMonth ?? 1,
                                            DayOfWeek = model.DeliveryConfig.Schedule.DayOfWeek,
                                            Time = model.DeliveryConfig.Schedule.Time ?? "09:00"
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
                                : null
                        };
                        
                        success = await _mediator.Send(updateCommand);
                        
                        if (success)
                            TempData["SuccessMessage"] = "Survey updated successfully.";
                        else
                            TempData["ErrorMessage"] = "Failed to update survey.";
                    }
                    else
                    {
                        // Create new survey through CQRS command
                        var createCommand = new CreateSurveyCommand(
                            model.Title,
                            model.Description,
                            model.Questions.Select(q => q.ToDomainModel()).ToList(),
                            model.Status,
                            model.DeliveryConfig != null
                                ? new DeliveryConfiguration
                                {
                                    Type = model.DeliveryConfig.Type,
                                    EmailAddresses = model.DeliveryConfig.EmailAddresses ?? new List<string>(),
                                    Schedule = model.DeliveryConfig.Schedule != null
                                        ? new ScheduleSettings
                                        {
                                            Frequency = model.DeliveryConfig.Schedule.Frequency,
                                            DayOfMonth = model.DeliveryConfig.Schedule.DayOfMonth ?? 1,
                                            DayOfWeek = model.DeliveryConfig.Schedule.DayOfWeek,
                                            Time = model.DeliveryConfig.Schedule.Time ?? "09:00"
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
                                : null
                        );
                        
                        var surveyId = await _mediator.Send(createCommand);
                        success = surveyId > 0;
                        
                        if (success)
                            TempData["SuccessMessage"] = "Survey created successfully.";
                        else
                            TempData["ErrorMessage"] = "Failed to create survey.";
                    }

                    if (success)
                        return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                    Console.WriteLine($"Error creating/updating survey: {ex}");
                }
            }

            return View(model);
        }

        // GET: Surveys/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var query = new GetSurveyByIdQuery(id);
            var survey = await _mediator.Send(query);
            
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
                    Text = q.Text,
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
                                DayOfWeek = survey.DeliveryConfig.Schedule.DayOfWeek,
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

            var model = new SurveyPreviewViewModel
            {
                Id = survey.Id,
                Title = survey.Title,
                Description = survey.Description,
                Questions = survey.Questions.Select(q => new QuestionViewModel
                {
                    Id = q.Id.ToString(),
                    Text = q.Text,
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

            string surveyUrl = Url.Action("Take", "SurveyResponses", new { id = survey.Id }, Request.Scheme);
            
            ViewBag.SurveyUrl = surveyUrl;
            ViewBag.SurveyTitle = survey.Title;

            return View();
        }

        // GET: Surveys/AddQuestion
        [HttpGet]
        public IActionResult AddQuestion(int index, string id)
        {
            var question = new QuestionViewModel
            {
                Id = id,
                Text = "New Question",
                Type = "single-choice",
                Required = true,
                Options = new List<string> { "Option 1", "Option 2", "Option 3" }
            };
            
            return PartialView("_QuestionBuilderPartial", Tuple.Create(question, index, index + 1));
        }

        // GET: Surveys/AddSampleQuestions
        [HttpGet]
        public IActionResult AddSampleQuestions(int startIndex)
        {
            var sampleQuestions = new List<QuestionViewModel>
            {
                new QuestionViewModel
                {
                    Id = $"new-{Guid.NewGuid()}",
                    Text = "How satisfied are you with our service?",
                    Type = "rating",
                    Required = true,
                    Settings = new QuestionSettingsViewModel { Min = 1, Max = 5 }
                },
                new QuestionViewModel
                {
                    Id = $"new-{Guid.NewGuid()}",
                    Text = "Would you recommend our product to others?",
                    Type = "single-choice",
                    Required = true,
                    Options = new List<string> { "Yes, definitely", "Maybe", "No" }
                },
                new QuestionViewModel
                {
                    Id = $"new-{Guid.NewGuid()}",
                    Text = "Please share any additional feedback",
                    Type = "text",
                    Required = false
                }
            };
            
            var result = new List<Tuple<QuestionViewModel, int, int>>();
            
            for (int i = 0; i < sampleQuestions.Count; i++)
            {
                result.Add(Tuple.Create(sampleQuestions[i], startIndex + i, startIndex + sampleQuestions.Count));
            }
            
            return PartialView("_MultipleSampleQuestionsPartial", result);
        }

        // Helper method to get real surveys from database
        private async Task<List<SurveyViewModel>> GetSurveys()
        {
            try
            {
                var query = new GetAllSurveysQuery();
                var surveys = await _mediator.Send(query);
                
                return surveys.Select(s => new SurveyViewModel
                {
                    Id = s.Id.ToString(),
                    Title = s.Title,
                    Description = s.Description,
                    CreatedAt = s.CreatedAt,
                    ResponseCount = s.ResponseCount,
                    CompletionRate = s.CompletionRate,
                    Status = s.Status
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting surveys: {ex.Message}");
                return GetSampleSurveys();
            }
        }

        // Helper method to get sample surveys while we're developing
        private List<SurveyViewModel> GetSampleSurveys()
        {
            return new List<SurveyViewModel>
            {
                new SurveyViewModel
                {
                    Id = "1",
                    Title = "Customer Satisfaction Survey",
                    Description = "Gather feedback about our customer service quality",
                    CreatedAt = DateTime.Parse("2023-10-15T12:00:00Z"),
                    ResponseCount = 42,
                    CompletionRate = 78,
                    Status = "active"
                },
                new SurveyViewModel
                {
                    Id = "2",
                    Title = "Product Feedback Survey",
                    Description = "Help us improve our product offerings",
                    CreatedAt = DateTime.Parse("2023-09-22T15:30:00Z"),
                    ResponseCount = 103,
                    CompletionRate = 89,
                    Status = "active"
                },
                new SurveyViewModel
                {
                    Id = "3",
                    Title = "Website Usability Survey",
                    Description = "Evaluate the user experience of our new website",
                    CreatedAt = DateTime.Parse("2023-11-05T09:15:00Z"),
                    ResponseCount = 28,
                    CompletionRate = 65,
                    Status = "draft"
                },
                new SurveyViewModel
                {
                    Id = "4",
                    Title = "Employee Satisfaction Survey",
                    Description = "Annual survey for employee feedback",
                    CreatedAt = DateTime.Parse("2023-08-10T14:20:00Z"),
                    ResponseCount = 56,
                    CompletionRate = 92,
                    Status = "archived"
                }
            };
        }
    }
}
