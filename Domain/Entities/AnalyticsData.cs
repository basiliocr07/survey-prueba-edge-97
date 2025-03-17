
using System;
using System.Collections.Generic;

namespace SurveyApp.Domain.Entities
{
    public class AnalyticsData
    {
        public Guid Id { get; set; }
        public int TotalSurveys { get; set; }
        public int TotalResponses { get; set; }
        public double AverageCompletionRate { get; set; }
        public Dictionary<string, int> QuestionTypeDistribution { get; set; } = new Dictionary<string, int>();
        public List<SurveyResponseTrend> ResponseTrends { get; set; } = new List<SurveyResponseTrend>();

        public void UpdateMetrics(int totalSurveys, int totalResponses, double averageCompletionRate)
        {
            TotalSurveys = totalSurveys;
            TotalResponses = totalResponses;
            AverageCompletionRate = averageCompletionRate;
        }

        public void SetQuestionTypeDistribution(Dictionary<string, int> distribution)
        {
            QuestionTypeDistribution = distribution;
        }

        public void AddResponseTrend(SurveyResponseTrend trend)
        {
            ResponseTrends.Add(trend);
        }
    }

    public class SurveyResponseTrend
    {
        public string Date { get; set; }
        public int Responses { get; set; }
    }
}
