
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

        // Helper method to get surveys
        private async Task<List<SurveyViewModel>> GetSurveys()
        {
            var surveys = await _mediator.Send(new GetAllSurveysQuery());
            return surveys.Select(s => new SurveyViewModel
            {
                Id = s.Id.ToString(),
                Title = s.Title,
                Description = s.Description,
                CreatedAt = s.CreatedAt,
                Status = s.Status,
                ResponseCount = s.ResponseCount,
                CompletionRate = s.CompletionRate
            }).ToList();
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
                    QuestionStats = statistics.QuestionStats?.Select(q => new QuestionStatViewModel
                    {
                        QuestionId = q.QuestionId,
                        QuestionTitle = q.QuestionTitle,
                        QuestionText = q.QuestionText,
                        Responses = q.Responses?.Select(r => new StatResponseViewModel
                        {
                            Answer = r.Answer,
                            Count = r.Count,
                            Percentage = r.Percentage
                        }).ToList() ?? new List<StatResponseViewModel>(),
                        ResponseDistribution = q.ResponseDistribution?.ToDictionary(
                            kvp => kvp.Key,
                            kvp => new ResponseDistribution
                            {
                                Count = kvp.Value.Count,
                                Percentage = kvp.Value.Percentage
                            }
                        ) ?? new Dictionary<string, ResponseDistribution>()
                    }).ToList() ?? new List<QuestionStatViewModel>()
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
                        question.EnsureConsistency();
                    }
                    
                    // Map view model to domain model
                    var survey = new Survey
                    {
                        Id = model.Id,
                        Title = model.Title,
                        Description = model.Description,
                        Status = model.Status,
                        CreatedAt = DateTime.Now,
                        Questions = model.Questions.Select(q => q.ToDomainModel()).ToList(),
                        DeliveryConfig = model.DeliveryConfig != null ? new DeliveryConfiguration
                        {
                            Type = model.DeliveryConfig.Type,
                            EmailAddresses = model.DeliveryConfig.EmailAddresses,
                            Schedule = model.DeliveryConfig.Schedule != null ? new ScheduleSettings
                            {
                                Frequency = model.DeliveryConfig.Schedule.Frequency,
                                DayOfMonth = model.DeliveryConfig.Schedule.DayOfMonth ?? 1,
                                DayOfWeek = model.DeliveryConfig.Schedule.DayOfWeek,
                                Time = model.DeliveryConfig.Schedule.Time,
                                StartDate = model.DeliveryConfig.Schedule.StartDate
                            } : null,
                            Trigger = model.DeliveryConfig.Trigger != null ? new TriggerSettings
                            {
                                Type = model.DeliveryConfig.Trigger.Type,
                                DelayHours = model.DeliveryConfig.Trigger.DelayHours,
                                SendAutomatically = model.DeliveryConfig.Trigger.SendAutomatically
                            } : null,
                            IncludeAllCustomers = model.DeliveryConfig.IncludeAllCustomers,
                            CustomerTypeFilter = model.DeliveryConfig.CustomerTypeFilter
                        } : null
                    };
                    
                    bool success;
                    
                    if (model.Id > 0)
                    {
                        // Update existing survey
                        success = await _surveyService.UpdateSurveyAsync(survey);
                    }
                    else
                    {
                        // Create new survey
                        success = await _surveyService.CreateSurveyAsync(survey);
                    }
                    
                    if (success)
                    {
                        // If delivery config is set to send emails immediately, do it
                        if (model.DeliveryConfig != null && 
                            model.DeliveryConfig.Type == "manual" && 
                            model.DeliveryConfig.EmailAddresses != null && 
                            model.DeliveryConfig.EmailAddresses.Any())
                        {
                            await _surveyService.SendSurveyEmailsAsync(survey.Id, model.DeliveryConfig.EmailAddresses);
                        }
                        
                        TempData["SuccessMessage"] = model.Id > 0 ? "Survey updated successfully!" : "Survey created successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        ModelState.AddModelError("", "Failed to save the survey. Please try again.");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                }
            }
            
            // If we got to here, something went wrong, redisplay form
            return View(model);
        }

        // GET: Surveys/Edit/{id}
        public async Task<IActionResult> Edit(int id)
        {
            var survey = await _surveyService.GetSurveyByIdAsync(id);
            if (survey == null)
            {
                return NotFound();
            }
            
            var viewModel = new CreateSurveyViewModel
            {
                Id = survey.Id,
                Title = survey.Title,
                Description = survey.Description,
                Status = survey.Status,
                Questions = survey.Questions.Select(q => QuestionViewModel.FromDomainModel(q)).ToList(),
                DeliveryConfig = survey.DeliveryConfig != null ? new DeliveryConfigViewModel
                {
                    Type = survey.DeliveryConfig.Type,
                    EmailAddresses = survey.DeliveryConfig.EmailAddresses,
                    Schedule = survey.DeliveryConfig.Schedule != null ? new ScheduleSettingsViewModel
                    {
                        Frequency = survey.DeliveryConfig.Schedule.Frequency,
                        DayOfMonth = survey.DeliveryConfig.Schedule.DayOfMonth,
                        DayOfWeek = survey.DeliveryConfig.Schedule.DayOfWeek,
                        Time = survey.DeliveryConfig.Schedule.Time,
                        StartDate = survey.DeliveryConfig.Schedule.StartDate
                    } : new ScheduleSettingsViewModel(),
                    Trigger = survey.DeliveryConfig.Trigger != null ? new TriggerSettingsViewModel
                    {
                        Type = survey.DeliveryConfig.Trigger.Type,
                        DelayHours = survey.DeliveryConfig.Trigger.DelayHours,
                        SendAutomatically = survey.DeliveryConfig.Trigger.SendAutomatically
                    } : new TriggerSettingsViewModel(),
                    IncludeAllCustomers = survey.DeliveryConfig.IncludeAllCustomers,
                    CustomerTypeFilter = survey.DeliveryConfig.CustomerTypeFilter
                } : new DeliveryConfigViewModel()
            };
            
            return View("Create", viewModel);
        }

        // GET: Surveys/Delete/{id}
        public async Task<IActionResult> Delete(int id)
        {
            var survey = await _surveyService.GetSurveyByIdAsync(id);
            if (survey == null)
            {
                return NotFound();
            }
            
            return View(new SurveyViewModel
            {
                Id = survey.Id.ToString(),
                Title = survey.Title,
                Description = survey.Description,
                CreatedAt = survey.CreatedAt,
                Status = survey.Status,
                ResponseCount = survey.ResponseCount
            });
        }

        // POST: Surveys/Delete/{id}
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _surveyService.DeleteSurveyAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Survey deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            
            TempData["ErrorMessage"] = "Failed to delete survey. Please try again.";
            return RedirectToAction(nameof(Delete), new { id });
        }
    }
}
