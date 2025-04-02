
using Microsoft.AspNetCore.Mvc;
using SurveyApp.Application.Interfaces;
using SurveyApp.Domain.Models;
using SurveyApp.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;

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
        public async Task<IActionResult> Index(int? surveyId = null)
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
                SelectedSurveyId = surveyId,
                DeliveryConfig = surveyId.HasValue 
                    ? await GetSurveyDeliveryConfig(surveyId.Value) 
                    : GetGlobalDeliveryConfig(),
                Customers = GetSampleCustomers() // En un entorno real, estos datos vendrían de la base de datos
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetSurveyConfig(int surveyId)
        {
            var config = await GetSurveyDeliveryConfig(surveyId);
            return Json(config);
        }

        [HttpPost]
        public async Task<IActionResult> SaveGlobalConfig([FromBody] DeliveryConfigViewModel config)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Guardar en TempData para simular el localStorage del cliente
            TempData["GlobalEmailConfig"] = JsonSerializer.Serialize(config);
            TempData.Keep("GlobalEmailConfig");

            return Json(new { success = true, message = "Configuración global guardada exitosamente" });
        }

        [HttpPost]
        public async Task<IActionResult> SaveSurveyConfig(int surveyId, [FromBody] DeliveryConfigViewModel config)
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

            bool success = await _surveyService.UpdateSurveyDeliveryConfigAsync(surveyId, survey.DeliveryConfig);

            return Json(new { success, message = success ? "Configuración guardada exitosamente" : "Error al guardar la configuración" });
        }

        [HttpGet]
        public IActionResult GetCustomers(string search = "")
        {
            var customers = GetSampleCustomers();
            
            if (!string.IsNullOrEmpty(search))
            {
                search = search.ToLower();
                customers = customers.Where(c => 
                    c.Name.ToLower().Contains(search) || 
                    c.Email.ToLower().Contains(search)
                ).ToList();
            }
            
            return Json(customers);
        }

        // Helpers
        private DeliveryConfigViewModel GetGlobalDeliveryConfig()
        {
            // Intentar cargar desde TempData (simulando localStorage)
            if (TempData.TryGetValue("GlobalEmailConfig", out var serializedConfig))
            {
                try
                {
                    var config = JsonSerializer.Deserialize<DeliveryConfigViewModel>(serializedConfig.ToString());
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
                EmailAddresses = new List<string>(),
                Schedule = new ScheduleSettingsViewModel 
                { 
                    Frequency = "weekly", 
                    DayOfWeek = 1, 
                    Time = "09:00" 
                },
                Trigger = new TriggerSettingsViewModel
                {
                    Type = "ticket-closed",
                    DelayHours = 24,
                    SendAutomatically = true
                }
            };
        }

        private async Task<DeliveryConfigViewModel> GetSurveyDeliveryConfig(int surveyId)
        {
            var survey = await _surveyService.GetSurveyByIdAsync(surveyId);
            if (survey == null || survey.DeliveryConfig == null)
            {
                return GetGlobalDeliveryConfig();
            }

            return MapDeliveryConfigToViewModel(survey.DeliveryConfig);
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
                    DayOfWeek = viewModel.Schedule.DayOfWeek ?? 1,
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

        // Datos de muestra para clientes
        private List<CustomerViewModel> GetSampleCustomers()
        {
            return new List<CustomerViewModel>
            {
                new CustomerViewModel { Id = 1, Name = "Juan Pérez", Email = "juan.perez@example.com" },
                new CustomerViewModel { Id = 2, Name = "María García", Email = "maria.garcia@example.com" },
                new CustomerViewModel { Id = 3, Name = "Carlos Rodríguez", Email = "carlos.rodriguez@example.com" },
                new CustomerViewModel { Id = 4, Name = "Ana Martínez", Email = "ana.martinez@example.com" },
                new CustomerViewModel { Id = 5, Name = "Pedro López", Email = "pedro.lopez@example.com" },
                new CustomerViewModel { Id = 6, Name = "Laura Sánchez", Email = "laura.sanchez@example.com" },
                new CustomerViewModel { Id = 7, Name = "José Gómez", Email = "jose.gomez@example.com" },
                new CustomerViewModel { Id = 8, Name = "Sofía Torres", Email = "sofia.torres@example.com" },
                new CustomerViewModel { Id = 9, Name = "Miguel Ruiz", Email = "miguel.ruiz@example.com" },
                new CustomerViewModel { Id = 10, Name = "Carmen Díaz", Email = "carmen.diaz@example.com" }
            };
        }
    }
}
