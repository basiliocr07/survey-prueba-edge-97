
using System.Collections.Generic;

namespace SurveyApp.Web.Models
{
    public class SurveyStatisticsViewModel
    {
        public int TotalResponses { get; set; }
        public double AverageCompletionTime { get; set; }
        public int CompletionRate { get; set; }
        public List<QuestionStatisticViewModel> QuestionStats { get; set; } = new List<QuestionStatisticViewModel>();
    }
}
