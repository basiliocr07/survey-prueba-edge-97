
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

        public AnalyticsData()
        {
            Id = Guid.NewGuid();
        }

        public void AddResponseTrend(SurveyResponseTrend trend)
        {
            ResponseTrends.Add(trend);
        }

        public void UpdateTotals(int surveyCount, int responseCount)
        {
            TotalSurveys = surveyCount;
            TotalResponses = responseCount;
            
            if (surveyCount > 0 && responseCount > 0)
            {
                AverageCompletionRate = (double)responseCount / surveyCount;
            }
        }

        public void UpdateQuestionTypeDistribution(Dictionary<string, int> distribution)
        {
            QuestionTypeDistribution = distribution;
        }
    }
}
