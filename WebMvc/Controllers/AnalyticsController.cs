
using System;
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

        public AnalyticsController(IAnalyticsService analyticsService, ISurveyService surveyService)
        {
            _analyticsService = analyticsService;
            _surveyService = surveyService;
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
            TempData["SuccessMessage"] = "Analytics data has been refreshed successfully.";
            return RedirectToAction(nameof(Index));
        }
        
        [HttpGet]
        public IActionResult AnimatedPartial()
        {
            // This method is for demonstrating animations with partial views
            return PartialView("_AnimatedPartial");
        }
    }
}
