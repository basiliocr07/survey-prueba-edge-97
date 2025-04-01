
using System;
using System.Collections.Generic;

namespace SurveyApp.Domain.Models
{
    public class SurveyStatistics
    {
        public int TotalResponses { get; set; }
        public double AverageCompletionTime { get; set; }
        public int CompletionRate { get; set; }
        public List<QuestionStatistics> QuestionStats { get; set; } = new List<QuestionStatistics>();
    }

    public class QuestionStatistics
    {
        public string QuestionId { get; set; }
        public string QuestionTitle { get; set; }
        public string QuestionText { get; set; }
        public Dictionary<string, ResponseDistribution> ResponseDistribution { get; set; } = new Dictionary<string, ResponseDistribution>();
    }

    public class ResponseDistribution
    {
        public int Count { get; set; }
        public double Percentage { get; set; }
    }
}
