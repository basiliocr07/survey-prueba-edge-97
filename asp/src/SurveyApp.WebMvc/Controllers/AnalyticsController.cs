
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SurveyApp.Application.DTOs;
using SurveyApp.Application.Services;
using SurveyApp.WebMvc.Models;

namespace SurveyApp.WebMvc.Controllers
{
    public class AnalyticsController : Controller
    {
        private readonly ISurveyService _surveyService;
        private readonly ISurveyResponseService _surveyResponseService;
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(
            ISurveyService surveyService,
            ISurveyResponseService surveyResponseService,
            IAnalyticsService analyticsService)
        {
            _surveyService = surveyService;
            _surveyResponseService = surveyResponseService;
            _analyticsService = analyticsService;
        }

        public IActionResult Results(Guid? surveyId = null)
        {
            var surveysDto = _surveyService.GetAllSurveys();
            var surveys = surveysDto.Select(s => new SurveyOverviewViewModel
            {
                Id = s.Id,
                Title = s.Title,
                Responses = _surveyResponseService.GetSurveyResponseCount(s.Id),
                CompletionRate = _analyticsService.GetSurveyCompletionRate(s.Id),
                CreatedAt = s.CreatedAt
            }).ToList();
            
            // Si no se especifica una encuesta, seleccionar la primera
            if (surveyId == null && surveys.Count > 0)
            {
                surveyId = surveys[0].Id;
            }
            
            // Obtener la encuesta seleccionada
            var selectedSurvey = surveys.FirstOrDefault(s => s.Id == surveyId) ?? surveys.FirstOrDefault();
            
            // Obtener las respuestas de la encuesta seleccionada
            var responses = new List<SurveyResponseAnalyticsViewModel>();
            if (selectedSurvey != null)
            {
                var responsesDto = _surveyResponseService.GetSurveyResponsesById(selectedSurvey.Id);
                
                responses = responsesDto.Select(r => new SurveyResponseAnalyticsViewModel
                {
                    Id = r.Id,
                    SurveyId = r.SurveyId,
                    SurveyTitle = selectedSurvey.Title,
                    RespondentName = r.RespondentName,
                    RespondentEmail = r.RespondentEmail,
                    RespondentPhone = r.RespondentPhone ?? string.Empty,
                    RespondentCompany = r.RespondentCompany ?? string.Empty,
                    SubmittedAt = r.SubmittedAt,
                    CompletionTime = r.CompletionTime,
                    Answers = r.Answers.Select(a => new QuestionAnswerViewModel
                    {
                        QuestionId = a.QuestionId,
                        QuestionTitle = _surveyService.GetQuestionTitle(a.QuestionId),
                        QuestionType = _surveyService.GetQuestionType(a.QuestionId),
                        Answer = a.Answer,
                        IsValid = true // Simplificado para este ejemplo
                    }).ToList()
                }).ToList();
            }

            var viewModel = new ResultsViewModel
            {
                Surveys = surveys,
                SelectedSurveyId = selectedSurvey?.Id ?? Guid.Empty,
                SelectedSurvey = selectedSurvey,
                Responses = responses
            };

            return View(viewModel);
        }
        
        [HttpGet]
        public IActionResult ResponseDetail(Guid id)
        {
            var responseDto = _surveyResponseService.GetSurveyResponseById(id);
            if (responseDto == null)
            {
                return NotFound();
            }
            
            var surveyDto = _surveyService.GetSurveyById(responseDto.SurveyId);
            if (surveyDto == null)
            {
                return NotFound();
            }
            
            var viewModel = new SurveyResponseAnalyticsViewModel
            {
                Id = responseDto.Id,
                SurveyId = responseDto.SurveyId,
                SurveyTitle = surveyDto.Title,
                RespondentName = responseDto.RespondentName,
                RespondentEmail = responseDto.RespondentEmail,
                RespondentPhone = responseDto.RespondentPhone ?? string.Empty,
                RespondentCompany = responseDto.RespondentCompany ?? string.Empty,
                SubmittedAt = responseDto.SubmittedAt,
                CompletionTime = responseDto.CompletionTime,
                Answers = responseDto.Answers.Select(a => new QuestionAnswerViewModel
                {
                    QuestionId = a.QuestionId,
                    QuestionTitle = _surveyService.GetQuestionTitle(a.QuestionId),
                    QuestionType = _surveyService.GetQuestionType(a.QuestionId),
                    Answer = a.Answer,
                    IsValid = true // Simplificado para este ejemplo
                }).ToList()
            };
            
            return View(viewModel);
        }
    }
    
    public class ResponsesAnalyticsViewModel
    {
        public List<SurveyOverviewViewModel> Surveys { get; set; } = new List<SurveyOverviewViewModel>();
        public Guid? SelectedSurveyId { get; set; }
        public List<SurveyResponseAnalyticsViewModel> Responses { get; set; } = new List<SurveyResponseAnalyticsViewModel>();
    }
    
    public class ResultsViewModel
    {
        public List<SurveyOverviewViewModel> Surveys { get; set; } = new List<SurveyOverviewViewModel>();
        public Guid SelectedSurveyId { get; set; }
        public SurveyOverviewViewModel? SelectedSurvey { get; set; }
        public List<SurveyResponseAnalyticsViewModel> Responses { get; set; } = new List<SurveyResponseAnalyticsViewModel>();
    }
}
