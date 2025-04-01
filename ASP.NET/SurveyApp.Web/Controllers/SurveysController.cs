
using Microsoft.AspNetCore.Mvc;
using SurveyApp.Application.Interfaces;
using SurveyApp.Domain.Models;
using SurveyApp.Web.Models;
using System;
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

        public async Task<IActionResult> Index(string filter = "all")
        {
            try
            {
                var surveys = filter == "all" 
                    ? await _surveyService.GetAllSurveysAsync() 
                    : await _surveyService.GetSurveysByStatusAsync(filter);

                var viewModel = new SurveysViewModel
                {
                    Surveys = surveys.ToList(),
                    FilterActive = filter
                };

                // Cargar estadísticas para cada encuesta
                foreach (var survey in viewModel.Surveys)
                {
                    var stats = await _surveyService.GetSurveyStatisticsAsync(survey.Id);
                    survey.ResponseCount = stats.TotalResponses;
                    survey.CompletionRate = stats.CompletionRate;
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al cargar las encuestas: {ex.Message}";
                return View(new SurveysViewModel());
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var survey = await _surveyService.GetSurveyByIdAsync(id);
            if (survey == null)
            {
                TempData["ErrorMessage"] = "No se encontró la encuesta solicitada.";
                return RedirectToAction("Index");
            }

            var viewModel = new SurveyDetailViewModel
            {
                Id = survey.Id,
                Title = survey.Title,
                Description = survey.Description,
                Questions = survey.Questions.Select(SurveyQuestionViewModel.FromQuestionViewModel).ToList()
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult Preview(int id)
        {
            return RedirectToAction("Details", "SurveyPreview", new { id });
        }

        [HttpGet]
        public IActionResult Results(int id)
        {
            return RedirectToAction("Details", "SurveyResults", new { id });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _surveyService.DeleteSurveyAsync(id);
                if (result)
                {
                    TempData["SuccessMessage"] = "Encuesta eliminada exitosamente.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al eliminar la encuesta.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al eliminar la encuesta: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
    }
}
