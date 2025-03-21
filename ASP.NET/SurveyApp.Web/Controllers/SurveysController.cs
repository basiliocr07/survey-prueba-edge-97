
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
            var surveys = await GetSampleSurveys();
            ViewBag.FilterActive = "all"; // Default filter
            
            return View(surveys);
        }

        // GET: Surveys/Filter
        [HttpGet]
        public async Task<ActionResult> Filter(string filter)
        {
            var allSurveys = await GetSampleSurveys();
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

        // GET: Surveys/Create
        public IActionResult Create()
        {
            return View(new CreateSurveyViewModel
            {
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
                    }).ToList()
                };

                await _surveyService.CreateSurveyAsync(survey);
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
                }).ToList()
            };

            return View("Create", model);
        }

        // POST: Surveys/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _surveyService.DeleteSurveyAsync(id);
            return RedirectToAction(nameof(Index));
        }

        // Helper method to get sample surveys while we're developing
        private async Task<List<SurveyViewModel>> GetSampleSurveys()
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
