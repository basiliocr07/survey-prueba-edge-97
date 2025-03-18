
using System;
using System.Collections.Generic;

namespace SurveyApp.Application.DTOs
{
    public class AnalyticsDto
    {
        public int TotalSurveys { get; set; }
        public int TotalResponses { get; set; }
        public double AverageCompletionRate { get; set; }
        public Dictionary<string, int> QuestionTypeDistribution { get; set; } = new Dictionary<string, int>();
        public List<SurveyResponseTrendDto> ResponseTrends { get; set; } = new List<SurveyResponseTrendDto>();
        
        // Additional properties to match the ViewModel
        public double SurveyGrowthRate { get; set; }
        public double ResponseGrowthRate { get; set; }
        public double AvgCompletionRate { get; set; }
        public double CompletionRateChange { get; set; }
        public double AvgResponseTime { get; set; }
        public double ResponseTimeChange { get; set; }
        public List<SurveyPerformanceDto> SurveyPerformance { get; set; } = new List<SurveyPerformanceDto>();
        public List<DemographicDto> Demographics { get; set; } = new List<DemographicDto>();
        public List<DeviceDistributionDto> DeviceDistribution { get; set; } = new List<DeviceDistributionDto>();
    }

    public class SurveyResponseTrendDto
    {
        public string Date { get; set; }
        public int Responses { get; set; }
    }

    public class SurveyPerformanceDto
    {
        public string Title { get; set; }
        public int ResponseCount { get; set; }
        public int CompletionRate { get; set; }
        public int AverageTimeMinutes { get; set; }
    }

    public class DemographicDto
    {
        public string Category { get; set; }
        public double Percentage { get; set; }
    }

    public class DeviceDistributionDto
    {
        public string DeviceType { get; set; }
        public double Percentage { get; set; }
    }
}
