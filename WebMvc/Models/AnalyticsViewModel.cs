
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
        
        // NPS score distribution
        public Dictionary<int, int> NPSDistribution { get; set; } = new Dictionary<int, int>();
        public double NPSScore { get; set; }
        
        // Rating score distribution
        public Dictionary<int, int> RatingDistribution { get; set; } = new Dictionary<int, int>();
        public double AverageRating { get; set; }
        
        // Response time breakdown
        public List<TimeOfDayResponseViewModel> TimeOfDayResponses { get; set; } = new List<TimeOfDayResponseViewModel>();
        public List<DayOfWeekResponseViewModel> DayOfWeekResponses { get; set; } = new List<DayOfWeekResponseViewModel>();
        
        // Nuevas propiedades para mayor análisis
        public int TotalQuestions { get; set; }
        public double AverageQuestionsPerSurvey => TotalSurveys > 0 ? (double)TotalQuestions / TotalSurveys : 0;
        public int SkippedQuestionsCount { get; set; }
        public double SkipRate => TotalQuestions > 0 ? (double)SkippedQuestionsCount / TotalQuestions : 0;
        public Dictionary<string, double> CategoryCompletionRates { get; set; } = new Dictionary<string, double>();
        public List<QuestionPerformanceViewModel> TopPerformingQuestions { get; set; } = new List<QuestionPerformanceViewModel>();
        public List<QuestionPerformanceViewModel> LowPerformingQuestions { get; set; } = new List<QuestionPerformanceViewModel>();
        public Dictionary<string, int> FeedbackSentimentDistribution { get; set; } = new Dictionary<string, int>();
        public List<ResponseTimeRangeViewModel> ResponseTimeRanges { get; set; } = new List<ResponseTimeRangeViewModel>();
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

    public class TimeOfDayResponseViewModel
    {
        public string TimeRange { get; set; }
        public int ResponseCount { get; set; }
        public double Percentage { get; set; }
    }

    public class DayOfWeekResponseViewModel
    {
        public string DayName { get; set; }
        public int ResponseCount { get; set; }
        public double Percentage { get; set; }
    }
    
    public class QuestionAnalyticsViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public int ResponseCount { get; set; }
        public double CompletionRate { get; set; }
        public Dictionary<string, int> AnswerDistribution { get; set; } = new Dictionary<string, int>();
        
        // For rating and NPS questions
        public double AverageScore { get; set; }
        public Dictionary<int, int> ScoreDistribution { get; set; } = new Dictionary<int, int>();
    }
    
    // Nuevas clases para análisis extendido
    public class QuestionPerformanceViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
        public int ResponseCount { get; set; }
        public double CompletionRate { get; set; }
        public double AverageTimeSeconds { get; set; }
        public int SkipCount { get; set; }
    }
    
    public class ResponseTimeRangeViewModel
    {
        public string Range { get; set; }
        public int Count { get; set; }
        public double Percentage { get; set; }
    }
}
