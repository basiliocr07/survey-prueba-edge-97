
using System;
using System.Collections.Generic;

namespace SurveyApp.Domain.Models
{
    public class SurveyStatistics
    {
        public int SurveyId { get; set; }
        public int TotalResponses { get; set; }
        public int CompletionRate { get; set; }
        public int AverageCompletionTime { get; set; } // In seconds
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<QuestionStatistic> QuestionStats { get; set; } = new List<QuestionStatistic>();
    }

    public class QuestionStatistic
    {
        public string QuestionId { get; set; } = string.Empty;
        public string QuestionTitle { get; set; } = string.Empty;
        public string QuestionText { get; set; } = string.Empty; // Added for compatibility
        public List<AnswerStatistic> Responses { get; set; } = new List<AnswerStatistic>();
        public Dictionary<string, ResponseDistributionItem> ResponseDistribution { get; set; } = new Dictionary<string, ResponseDistributionItem>();
    }

    public class AnswerStatistic
    {
        public string Answer { get; set; } = string.Empty;
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    public class ResponseDistributionItem
    {
        public int Count { get; set; }
        public double Percentage { get; set; }
    }
}
