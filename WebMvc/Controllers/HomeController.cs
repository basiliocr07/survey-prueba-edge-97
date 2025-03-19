
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SurveyApp.Application.Services;
using SurveyApp.WebMvc.Models;

namespace SurveyApp.WebMvc.Controllers
{
    // Home es accesible para todos (anónimos, clientes y administradores)
    public class HomeController : Controller
    {
        private readonly ISurveyService _surveyService;
        private readonly ISuggestionService _suggestionService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            ISurveyService surveyService,
            ISuggestionService suggestionService,
            ILogger<HomeController> logger)
        {
            _surveyService = surveyService;
            _suggestionService = suggestionService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                // Crear el modelo de vista con las características destacadas
                var viewModel = new HomeViewModel
                {
                    Features = new List<FeatureViewModel>
                    {
                        new FeatureViewModel
                        {
                            Title = "Intuitive Survey Builder",
                            Description = "Create beautiful surveys with our drag-and-drop interface. No coding required.",
                            Icon = "fa-file-alt"
                        },
                        new FeatureViewModel
                        {
                            Title = "Powerful Analytics",
                            Description = "Get real-time insights with customizable reports and advanced visualizations.",
                            Icon = "fa-chart-bar"
                        },
                        new FeatureViewModel
                        {
                            Title = "Multiple Question Types",
                            Description = "Choose from a variety of question types to gather the precise data you need.",
                            Icon = "fa-check-square"
                        },
                        new FeatureViewModel
                        {
                            Title = "Rating Scales",
                            Description = "Measure sentiment and satisfaction with customizable rating scales.",
                            Icon = "fa-star"
                        },
                        new FeatureViewModel
                        {
                            Title = "Ranking Questions",
                            Description = "Allow respondents to rank items in order of preference or importance.",
                            Icon = "fa-arrows-alt-v"
                        },
                        new FeatureViewModel
                        {
                            Title = "Logic Branching",
                            Description = "Create dynamic surveys that adapt based on previous answers.",
                            Icon = "fa-plus-circle"
                        }
                    }
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar la página de inicio");
                return View("Error");
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
