
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SurveyApp.Application.Services;
using SurveyApp.WebMvc.Models;
using System.Linq;
using System;
using System.Collections.Generic;

namespace SurveyApp.WebMvc.Controllers
{
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

            var random = new Random();

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
                
                // Add mock data for additional properties
                SurveyGrowthRate = random.Next(5, 25),
                ResponseGrowthRate = random.Next(5, 30),
                AvgCompletionRate = Math.Round(analyticsData.AverageCompletionRate),
                CompletionRateChange = random.Next(-5, 15),
                AvgResponseTime = random.Next(2, 8),
                ResponseTimeChange = random.Next(-10, 10),
                
                // Add mock survey performance data
                SurveyPerformance = surveys.Take(5).Select(s => new SurveyPerformanceViewModel
                {
                    Title = s.Title,
                    ResponseCount = s.Responses,
                    CompletionRate = s.CompletionRate,
                    AverageTimeMinutes = random.Next(2, 10)
                }).ToList(),
                
                // Add mock demographic data
                Demographics = new List<DemographicViewModel>
                {
                    new DemographicViewModel { Category = "18-24", Percentage = random.Next(10, 25) },
                    new DemographicViewModel { Category = "25-34", Percentage = random.Next(20, 35) },
                    new DemographicViewModel { Category = "35-44", Percentage = random.Next(15, 30) },
                    new DemographicViewModel { Category = "45-54", Percentage = random.Next(10, 20) },
                    new DemographicViewModel { Category = "55+", Percentage = random.Next(5, 15) }
                },
                
                // Add mock device distribution data
                DeviceDistribution = new List<DeviceDistributionViewModel>
                {
                    new DeviceDistributionViewModel { DeviceType = "Desktop", Percentage = random.Next(30, 50) },
                    new DeviceDistributionViewModel { DeviceType = "Mobile", Percentage = random.Next(30, 45) },
                    new DeviceDistributionViewModel { DeviceType = "Tablet", Percentage = random.Next(10, 20) },
                    new DeviceDistributionViewModel { DeviceType = "Other", Percentage = random.Next(2, 8) }
                }
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
