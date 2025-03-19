using System;
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
                var surveys = await _surveyService.GetAllSurveysAsync();
                return View(surveys);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las encuestas");
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
