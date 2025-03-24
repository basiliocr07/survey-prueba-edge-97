
using System.Collections.Generic;

namespace SurveyApp.Domain.Models
{
    public class SurveyStatisticsViewModel
    {
        public int TotalResponses { get; set; }
        public double AverageCompletionTime { get; set; }
        public int CompletionRate { get; set; }
        public List<QuestionStatistic> QuestionStats { get; set; } = new List<QuestionStatistic>();
    }

    public class QuestionStatistic
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
