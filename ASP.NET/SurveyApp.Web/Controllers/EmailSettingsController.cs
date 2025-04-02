
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
    public class EmailSettingsController : Controller
    {
        private readonly ISurveyService _surveyService;

        public EmailSettingsController(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Obtener todas las encuestas para el selector
            var surveys = await _surveyService.GetAllSurveysAsync();
            
            // Modelo para la vista
            var model = new EmailSettingsViewModel
            {
                Surveys = surveys.Select(s => new SurveyListItemViewModel
                {
                    Id = s.Id,
                    Title = s.Title,
                    CreatedAt = s.CreatedAt,
                    HasCustomDeliveryConfig = s.DeliveryConfig != null
                }).ToList(),
                DeliveryConfig = GetGlobalDeliveryConfig()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetSurveyConfig(int surveyId)
        {
            var survey = await _surveyService.GetSurveyByIdAsync(surveyId);
            if (survey == null)
            {
                return NotFound();
            }

            var deliveryConfig = survey.DeliveryConfig != null 
                ? MapDeliveryConfigToViewModel(survey.DeliveryConfig) 
                : GetGlobalDeliveryConfig();

            return Json(deliveryConfig);
        }

        [HttpPost]
        public async Task<IActionResult> SaveGlobalConfig(DeliveryConfigViewModel config)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Guardar en TempData para simular el localStorage del cliente
            TempData["GlobalEmailConfig"] = System.Text.Json.JsonSerializer.Serialize(config);

            return Json(new { success = true, message = "Configuración global guardada exitosamente" });
        }

        [HttpPost]
        public async Task<IActionResult> SaveSurveyConfig(int surveyId, DeliveryConfigViewModel config)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var survey = await _surveyService.GetSurveyByIdAsync(surveyId);
            if (survey == null)
            {
                return NotFound();
            }

            survey.DeliveryConfig = MapViewModelToDeliveryConfig(config);

            bool success = await _surveyService.UpdateSurveyAsync(survey);

            return Json(new { success, message = success ? "Configuración guardada exitosamente" : "Error al guardar la configuración" });
        }

        // Helpers
        private DeliveryConfigViewModel GetGlobalDeliveryConfig()
        {
            // Intentar cargar desde TempData (simulando localStorage)
            if (TempData.TryGetValue("GlobalEmailConfig", out var serializedConfig))
            {
                try
                {
                    var config = System.Text.Json.JsonSerializer.Deserialize<DeliveryConfigViewModel>(serializedConfig.ToString());
                    TempData.Keep("GlobalEmailConfig");
                    return config;
                }
                catch
                {
                    // En caso de error, retornar configuración por defecto
                }
            }

            // Configuración por defecto
            return new DeliveryConfigViewModel
            {
                Type = "manual",
                EmailAddresses = new List<string>()
            };
        }

        private DeliveryConfiguration MapViewModelToDeliveryConfig(DeliveryConfigViewModel viewModel)
        {
            return new DeliveryConfiguration
            {
                Type = viewModel.Type,
                EmailAddresses = viewModel.EmailAddresses ?? new List<string>(),
                Schedule = viewModel.Schedule != null ? new ScheduleSettings
                {
                    Frequency = viewModel.Schedule.Frequency,
                    DayOfMonth = viewModel.Schedule.DayOfMonth ?? 1,
                    DayOfWeek = viewModel.Schedule.DayOfWeek,
                    Time = viewModel.Schedule.Time ?? "09:00",
                    StartDate = !string.IsNullOrEmpty(viewModel.Schedule.StartDate) 
                        ? DateTime.Parse(viewModel.Schedule.StartDate) 
                        : null
                } : null,
                Trigger = viewModel.Trigger != null ? new TriggerSettings
                {
                    Type = viewModel.Trigger.Type,
                    DelayHours = viewModel.Trigger.DelayHours,
                    SendAutomatically = viewModel.Trigger.SendAutomatically
                } : null
            };
        }

        private DeliveryConfigViewModel MapDeliveryConfigToViewModel(DeliveryConfiguration config)
        {
            return new DeliveryConfigViewModel
            {
                Type = config.Type,
                EmailAddresses = config.EmailAddresses,
                Schedule = config.Schedule != null ? new ScheduleSettingsViewModel
                {
                    Frequency = config.Schedule.Frequency,
                    DayOfMonth = config.Schedule.DayOfMonth,
                    DayOfWeek = config.Schedule.DayOfWeek,
                    Time = config.Schedule.Time,
                    StartDate = config.Schedule.StartDate?.ToString("yyyy-MM-dd")
                } : null,
                Trigger = config.Trigger != null ? new TriggerSettingsViewModel
                {
                    Type = config.Trigger.Type,
                    DelayHours = config.Trigger.DelayHours,
                    SendAutomatically = config.Trigger.SendAutomatically
                } : null
            };
        }
    }
}
