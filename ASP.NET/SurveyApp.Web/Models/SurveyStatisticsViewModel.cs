
using System.Collections.Generic;

namespace SurveyApp.Web.Models
{
    public class SurveyStatisticsViewModel
    {
        public int TotalResponses { get; set; }
        public double AverageCompletionTime { get; set; }
        public double CompletionRate { get; set; }
        public List<QuestionStatisticViewModel> QuestionStats { get; set; } = new List<QuestionStatisticViewModel>();
    }

    public class QuestionStatisticViewModel
    {
        public string QuestionId { get; set; }
        public string QuestionTitle { get; set; }
        public string QuestionText { get; set; }
        public List<ResponseViewModel> Responses { get; set; } = new List<ResponseViewModel>();
        public Dictionary<string, ResponseDistributionViewModel> ResponseDistribution { get; set; } = new Dictionary<string, ResponseDistributionViewModel>();
    }

    public class ResponseViewModel
    {
        public string Answer { get; set; }
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    public class ResponseDistributionViewModel
    {
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    public class SurveyResultsViewModel
    {
        public SurveyViewModel Survey { get; set; }
        public SurveyStatisticsViewModel Statistics { get; set; }
    }

    public class SurveyViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public int ResponseCount { get; set; }
        public double CompletionRate { get; set; }
    }
}
