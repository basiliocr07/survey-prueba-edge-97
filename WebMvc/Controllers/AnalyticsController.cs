
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
                
                SurveyGrowthRate = analyticsData.SurveyGrowthRate,
                ResponseGrowthRate = analyticsData.ResponseGrowthRate,
                AvgCompletionRate = analyticsData.AvgCompletionRate,
                CompletionRateChange = analyticsData.CompletionRateChange,
                AvgResponseTime = analyticsData.AvgResponseTime,
                ResponseTimeChange = analyticsData.ResponseTimeChange,
                
                SurveyPerformance = analyticsData.SurveyPerformance.Select(sp => new SurveyPerformanceViewModel
                {
                    Title = sp.Title,
                    ResponseCount = sp.ResponseCount,
                    CompletionRate = sp.CompletionRate,
                    AverageTimeMinutes = sp.AverageTimeMinutes
                }).ToList(),
                
                Demographics = analyticsData.Demographics.Select(d => new DemographicViewModel
                {
                    Category = d.Category,
                    Percentage = d.Percentage
                }).ToList(),
                
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
            await Task.Delay(TimeSpan.FromMilliseconds(800));
            await _analyticsService.RefreshAnalyticsDataAsync();
            TempData["SuccessMessage"] = "Los datos de análisis se han actualizado correctamente.";
            return RedirectToAction(nameof(Index));
        }
        
        [HttpGet]
        public IActionResult AnimatedPartial()
        {
            return PartialView("_AnimatedPartial");
        }
        
        [HttpGet]
        public async Task<IActionResult> Responses(Guid? surveyId = null)
        {
            try
            {
                var surveys = await _surveyService.GetAllSurveysAsync();
                var responsesDto = await _analyticsService.GetResponsesAnalyticsAsync(surveyId);
                
                var responses = responsesDto.Select(r => new SurveyResponseAnalyticsViewModel
                {
                    Id = r.Id,
                    SurveyId = r.SurveyId,
                    SurveyTitle = r.SurveyTitle,
                    RespondentName = r.RespondentName,
                    RespondentEmail = r.RespondentEmail,
                    RespondentCompany = r.RespondentCompany,
                    SubmittedAt = r.SubmittedAt,
                    IsValidated = r.IsValidated,
                    CompletionTime = r.CompletionTime,
                    QuestionCount = r.QuestionCount,
                    ValidAnswersCount = r.ValidAnswersCount,
                    DeviceType = r.DeviceType,
                    Browser = r.Browser,
                    OperatingSystem = r.OperatingSystem,
                    Location = r.Location,
                    Answers = r.Answers.Select(a => new QuestionAnswerViewModel
                    {
                        QuestionId = a.QuestionId,
                        QuestionTitle = a.QuestionTitle,
                        QuestionType = a.QuestionType,
                        Answer = a.Answer,
                        MultipleAnswers = a.MultipleAnswers,
                        IsValid = a.IsValid,
                        ScoreValue = a.ScoreValue,
                        CompletionTimeSeconds = a.CompletionTimeSeconds,
                        Category = a.Category
                    }).ToList()
                }).ToList();
                
                var viewModel = new ResponsesAnalyticsViewModel
                {
                    Surveys = surveys.Select(s => new SurveyListItemViewModel
                    {
                        Id = s.Id,
                        Title = s.Title,
                        Description = s.Description ?? "",
                        CreatedAt = s.CreatedAt,
                        ResponseCount = s.Responses,
                        Status = s.Status ?? "Active",
                        Responses = s.Responses
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
                var responseDto = await _analyticsService.GetResponseAnalyticsAsync(id);
                
                var viewModel = new SurveyResponseAnalyticsViewModel
                {
                    Id = responseDto.Id,
                    SurveyId = responseDto.SurveyId,
                    SurveyTitle = responseDto.SurveyTitle,
                    RespondentName = responseDto.RespondentName,
                    RespondentEmail = responseDto.RespondentEmail,
                    RespondentPhone = responseDto.RespondentPhone,
                    RespondentCompany = responseDto.RespondentCompany,
                    SubmittedAt = responseDto.SubmittedAt,
                    IsValidated = responseDto.IsValidated,
                    CompletionTime = responseDto.CompletionTime,
                    QuestionCount = responseDto.QuestionCount,
                    ValidAnswersCount = responseDto.ValidAnswersCount,
                    DeviceType = responseDto.DeviceType,
                    Browser = responseDto.Browser,
                    OperatingSystem = responseDto.OperatingSystem,
                    Location = responseDto.Location,
                    Answers = responseDto.Answers.Select(a => new QuestionAnswerViewModel
                    {
                        QuestionId = a.QuestionId,
                        QuestionTitle = a.QuestionTitle,
                        QuestionType = a.QuestionType,
                        Answer = a.Answer,
                        MultipleAnswers = a.MultipleAnswers,
                        IsValid = a.IsValid,
                        ScoreValue = a.ScoreValue,
                        CompletionTimeSeconds = a.CompletionTimeSeconds,
                        Category = a.Category
                    }).ToList()
                };
                
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener detalle de respuesta {ResponseId}", id);
                TempData["ErrorMessage"] = "Ocurrió un error al obtener el detalle de la respuesta.";
                return RedirectToAction(nameof(Responses));
            }
        }
        
        [HttpGet]
        public async Task<IActionResult> SurveyDashboard(Guid id)
        {
            try
            {
                var dashboardData = await _analyticsService.GetSurveyAnalyticsDashboardAsync(id);
                var survey = await _surveyService.GetSurveyByIdAsync(id);
                
                if (survey == null)
                {
                    TempData["ErrorMessage"] = "No se encontró la encuesta solicitada.";
                    return RedirectToAction(nameof(Index));
                }
                
                var viewModel = new SurveyDashboardViewModel
                {
                    Id = survey.Id,
                    Title = survey.Title,
                    Description = survey.Description,
                    ResponseCount = (int)dashboardData["responseCount"],
                    AverageCompletionTime = (double)dashboardData["averageCompletionTime"],
                    ValidationRate = (double)dashboardData["validationRate"],
                    QuestionTypeDistribution = (Dictionary<string, int>)dashboardData["questionTypeDistribution"],
                    CompletionRateByQuestionType = (Dictionary<string, double>)dashboardData["completionRateByQuestionType"],
                    AverageScoreByCategory = (Dictionary<string, double>)dashboardData["averageScoreByCategory"],
                    NPSDistribution = (Dictionary<int, int>)dashboardData["npsDistribution"],
                    RatingDistribution = (Dictionary<int, int>)dashboardData["ratingDistribution"],
                    ResponseTrends = ((List<dynamic>)dashboardData["responseTrends"]).Select(rt => new ResponseTrendViewModel
                    {
                        Date = rt.Date,
                        Responses = rt.Count,
                        Period = rt.Date,
                        ResponseCount = rt.Count
                    }).ToList()
                };
                
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener dashboard para la encuesta {SurveyId}", id);
                TempData["ErrorMessage"] = "Ocurrió un error al obtener el dashboard de la encuesta.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
    
    public class ResponsesAnalyticsViewModel
    {
        public List<SurveyListItemViewModel> Surveys { get; set; } = new List<SurveyListItemViewModel>();
        public Guid? SelectedSurveyId { get; set; }
        public List<SurveyResponseAnalyticsViewModel> Responses { get; set; } = new List<SurveyResponseAnalyticsViewModel>();
    }
    
    public class SurveyDashboardViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int ResponseCount { get; set; }
        public double AverageCompletionTime { get; set; }
        public double ValidationRate { get; set; }
        public Dictionary<string, int> QuestionTypeDistribution { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, double> CompletionRateByQuestionType { get; set; } = new Dictionary<string, double>();
        public Dictionary<string, double> AverageScoreByCategory { get; set; } = new Dictionary<string, double>();
        public Dictionary<int, int> NPSDistribution { get; set; } = new Dictionary<int, int>();
        public Dictionary<int, int> RatingDistribution { get; set; } = new Dictionary<int, int>();
        public List<ResponseTrendViewModel> ResponseTrends { get; set; } = new List<ResponseTrendViewModel>();
        
        public string FormattedAverageCompletionTime
        {
            get
            {
                var timeSpan = TimeSpan.FromSeconds(AverageCompletionTime);
                if (timeSpan.TotalMinutes < 1)
                    return $"{timeSpan.Seconds}s";
                if (timeSpan.TotalHours < 1)
                    return $"{timeSpan.Minutes}m {timeSpan.Seconds}s";
                
                return $"{(int)timeSpan.TotalHours}h {timeSpan.Minutes}m";
            }
        }
        
        public double NPSScore
        {
            get
            {
                if (!NPSDistribution.Any())
                    return 0;
                
                int promoters = NPSDistribution.Where(kv => kv.Key >= 9).Sum(kv => kv.Value);
                int detractors = NPSDistribution.Where(kv => kv.Key <= 6).Sum(kv => kv.Value);
                int total = NPSDistribution.Sum(kv => kv.Value);
                
                if (total == 0)
                    return 0;
                    
                return ((double)promoters / total * 100) - ((double)detractors / total * 100);
            }
        }
        
        public double AverageRating
        {
            get
            {
                if (!RatingDistribution.Any())
                    return 0;
                
                int sum = RatingDistribution.Sum(kv => kv.Key * kv.Value);
                int count = RatingDistribution.Sum(kv => kv.Value);
                
                if (count == 0)
                    return 0;
                    
                return (double)sum / count;
            }
        }
    }
    
    public class SurveyListItemViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int ResponseCount { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public int Responses { get; set; }
    }
}
