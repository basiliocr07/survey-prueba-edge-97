
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SurveyApp.Application.Services;
using SurveyApp.WebMvc.Models;

namespace SurveyApp.WebMvc.Controllers
{
    [Authorize(Policy = "AdminOnly")]
    public class AnalyticsController : Controller
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly ISurveyService _surveyService;
        private readonly ILogger<AnalyticsController> _logger;

        public AnalyticsController(
            IAnalyticsService analyticsService, 
            ISurveyService surveyService,
            ILogger<AnalyticsController> logger)
        {
            _analyticsService = analyticsService;
            _surveyService = surveyService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var analyticsData = await _analyticsService.GetAnalyticsDataAsync();
            var surveys = await _surveyService.GetAllSurveysAsync();

            var viewModel = new AnalyticsViewModel
            {
                TotalSurveys = analyticsData.TotalSurveys,
                TotalResponses = analyticsData.TotalResponses,
                AverageCompletionRate = analyticsData.AverageCompletionRate,
                QuestionTypeDistribution = analyticsData.QuestionTypeDistribution,
                ResponseTrends = analyticsData.ResponseTrends.Select(rt => new ResponseTrendViewModel
                {
                    Date = rt.Date,
                    Responses = rt.Responses,
                    Period = rt.Date,
                    ResponseCount = rt.Responses
                }).ToList(),
                RecentSurveys = surveys.OrderByDescending(s => s.CreatedAt)
                    .Take(5)
                    .Select(s => new SurveyOverviewViewModel
                    {
                        Id = s.Id,
                        Title = s.Title,
                        Responses = s.Responses,
                        CompletionRate = s.CompletionRate,
                        CreatedAt = s.CreatedAt
                    }).ToList(),
                
                // Map additional properties from the DTO
                SurveyGrowthRate = analyticsData.SurveyGrowthRate,
                ResponseGrowthRate = analyticsData.ResponseGrowthRate,
                AvgCompletionRate = analyticsData.AvgCompletionRate,
                CompletionRateChange = analyticsData.CompletionRateChange,
                AvgResponseTime = analyticsData.AvgResponseTime,
                ResponseTimeChange = analyticsData.ResponseTimeChange,
                
                // Map survey performance data
                SurveyPerformance = analyticsData.SurveyPerformance.Select(sp => new SurveyPerformanceViewModel
                {
                    Title = sp.Title,
                    ResponseCount = sp.ResponseCount,
                    CompletionRate = sp.CompletionRate,
                    AverageTimeMinutes = sp.AverageTimeMinutes
                }).ToList(),
                
                // Map demographic data
                Demographics = analyticsData.Demographics.Select(d => new DemographicViewModel
                {
                    Category = d.Category,
                    Percentage = d.Percentage
                }).ToList(),
                
                // Map device distribution data
                DeviceDistribution = analyticsData.DeviceDistribution.Select(dd => new DeviceDistributionViewModel
                {
                    DeviceType = dd.DeviceType,
                    Percentage = dd.Percentage
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> RefreshAnalytics()
        {
            // Add a small delay to make the animation more noticeable (similar to React)
            await Task.Delay(TimeSpan.FromMilliseconds(800));
            await _analyticsService.RefreshAnalyticsDataAsync();
            TempData["SuccessMessage"] = "Los datos de análisis se han actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        
        [HttpGet]
        public IActionResult AnimatedPartial()
        {
            // This method is for demonstrating animations with partial views
            return PartialView("_AnimatedPartial");
        }
        
        [HttpGet]
        public async Task<IActionResult> Responses(Guid? surveyId = null)
        {
            try
            {
                var surveys = await _surveyService.GetAllSurveysAsync();
                
                var responses = new List<SurveyResponseAnalyticsViewModel>();
                
                if (surveyId.HasValue)
                {
                    // Get responses for specific survey
                    var surveyResponses = await _surveyService.GetSurveyResponsesAsync(surveyId.Value);
                    var survey = surveys.FirstOrDefault(s => s.Id == surveyId.Value);
                    
                    if (survey != null)
                    {
                        responses = surveyResponses.Select(r => new SurveyResponseAnalyticsViewModel
                        {
                            Id = r.Id,
                            SurveyId = r.SurveyId,
                            SurveyTitle = survey.Title,
                            RespondentName = r.RespondentName,
                            RespondentEmail = r.RespondentEmail,
                            RespondentCompany = r.RespondentCompany,
                            SubmittedAt = r.SubmittedAt,
                            IsValidated = r.Answers.All(a => a.IsValid),
                            Answers = r.Answers.Select(a => new QuestionAnswerViewModel
                            {
                                QuestionId = a.QuestionId,
                                QuestionTitle = a.QuestionTitle,
                                QuestionType = a.QuestionType,
                                Answer = a.Answer,
                                MultipleAnswers = a.MultipleAnswers,
                                IsValid = a.IsValid
                            }).ToList()
                        }).ToList();
                    }
                }
                else
                {
                    // Get responses for all surveys, but limit to most recent ones
                    foreach (var survey in surveys)
                    {
                        try
                        {
                            var surveyResponses = await _surveyService.GetSurveyResponsesAsync(survey.Id);
                            
                            responses.AddRange(surveyResponses.Select(r => new SurveyResponseAnalyticsViewModel
                            {
                                Id = r.Id,
                                SurveyId = r.SurveyId,
                                SurveyTitle = survey.Title,
                                RespondentName = r.RespondentName,
                                RespondentEmail = r.RespondentEmail,
                                RespondentCompany = r.RespondentCompany,
                                SubmittedAt = r.SubmittedAt,
                                IsValidated = r.Answers.All(a => a.IsValid),
                                Answers = r.Answers.Select(a => new QuestionAnswerViewModel
                                {
                                    QuestionId = a.QuestionId,
                                    QuestionTitle = a.QuestionTitle,
                                    QuestionType = a.QuestionType,
                                    Answer = a.Answer,
                                    MultipleAnswers = a.MultipleAnswers,
                                    IsValid = a.IsValid
                                }).ToList()
                            }));
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error al obtener respuestas para la encuesta {SurveyId}", survey.Id);
                        }
                    }
                    
                    // Limit to most recent 50 responses
                    responses = responses.OrderByDescending(r => r.SubmittedAt).Take(50).ToList();
                }
                
                var viewModel = new ResponsesAnalyticsViewModel
                {
                    Surveys = surveys.Select(s => new SurveyListItemViewModel
                    {
                        Id = s.Id,
                        Title = s.Title,
                        ResponseCount = s.Responses
                    }).ToList(),
                    SelectedSurveyId = surveyId,
                    Responses = responses
                };
                
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener respuestas de encuestas");
                TempData["ErrorMessage"] = "Ocurrió un error al obtener las respuestas.";
                return RedirectToAction(nameof(Index));
            }
        }
        
        [HttpGet]
        public async Task<IActionResult> ResponseDetail(Guid id)
        {
            try
            {
                var surveys = await _surveyService.GetAllSurveysAsync();
                
                // Find the survey that contains this response
                SurveyResponseAnalyticsViewModel responseViewModel = null;
                
                foreach (var survey in surveys)
                {
                    try
                    {
                        var surveyResponses = await _surveyService.GetSurveyResponsesAsync(survey.Id);
                        var response = surveyResponses.FirstOrDefault(r => r.Id == id);
                        
                        if (response != null)
                        {
                            responseViewModel = new SurveyResponseAnalyticsViewModel
                            {
                                Id = response.Id,
                                SurveyId = response.SurveyId,
                                SurveyTitle = survey.Title,
                                RespondentName = response.RespondentName,
                                RespondentEmail = response.RespondentEmail,
                                RespondentCompany = response.RespondentCompany,
                                SubmittedAt = response.SubmittedAt,
                                IsValidated = response.Answers.All(a => a.IsValid),
                                Answers = response.Answers.Select(a => new QuestionAnswerViewModel
                                {
                                    QuestionId = a.QuestionId,
                                    QuestionTitle = a.QuestionTitle,
                                    QuestionType = a.QuestionType,
                                    Answer = a.Answer,
                                    MultipleAnswers = a.MultipleAnswers,
                                    IsValid = a.IsValid
                                }).ToList()
                            };
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error al obtener respuesta {ResponseId} para la encuesta {SurveyId}", id, survey.Id);
                    }
                }
                
                if (responseViewModel == null)
                {
                    TempData["ErrorMessage"] = "No se encontró la respuesta solicitada.";
                    return RedirectToAction(nameof(Responses));
                }
                
                return View(responseViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener detalle de respuesta {ResponseId}", id);
                TempData["ErrorMessage"] = "Ocurrió un error al obtener el detalle de la respuesta.";
                return RedirectToAction(nameof(Responses));
            }
        }
    }
    
    public class ResponsesAnalyticsViewModel
    {
        public List<SurveyListItemViewModel> Surveys { get; set; } = new List<SurveyListItemViewModel>();
        public Guid? SelectedSurveyId { get; set; }
        public List<SurveyResponseAnalyticsViewModel> Responses { get; set; } = new List<SurveyResponseAnalyticsViewModel>();
    }
}
