
using System;
using System.Collections.Generic;

namespace SurveyApp.Domain.Entities
{
    public class AnalyticsData
    {
        public Guid Id { get; private set; }
        public int TotalSurveys { get; private set; }
        public int TotalResponses { get; private set; }
        public double AverageCompletionRate { get; private set; }
        public Dictionary<string, int> QuestionTypeDistribution { get; private set; } = new Dictionary<string, int>();
        public List<SurveyResponseTrend> ResponseTrends { get; private set; } = new List<SurveyResponseTrend>();

        public AnalyticsData()
        {
            Id = Guid.NewGuid();
        }

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
}
