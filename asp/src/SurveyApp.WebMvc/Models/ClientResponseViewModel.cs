
using System;
using System.Collections.Generic;

namespace SurveyApp.WebMvc.Models
{
    public class ClientResponseViewModel
    {
        public Guid SurveyId { get; set; }
        public string SurveyTitle { get; set; } = string.Empty;
        public string SurveyDescription { get; set; } = string.Empty;
        public string RespondentName { get; set; } = string.Empty;
        public string RespondentEmail { get; set; } = string.Empty;
        public string RespondentPhone { get; set; } = string.Empty;
        public string RespondentCompany { get; set; } = string.Empty;
        public bool IsExistingClient { get; set; }
        public string? ExistingClientId { get; set; }
        public List<QuestionAnswerViewModel> Answers { get; set; } = new List<QuestionAnswerViewModel>();
        public DateTime SubmittedAt { get; set; } = DateTime.Now;
        public int? CompletionTime { get; set; }
    }

    public class QuestionAnswerViewModel
    {
        public Guid QuestionId { get; set; }
        public string QuestionTitle { get; set; } = string.Empty;
        public string QuestionType { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public List<string> MultipleAnswers { get; set; } = new List<string>();
        public bool IsValid { get; set; } = true;
    }
}
