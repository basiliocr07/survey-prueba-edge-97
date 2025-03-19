
using System;
using System.Collections.Generic;

namespace SurveyApp.WebMvc.Models
{
    public class SurveyResponseAnalyticsViewModel
    {
        public Guid Id { get; set; }
        public Guid SurveyId { get; set; }
        public string SurveyTitle { get; set; }
        public string RespondentName { get; set; }
        public string RespondentEmail { get; set; }
        public string RespondentPhone { get; set; }
        public string RespondentCompany { get; set; }
        public DateTime SubmittedAt { get; set; }
        public bool IsValidated { get; set; }
        public double CompletionTime { get; set; }
        public List<QuestionAnswerViewModel> Answers { get; set; } = new List<QuestionAnswerViewModel>();
    }

    public class QuestionAnswerViewModel
    {
        public Guid QuestionId { get; set; }
        public string QuestionTitle { get; set; }
        public string QuestionType { get; set; }
        public string Answer { get; set; }
        public List<string> MultipleAnswers { get; set; }
        public bool IsValid { get; set; }
    }
}
