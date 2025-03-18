
using System;
using System.Collections.Generic;

namespace SurveyApp.WebMvc.Models
{
    public class AnalyticsViewModel
    {
        public int TotalSurveys { get; set; }
        public int TotalResponses { get; set; }
        public double AverageCompletionRate { get; set; }
        public Dictionary<string, int> QuestionTypeDistribution { get; set; } = new Dictionary<string, int>();
        public List<ResponseTrendViewModel> ResponseTrends { get; set; } = new List<ResponseTrendViewModel>();
        public List<SurveyOverviewViewModel> RecentSurveys { get; set; } = new List<SurveyOverviewViewModel>();
        
        // Additional properties needed by the view
        public double SurveyGrowthRate { get; set; }
        public double ResponseGrowthRate { get; set; }
        public double AvgCompletionRate { get; set; }
        public double CompletionRateChange { get; set; }
        public double AvgResponseTime { get; set; }
        public double ResponseTimeChange { get; set; }
        public List<SurveyPerformanceViewModel> SurveyPerformance { get; set; } = new List<SurveyPerformanceViewModel>();
        public List<DemographicViewModel> Demographics { get; set; } = new List<DemographicViewModel>();
        public List<DeviceDistributionViewModel> DeviceDistribution { get; set; } = new List<DeviceDistributionViewModel>();
    }

    public class ResponseTrendViewModel
    {
        public string Date { get; set; }
        public int Responses { get; set; }
        public string Period { get; set; }
        public int ResponseCount { get; set; }
    }

    public class SurveyOverviewViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public int Responses { get; set; }
        public int CompletionRate { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class SurveyPerformanceViewModel
    {
        public string Title { get; set; }
        public int ResponseCount { get; set; }
        public int CompletionRate { get; set; }
        public int AverageTimeMinutes { get; set; }
    }

    public class DemographicViewModel
    {
        public string Category { get; set; }
        public double Percentage { get; set; }
    }

    public class DeviceDistributionViewModel
    {
        public string DeviceType { get; set; }
        public double Percentage { get; set; }
    }
}
