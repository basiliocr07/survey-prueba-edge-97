using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SurveyApp.Application.Interfaces;
using SurveyApp.Domain.Models;
using SurveyApp.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyApp.Web.Controllers
{
    public class SurveyResponsesController : Controller
    {
        private readonly ILogger<SurveyResponsesController> _logger;
        private readonly ISurveyService _surveyService;
        private readonly ISurveyResponseService _responseService;

        public SurveyResponsesController(
            ILogger<SurveyResponsesController> logger,
            ISurveyService surveyService,
            ISurveyResponseService responseService)
        {
            _logger = logger;
            _surveyService = surveyService;
            _responseService = responseService;
        }

        // GET: /SurveyResponses/Index/{surveyId}
        public async Task<IActionResult> Index(int surveyId)
        {
            var survey = await _surveyService.GetSurveyByIdAsync(surveyId);
            if (survey == null)
            {
                return NotFound();
            }

            var responses = await _responseService.GetResponsesBySurveyIdAsync(surveyId);
            var viewModel = responses.Select(r => new SurveyApp.Web.Models.SurveyResponseViewModel
            {
                Id = r.Id,
                SurveyId = r.SurveyId,
                SurveyTitle = survey.Title,
                RespondentName = r.RespondentName,
                RespondentEmail = r.RespondentEmail,
                RespondentPhone = r.RespondentPhone,
                RespondentCompany = r.RespondentCompany,
                SubmittedAt = r.SubmittedAt,
                Answers = r.Answers.Select(a => new SurveyApp.Web.Models.QuestionResponseViewModel
                {
                    QuestionId = a.QuestionId,
                    QuestionTitle = a.QuestionTitle,
                    QuestionType = a.QuestionType,
                    Value = a.Value,
                    IsValid = a.IsValid
                }).ToList(),
                IsExistingClient = r.IsExistingClient ?? false,
                ExistingClientId = r.ExistingClientId,
                CompletionTime = r.CompletionTime ?? 0
            }).ToList();

            ViewBag.SurveyId = surveyId;
            ViewBag.SurveyTitle = survey.Title;

            return View(viewModel);
        }

        // GET: /SurveyResponses/Take/{id}
        public async Task<IActionResult> Take(int id)
        {
            var survey = await _surveyService.GetSurveyByIdAsync(id);
            if (survey == null)
            {
                return NotFound();
            }

            var viewModel = new SurveyApp.Web.Models.SurveyDetailViewModel
            {
                Id = survey.Id,
                Title = survey.Title,
                Description = survey.Description,
                Questions = survey.Questions.Select(q => new SurveyApp.Web.Models.SurveyQuestionViewModel
                {
                    Id = q.Id.ToString(),
                    Type = q.Type,
                    Question = q.Text,
                    Options = q.Options,
                    Required = q.Required
                }).ToList()
            };

            return View(viewModel);
        }

        // POST: /SurveyResponses/Submit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(int surveyId, SurveyApp.Web.Models.SurveySubmissionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(nameof(Take), new { id = surveyId });
            }

            var survey = await _surveyService.GetSurveyByIdAsync(surveyId);
            if (survey == null)
            {
                return NotFound();
            }

            var response = new SurveyApp.Domain.Models.SurveyResponse
            {
                SurveyId = surveyId,
                RespondentName = model.Name,
                RespondentEmail = model.Email,
                RespondentPhone = model.Phone,
                RespondentCompany = model.Company,
                SubmittedAt = DateTime.UtcNow,
                Answers = new List<SurveyApp.Domain.Models.QuestionResponse>()
            };

            foreach (var question in survey.Questions)
            {
                var questionId = question.Id.ToString();
                if (model.Answers.TryGetValue(questionId, out string value))
                {
                    response.Answers.Add(new SurveyApp.Domain.Models.QuestionResponse
                    {
                        QuestionId = questionId,
                        QuestionTitle = question.Text,
                        QuestionType = question.Type,
                        Value = value,
                        IsValid = true // Initial value, will be validated in service
                    });
                }
            }

            var result = await _responseService.SubmitResponseAsync(response);
            if (result)
            {
                TempData["SuccessMessage"] = "Your response has been submitted successfully!";
                return RedirectToAction("ThankYou", new { surveyId });
            }
            else
            {
                TempData["ErrorMessage"] = "There was an error submitting your response. Please try again.";
                return RedirectToAction(nameof(Take), new { id = surveyId });
            }
        }

        // GET: /SurveyResponses/ThankYou
        public IActionResult ThankYou(int surveyId)
        {
            return View(surveyId);
        }
    }
}
