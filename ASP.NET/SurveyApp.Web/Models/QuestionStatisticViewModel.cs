
using System.Collections.Generic;

namespace SurveyApp.Web.Models
{
    public class QuestionStatisticViewModel
    {
        public string QuestionId { get; set; }
        public string QuestionTitle { get; set; }
        public string QuestionText { get; set; }
        public List<AnswerStatisticViewModel> Responses { get; set; } = new List<AnswerStatisticViewModel>();
        public Dictionary<string, ResponseDistributionViewModel> ResponseDistribution { get; set; } = new Dictionary<string, ResponseDistributionViewModel>();
    }

    public class AnswerStatisticViewModel
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
}
