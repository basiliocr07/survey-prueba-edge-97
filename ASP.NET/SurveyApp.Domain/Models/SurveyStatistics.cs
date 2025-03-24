
using System;

namespace SurveyApp.Domain.Models
{
    public class SurveyStatistics
    {
        public int SurveyId { get; set; }
        public int TotalResponses { get; set; }
        public int CompletionRate { get; set; }
        public int AverageCompletionTime { get; set; } // En segundos
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<QuestionStatistic> QuestionStats { get; set; } = new List<QuestionStatistic>();
    }

    public class QuestionStatistic
    {
        public string QuestionId { get; set; }
        public string QuestionTitle { get; set; }
        public List<AnswerStatistic> Responses { get; set; } = new List<AnswerStatistic>();
    }

    public class AnswerStatistic
    {
        public string Answer { get; set; }
        public int Count { get; set; }
        public double Percentage { get; set; }
    }
}
