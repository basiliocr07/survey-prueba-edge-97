
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
            var viewModel = responses.Select(r => new SurveyResponseViewModel
            {
                Id = r.Id,
                SurveyId = r.SurveyId,
                SurveyTitle = survey.Title,
                RespondentName = r.RespondentName,
                RespondentEmail = r.RespondentEmail,
                RespondentPhone = r.RespondentPhone,
                RespondentCompany = r.RespondentCompany,
                SubmittedAt = r.SubmittedAt,
                Answers = r.Answers.Select(a => new QuestionResponseViewModel
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

            var viewModel = new SurveyDetailViewModel
            {
                Id = survey.Id,
                Title = survey.Title,
                Description = survey.Description,
                Questions = survey.Questions.Select(q => new SurveyQuestionViewModel
                {
                    Id = q.Id.ToString(),
                    Type = q.Type,
                    Question = q.Text,
                    Text = q.Text,
                    Title = q.Text,
                    Description = q.Description ?? "",
                    Options = q.Options,
                    Required = q.Required
                }).ToList()
            };

            return View(viewModel);
        }

        // POST: /SurveyResponses/Submit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(int surveyId, SurveySubmissionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Invalid model state when submitting survey response");
                return RedirectToAction(nameof(Take), new { id = surveyId });
            }

            var survey = await _surveyService.GetSurveyByIdAsync(surveyId);
            if (survey == null)
            {
                _logger.LogWarning($"Survey with ID {surveyId} not found");
                return NotFound();
            }

            try
            {
                var response = new SurveyResponse
                {
                    SurveyId = surveyId,
                    RespondentName = model.Name,
                    RespondentEmail = model.Email,
                    RespondentPhone = model.Phone,
                    RespondentCompany = model.Company,
                    SubmittedAt = DateTime.UtcNow,
                    Answers = new List<QuestionResponse>()
                };

                foreach (var question in survey.Questions)
                {
                    var questionId = question.Id.ToString();
                    if (model.Answers.TryGetValue(questionId, out string value))
                    {
                        response.Answers.Add(new QuestionResponse
                        {
                            QuestionId = questionId,
                            QuestionTitle = question.Text,
                            QuestionType = question.Type,
                            Value = value,
                            IsValid = true // Initial value, will be validated in service
                        });
                    }
                    else if (question.Required)
                    {
                        // If a required question is missing, log it but continue
                        _logger.LogWarning($"Required question {questionId} missing from submission");
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting survey response");
                TempData["ErrorMessage"] = "An unexpected error occurred. Please try again.";
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
