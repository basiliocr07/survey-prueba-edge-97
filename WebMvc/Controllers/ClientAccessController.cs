
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SurveyApp.Application.Services;
using SurveyApp.WebMvc.Models;
using System.Linq;

namespace SurveyApp.WebMvc.Controllers
{
    public class ClientAccessController : Controller
    {
        private readonly ISurveyService _surveyService;

        public ClientAccessController(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        // GET: ClientAccess/Survey/{id}
        [HttpGet("survey/{id}")]
        public async Task<IActionResult> Survey(Guid id)
        {
            try
            {
                var survey = await _surveyService.GetSurveyForClientAsync(id);
                if (survey == null)
                {
                    return NotFound();
                }

                // Map to view model
                var viewModel = new SurveyViewModel
                {
                    Id = survey.Id,
                    Title = survey.Title,
                    Description = survey.Description,
                    Questions = survey.Questions.Select(q => new QuestionViewModel
                    {
                        Id = q.Id,
                        Title = q.Title,
                        Description = q.Description,
                        Type = q.Type,
                        Required = q.Required,
                        Options = q.Options
                    }).ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                // Log the error
                return RedirectToAction("Error", "Home", new { message = ex.Message });
            }
        }

        // GET: ClientAccess/ThankYou
        [HttpGet("thank-you")]
        public IActionResult ThankYou()
        {
            // Using nullish coalescing operator
            ViewBag.SurveyTitle = TempData["SurveyTitle"] ?? "Encuesta";
            ViewBag.ThankYouMessage = TempData["ThankYouMessage"] ?? "Gracias por completar nuestra encuesta.";
            
            return View();
        }
    }
}
