
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
            var model = new CreateSurveyViewModel
            {
                Questions = new List<SurveyQuestionViewModel>
                {
                    new SurveyQuestionViewModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        Title = "Nueva pregunta",
                        Type = "text",
                        Required = true
                    }
                },
                DeliveryConfig = new DeliveryConfigViewModel
                {
                    Type = "manual",
                    EmailAddresses = new List<string>()
                }
            };
            
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateSurveyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Por favor, corrija los errores en el formulario.";
                return View(model);
            }

            try
            {
                // Mapear de ViewModel a entidad de dominio
                var survey = new Survey
                {
                    Id = model.Id,
                    Title = model.Title,
                    Description = model.Description,
                    Status = model.Status,
                    CreatedAt = DateTime.Now,
                    Questions = model.Questions.Select(q => new Question
                    {
                        Id = string.IsNullOrEmpty(q.Id) || q.Id.StartsWith("new-") 
                            ? 0 
                            : int.Parse(q.Id),
                        Text = q.Title,
                        Type = q.Type,
                        Required = q.Required,
                        Description = q.Description ?? string.Empty,
                        Options = q.Options ?? new List<string>(),
                        Settings = q.Settings != null ? new QuestionSettings
                        {
                            Min = q.Settings.Min,
                            Max = q.Settings.Max
                        } : null
                    }).ToList(),
                    DeliveryConfig = new DeliveryConfiguration
                    {
                        Type = model.DeliveryConfig?.Type ?? "manual",
                        EmailAddresses = model.DeliveryConfig?.EmailAddresses ?? new List<string>(),
                        Schedule = model.DeliveryConfig?.Schedule != null ? new ScheduleSettings
                        {
                            Frequency = model.DeliveryConfig.Schedule.Frequency,
                            DayOfMonth = model.DeliveryConfig.Schedule.DayOfMonth,
                            Time = model.DeliveryConfig.Schedule.Time
                        } : null,
                        Trigger = model.DeliveryConfig?.Trigger != null ? new TriggerSettings
                        {
                            Type = model.DeliveryConfig.Trigger.Type,
                            DelayHours = model.DeliveryConfig.Trigger.DelayHours,
                            SendAutomatically = model.DeliveryConfig.Trigger.SendAutomatically
                        } : null
                    }
                };

                bool success;
                if (model.Id > 0)
                {
                    success = await _surveyService.UpdateSurveyAsync(survey);
                    if (success)
                    {
                        TempData["SuccessMessage"] = "Encuesta actualizada exitosamente.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Error al actualizar la encuesta.";
                    }
                }
                else
                {
                    success = await _surveyService.CreateSurveyAsync(survey);
                    if (success)
                    {
                        TempData["SuccessMessage"] = "Encuesta creada exitosamente.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Error al crear la encuesta.";
                    }
                }

                if (success)
                {
                    return RedirectToAction("Index", "Surveys");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                TempData["ErrorMessage"] = $"Error inesperado: {ex.Message}";
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var survey = await _surveyService.GetSurveyByIdAsync(id);
            if (survey == null)
            {
                TempData["ErrorMessage"] = "No se encontró la encuesta solicitada.";
                return RedirectToAction("Index", "Surveys");
            }

            // Mapear de entidad de dominio a ViewModel usando explícitamente SurveyApp.Web.Models
            var model = new SurveyApp.Web.Models.CreateSurveyViewModel
            {
                Id = survey.Id,
                Title = survey.Title,
                Description = survey.Description,
                Status = survey.Status,
                Questions = survey.Questions.Select(q => new SurveyApp.Web.Models.SurveyQuestionViewModel
                {
                    Id = q.Id.ToString(),
                    Title = q.Text,
                    Type = q.Type,
                    Required = q.Required,
                    Description = q.Description,
                    Options = q.Options,
                    Settings = q.Settings != null ? new SurveyApp.Web.Models.QuestionSettingsViewModel
                    {
                        Min = q.Settings.Min,
                        Max = q.Settings.Max
                    } : null
                }).ToList(),
                DeliveryConfig = survey.DeliveryConfig != null ? new SurveyApp.Web.Models.DeliveryConfigViewModel
                {
                    Type = survey.DeliveryConfig.Type,
                    EmailAddresses = survey.DeliveryConfig.EmailAddresses,
                    Schedule = survey.DeliveryConfig.Schedule != null ? new SurveyApp.Web.Models.ScheduleSettingsViewModel
                    {
                        Frequency = survey.DeliveryConfig.Schedule.Frequency,
                        DayOfMonth = survey.DeliveryConfig.Schedule.DayOfMonth,
                        Time = survey.DeliveryConfig.Schedule.Time
                    } : null,
                    Trigger = survey.DeliveryConfig.Trigger != null ? new SurveyApp.Web.Models.TriggerSettingsViewModel
                    {
                        Type = survey.DeliveryConfig.Trigger.Type,
                        DelayHours = survey.DeliveryConfig.Trigger.DelayHours,
                        SendAutomatically = survey.DeliveryConfig.Trigger.SendAutomatically
                    } : null
                } : new SurveyApp.Web.Models.DeliveryConfigViewModel()
            };

            return View("Create", model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
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

            return RedirectToAction("Index", "Surveys");
        }

        [HttpPost]
        public async Task<IActionResult> SendEmails(int surveyId, List<string> emailAddresses)
        {
            try
            {
                var result = await _surveyService.SendSurveyEmailsAsync(surveyId, emailAddresses);
                if (result)
                {
                    TempData["SuccessMessage"] = "Correos enviados exitosamente.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al enviar los correos.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error al enviar los correos: {ex.Message}";
            }

            return RedirectToAction("Edit", new { id = surveyId });
        }
    }
}
