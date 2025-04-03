
using System.Collections.Generic;

namespace SurveyApp.Web.Models
{
    public class SurveyStatisticsViewModel
    {
        public int TotalResponses { get; set; }
        public double AverageCompletionTime { get; set; }
        public double CompletionRate { get; set; }
        public List<QuestionStatViewModel> QuestionStats { get; set; } = new List<QuestionStatViewModel>();
    }

    public class QuestionStatViewModel
    {
        public string QuestionId { get; set; }
        public string QuestionTitle { get; set; }
        public string QuestionText { get; set; }
        public List<StatResponseViewModel> Responses { get; set; } = new List<StatResponseViewModel>();
        public Dictionary<string, ResponseDistribution> ResponseDistribution { get; set; } = new Dictionary<string, ResponseDistribution>();
    }

    public class StatResponseViewModel
    {
        public string Answer { get; set; }
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    public class ResponseDistribution
    {
        public int Count { get; set; }
        public double Percentage { get; set; }
    }
}
