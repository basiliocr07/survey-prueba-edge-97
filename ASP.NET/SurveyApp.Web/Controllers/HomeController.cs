
using Microsoft.AspNetCore.Mvc;
using SurveyApp.Application.Interfaces;
using SurveyApp.Web.Models;
using System.Collections.Generic;
using System.Diagnostics;

namespace SurveyApp.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISurveyService _surveyService;

        public HomeController(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        public IActionResult Index()
        {
            // Define los mismos features que tenemos en la versión React
            var features = new List<FeatureViewModel>
            {
                new FeatureViewModel
                {
                    Title = "Intuitive Survey Builder",
                    Description = "Create beautiful surveys with our drag-and-drop interface. No coding required.",
                    IconName = "FileText"
                },
                new FeatureViewModel
                {
                    Title = "Powerful Analytics",
                    Description = "Get real-time insights with customizable reports and advanced visualizations.",
                    IconName = "BarChart3"
                },
                new FeatureViewModel
                {
                    Title = "Multiple Question Types",
                    Description = "Choose from a variety of question types to gather the precise data you need.",
                    IconName = "CheckSquare"
                },
                new FeatureViewModel
                {
                    Title = "Rating Scales",
                    Description = "Measure sentiment and satisfaction with customizable rating scales.",
                    IconName = "Star"
                },
                new FeatureViewModel
                {
                    Title = "Ranking Questions",
                    Description = "Allow respondents to rank items in order of preference or importance.",
                    IconName = "MoveVertical"
                },
                new FeatureViewModel
                {
                    Title = "Logic Branching",
                    Description = "Create dynamic surveys that adapt based on previous answers.",
                    IconName = "PlusCircle"
                }
            };

            // Pasar los datos a la vista
            return View(features);
        }

        public IActionResult Create()
        {
            return RedirectToAction("Create", "Survey");
        }

        public IActionResult Results()
        {
            return RedirectToAction("Index", "Results");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
