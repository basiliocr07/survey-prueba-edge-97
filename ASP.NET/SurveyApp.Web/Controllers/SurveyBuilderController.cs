
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SurveyApp.Application.Interfaces;
using SurveyApp.Application.Surveys.Commands.CreateSurvey;
using SurveyApp.Application.Surveys.Commands.UpdateSurvey;
using SurveyApp.Application.Surveys.Queries.GetSurveyById;
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
        private readonly IMediator _mediator;

        public SurveyBuilderController(ISurveyService surveyService, IMediator mediator)
        {
            _surveyService = surveyService;
            _mediator = mediator;
        }

        [HttpGet]
        public IActionResult Create()
        {
            var model = new CreateSurveyViewModel
            {
                Questions = new List<QuestionViewModel>
                {
                    new QuestionViewModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        Text = "New Question",
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
                // Mapear de ViewModel a entidad de dominio o comando
                if (model.Id > 0)
                {
                    // Actualización
                    var updateCommand = new UpdateSurveyCommand
                    {
                        Id = model.Id,
                        Title = model.Title,
                        Description = model.Description,
                        Status = model.Status,
                        Questions = model.Questions.Select(q => q.ToDomainModel()).ToList(),
                        DeliveryConfig = MapDeliveryConfig(model.DeliveryConfig)
                    };

                    var success = await _mediator.Send(updateCommand);
                    
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
                    // Creación
                    var createCommand = new CreateSurveyCommand(
                        model.Title,
                        model.Description,
                        model.Questions.Select(q => q.ToDomainModel()).ToList(),
                        model.Status,
                        MapDeliveryConfig(model.DeliveryConfig)
                    );

                    var surveyId = await _mediator.Send(createCommand);
                    
                    if (surveyId > 0)
                    {
                        TempData["SuccessMessage"] = "Encuesta creada exitosamente.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Error al crear la encuesta.";
                    }
                }

                return RedirectToAction("Index", "Surveys");
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
            var survey = await _mediator.Send(new GetSurveyByIdQuery(id));
            if (survey == null)
            {
                TempData["ErrorMessage"] = "No se encontró la encuesta solicitada.";
                return RedirectToAction("Index", "Surveys");
            }

            // Mapear de entidad de dominio a ViewModel
            var model = new CreateSurveyViewModel
            {
                Id = survey.Id,
                Title = survey.Title,
                Description = survey.Description,
                Status = survey.Status,
                Questions = survey.Questions.Select(q => new QuestionViewModel
                {
                    Id = q.Id.ToString(),
                    Text = q.Text,
                    Type = q.Type,
                    Required = q.Required,
                    Description = q.Description,
                    Options = q.Options,
                    Settings = q.Settings != null ? new QuestionSettingsViewModel
                    {
                        Min = q.Settings.Min,
                        Max = q.Settings.Max
                    } : null
                }).ToList(),
                DeliveryConfig = MapDeliveryConfigToViewModel(survey.DeliveryConfig)
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

        // Métodos auxiliares para mapeo
        private DeliveryConfiguration? MapDeliveryConfig(DeliveryConfigViewModel? viewModel)
        {
            if (viewModel == null) return null;

            return new DeliveryConfiguration
            {
                Type = viewModel.Type ?? "manual",
                EmailAddresses = viewModel.EmailAddresses ?? new List<string>(),
                Schedule = viewModel.Schedule != null ? new ScheduleSettings
                {
                    Frequency = viewModel.Schedule.Frequency,
                    DayOfMonth = viewModel.Schedule.DayOfMonth ?? 1,
                    DayOfWeek = viewModel.Schedule.DayOfWeek,
                    Time = viewModel.Schedule.Time ?? "09:00"
                } : null,
                Trigger = viewModel.Trigger != null ? new TriggerSettings
                {
                    Type = viewModel.Trigger.Type,
                    DelayHours = viewModel.Trigger.DelayHours,
                    SendAutomatically = viewModel.Trigger.SendAutomatically
                } : null
            };
        }

        private DeliveryConfigViewModel? MapDeliveryConfigToViewModel(DeliveryConfiguration? config)
        {
            if (config == null) return new DeliveryConfigViewModel();

            return new DeliveryConfigViewModel
            {
                Type = config.Type,
                EmailAddresses = config.EmailAddresses,
                Schedule = config.Schedule != null ? new ScheduleSettingsViewModel
                {
                    Frequency = config.Schedule.Frequency,
                    DayOfMonth = config.Schedule.DayOfMonth,
                    DayOfWeek = config.Schedule.DayOfWeek,
                    Time = config.Schedule.Time
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
