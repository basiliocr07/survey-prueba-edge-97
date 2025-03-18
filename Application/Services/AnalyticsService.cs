using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SurveyApp.Application.DTOs;
using SurveyApp.Application.Ports;
using SurveyApp.Domain.Entities;

namespace SurveyApp.Application.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IAnalyticsRepository _analyticsRepository;
        private readonly ISurveyRepository _surveyRepository;

        public AnalyticsService(IAnalyticsRepository analyticsRepository, ISurveyRepository surveyRepository)
        {
            _analyticsRepository = analyticsRepository;
            _surveyRepository = surveyRepository;
        }

        public async Task<AnalyticsDto> GetAnalyticsDataAsync()
        {
            var analyticsData = await _analyticsRepository.GetAnalyticsDataAsync();
            var random = new Random();
            
            var dto = new AnalyticsDto
            {
                TotalSurveys = analyticsData.TotalSurveys,
                TotalResponses = analyticsData.TotalResponses,
                AverageCompletionRate = analyticsData.AverageCompletionRate,
                QuestionTypeDistribution = analyticsData.QuestionTypeDistribution,
                ResponseTrends = analyticsData.ResponseTrends.Select(rt => new SurveyResponseTrendDto
                {
                    Date = rt.Date,
                    Responses = rt.Responses
                }).ToList(),
                
                // Additional properties with sample data
                SurveyGrowthRate = random.Next(5, 25),
                ResponseGrowthRate = random.Next(5, 30),
                AvgCompletionRate = Math.Round(analyticsData.AverageCompletionRate),
                CompletionRateChange = random.Next(-5, 15),
                AvgResponseTime = random.Next(2, 8),
                ResponseTimeChange = random.Next(-10, 10)
            };
            
            // Add sample survey performance data
            dto.SurveyPerformance = Enumerable.Range(1, 5).Select(i => new SurveyPerformanceDto
            {
                Title = $"Survey {i}",
                ResponseCount = random.Next(50, 200),
                CompletionRate = random.Next(60, 95),
                AverageTimeMinutes = random.Next(2, 10)
            }).ToList();
            
            // Add sample demographic data
            dto.Demographics = new List<DemographicDto>
            {
                new DemographicDto { Category = "18-24", Percentage = random.Next(10, 25) },
                new DemographicDto { Category = "25-34", Percentage = random.Next(20, 35) },
                new DemographicDto { Category = "35-44", Percentage = random.Next(15, 30) },
                new DemographicDto { Category = "45-54", Percentage = random.Next(10, 20) },
                new DemographicDto { Category = "55+", Percentage = random.Next(5, 15) }
            };
            
            // Add sample device distribution data
            dto.DeviceDistribution = new List<DeviceDistributionDto>
            {
                new DeviceDistributionDto { DeviceType = "Desktop", Percentage = random.Next(30, 50) },
                new DeviceDistributionDto { DeviceType = "Mobile", Percentage = random.Next(30, 45) },
                new DeviceDistributionDto { DeviceType = "Tablet", Percentage = random.Next(10, 20) },
                new DeviceDistributionDto { DeviceType = "Other", Percentage = random.Next(2, 8) }
            };
            
            return dto;
        }

        public async Task RefreshAnalyticsDataAsync()
        {
            var surveys = await _surveyRepository.GetAllAsync();
            
            var analyticsData = new AnalyticsData();
            
            // Calculate metrics
            int totalSurveys = surveys.Count;
            int totalResponses = surveys.Sum(s => s.Responses);
            double averageCompletionRate = surveys.Count > 0 
                ? surveys.Average(s => s.CompletionRate) 
                : 0;
            
            analyticsData.UpdateMetrics(totalSurveys, totalResponses, averageCompletionRate);
            
            // Calculate question type distribution
            var questionTypeDistribution = new Dictionary<string, int>();
            foreach (var survey in surveys)
            {
                foreach (var question in survey.Questions)
                {
                    string type = question.Type.ToString();
                    if (questionTypeDistribution.ContainsKey(type))
                    {
                        questionTypeDistribution[type]++;
                    }
                    else
                    {
                        questionTypeDistribution[type] = 1;
                    }
                }
            }
            analyticsData.SetQuestionTypeDistribution(questionTypeDistribution);
            
            // Calculate response trends (simplified)
            var lastSixMonths = Enumerable.Range(0, 6)
                .Select(i => DateTime.UtcNow.AddMonths(-i).ToString("MMM yyyy"))
                .ToList();
            
            var random = new Random();
            foreach (var month in lastSixMonths)
            {
                analyticsData.AddResponseTrend(new SurveyResponseTrend
                {
                    Date = month,
                    Responses = random.Next(50, 200) // Placeholder data
                });
            }
            
            await _analyticsRepository.UpdateAnalyticsDataAsync(analyticsData);
        }
    }
}
