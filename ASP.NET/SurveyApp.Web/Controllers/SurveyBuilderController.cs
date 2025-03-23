
using Microsoft.AspNetCore.Mvc;
using SurveyApp.Web.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SurveyApp.Application.Interfaces;

namespace SurveyApp.Web.Controllers
{
    public class SurveyBuilderController : Controller
    {
        private readonly ISurveyService _surveyService;

        public SurveyBuilderController(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new CreateSurveyViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSurveyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Map view model to domain model
                var surveyId = await _surveyService.CreateSurveyAsync(model);
                
                return RedirectToAction("Index", "Surveys");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating survey: {ex.Message}");
                return View(model);
            }
        }

        [HttpGet("SurveyBuilder/Edit/{id}")]
        public async Task<IActionResult> Edit(string id)
        {
            var survey = await _surveyService.GetSurveyByIdAsync(id);
            if (survey == null)
            {
                return NotFound();
            }

            // Map domain model to view model
            var viewModel = new CreateSurveyViewModel
            {
                Id = int.TryParse(id, out var idValue) ? idValue : 0,
                Title = survey.Title,
                Description = survey.Description ?? string.Empty,
                Status = survey.Status ?? "draft"
                // Map other properties
            };

            return View("Create", viewModel);
        }
    }
}
