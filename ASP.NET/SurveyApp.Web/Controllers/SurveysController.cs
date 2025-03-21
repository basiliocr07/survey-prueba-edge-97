
using Microsoft.AspNetCore.Mvc;
using SurveyApp.Application.Interfaces;
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
            // This would normally come from the database via the service
            // For now, we're creating sample data that matches the React version
            var surveys = new List<SurveyViewModel>
            {
                new SurveyViewModel
                {
                    Id = "1",
                    Title = "Customer Satisfaction Survey",
                    Description = "Gather feedback about our customer service quality",
                    CreatedAt = DateTime.Parse("2023-10-15T12:00:00Z"),
                    Responses = 42,
                    CompletionRate = 78
                },
                new SurveyViewModel
                {
                    Id = "2",
                    Title = "Product Feedback Survey",
                    Description = "Help us improve our product offerings",
                    CreatedAt = DateTime.Parse("2023-09-22T15:30:00Z"),
                    Responses = 103,
                    CompletionRate = 89
                },
                new SurveyViewModel
                {
                    Id = "3",
                    Title = "Website Usability Survey",
                    Description = "Evaluate the user experience of our new website",
                    CreatedAt = DateTime.Parse("2023-11-05T09:15:00Z"),
                    Responses = 28,
                    CompletionRate = 65
                }
            };

            ViewBag.FilterActive = "all"; // Default filter
            
            return View(surveys);
        }

        // GET: Surveys/Filter
        [HttpGet]
        public ActionResult Filter(string filter)
        {
            // This would filter based on the active filter in a real application
            // For now, we're just returning the view with the filter value
            ViewBag.FilterActive = filter ?? "all";

            // Same sample data as in Index
            var surveys = new List<SurveyViewModel>
            {
                new SurveyViewModel
                {
                    Id = "1",
                    Title = "Customer Satisfaction Survey",
                    Description = "Gather feedback about our customer service quality",
                    CreatedAt = DateTime.Parse("2023-10-15T12:00:00Z"),
                    Responses = 42,
                    CompletionRate = 78
                },
                new SurveyViewModel
                {
                    Id = "2",
                    Title = "Product Feedback Survey",
                    Description = "Help us improve our product offerings",
                    CreatedAt = DateTime.Parse("2023-09-22T15:30:00Z"),
                    Responses = 103,
                    CompletionRate = 89
                },
                new SurveyViewModel
                {
                    Id = "3",
                    Title = "Website Usability Survey",
                    Description = "Evaluate the user experience of our new website",
                    CreatedAt = DateTime.Parse("2023-11-05T09:15:00Z"),
                    Responses = 28,
                    CompletionRate = 65
                }
            };

            return View("Index", surveys);
        }

        // GET: Surveys/Create
        public IActionResult Create()
        {
            return RedirectToAction("Create", "Survey");
        }

        // GET: Surveys/Edit/5
        public IActionResult Edit(string id)
        {
            // In a real application, this would fetch the survey by ID
            return View();
        }

        // GET: Surveys/Delete/5
        [HttpPost]
        public IActionResult Delete(string id)
        {
            // In a real application, this would delete the survey by ID
            return RedirectToAction(nameof(Index));
        }
    }
}
