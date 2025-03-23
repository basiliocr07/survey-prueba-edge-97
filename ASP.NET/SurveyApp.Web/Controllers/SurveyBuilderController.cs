
using Microsoft.AspNetCore.Mvc;
using SurveyApp.Web.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SurveyApp.Application.Interfaces;
using SurveyApp.Domain.Models;
using System.Linq;

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
                var survey = MapViewModelToDomain(model);
                
                var success = await _surveyService.CreateSurveyAsync(survey);
                if (success)
                {
                    TempData["SuccessMessage"] = "Survey created successfully!";
                    return RedirectToAction("Index", "Surveys");
                }
                else
                {
                    ModelState.AddModelError("", "Failed to create survey.");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating survey: {ex.Message}");
                return View(model);
            }
        }

        [HttpGet("SurveyBuilder/Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            var survey = await _surveyService.GetSurveyByIdAsync(id);
            if (survey == null)
            {
                return NotFound();
            }

            // Map domain model to view model
            var viewModel = MapDomainToViewModel(survey);

            return View("Create", viewModel);
        }

        private Survey MapViewModelToDomain(CreateSurveyViewModel model)
        {
            var survey = new Survey
            {
                Id = model.Id,
                Title = model.Title,
                Description = model.Description,
                Status = model.Status,
                Questions = model.Questions.Select(q => new Question
                {
                    Id = int.TryParse(q.Id, out var id) ? id : 0,
                    Text = q.Title,
                    Description = q.Description,
                    Type = q.Type,
                    Required = q.Required,
                    Options = q.Options,
                    Settings = q.Settings != null ? new QuestionSettings
                    {
                        Min = q.Settings.Min,
                        Max = q.Settings.Max
                    } : null
                }).ToList()
            };

            // Map delivery configuration if it exists
            if (model.DeliveryConfig != null)
            {
                survey.DeliveryConfig = new DeliveryConfiguration
                {
                    Type = model.DeliveryConfig.Type,
                    EmailAddresses = model.DeliveryConfig.EmailAddresses,
                    Schedule = model.DeliveryConfig.Schedule != null ? new ScheduleSettings
                    {
                        Frequency = model.DeliveryConfig.Schedule.Frequency,
                        DayOfMonth = model.DeliveryConfig.Schedule.DayOfMonth,
                        Time = model.DeliveryConfig.Schedule.Time
                    } : null,
                    Trigger = model.DeliveryConfig.Trigger != null ? new TriggerSettings
                    {
                        Type = model.DeliveryConfig.Trigger.Type,
                        DelayHours = model.DeliveryConfig.Trigger.DelayHours,
                        SendAutomatically = model.DeliveryConfig.Trigger.SendAutomatically
                    } : null
                };
            }

            return survey;
        }

        private CreateSurveyViewModel MapDomainToViewModel(Survey survey)
        {
            var viewModel = new CreateSurveyViewModel
            {
                Id = survey.Id,
                Title = survey.Title,
                Description = survey.Description ?? string.Empty,
                Status = survey.Status ?? "draft",
                Questions = survey.Questions.Select(q => new SurveyQuestionViewModel
                {
                    Id = q.Id.ToString(),
                    Title = q.Text,
                    Description = q.Description ?? string.Empty,
                    Type = q.Type,
                    Required = q.Required,
                    Options = q.Options,
                    Settings = q.Settings != null ? new QuestionSettingsViewModel
                    {
                        Min = q.Settings.Min,
                        Max = q.Settings.Max
                    } : null
                }).ToList()
            };

            // Map delivery configuration if it exists
            if (survey.DeliveryConfig != null)
            {
                viewModel.DeliveryConfig = new DeliveryConfigViewModel
                {
                    Type = survey.DeliveryConfig.Type,
                    EmailAddresses = survey.DeliveryConfig.EmailAddresses,
                    Schedule = survey.DeliveryConfig.Schedule != null ? new ScheduleSettingsViewModel
                    {
                        Frequency = survey.DeliveryConfig.Schedule.Frequency,
                        DayOfMonth = survey.DeliveryConfig.Schedule.DayOfMonth,
                        Time = survey.DeliveryConfig.Schedule.Time
                    } : null,
                    Trigger = survey.DeliveryConfig.Trigger != null ? new TriggerSettingsViewModel
                    {
                        Type = survey.DeliveryConfig.Trigger.Type,
                        DelayHours = survey.DeliveryConfig.Trigger.DelayHours,
                        SendAutomatically = survey.DeliveryConfig.Trigger.SendAutomatically
                    } : null
                };
            }

            return viewModel;
        }
    }
}
