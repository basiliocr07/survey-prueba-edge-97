
using System;
using System.Collections.Generic;

namespace SurveyApp.Domain.Models
{
    public class SurveyResponse
    {
        public int Id { get; set; }
        public int SurveyId { get; set; }
        public string RespondentName { get; set; } = string.Empty;
        public string RespondentEmail { get; set; } = string.Empty;
        public string? RespondentPhone { get; set; }
        public string? RespondentCompany { get; set; }
        public DateTime SubmittedAt { get; set; }
        public List<QuestionResponse> Answers { get; set; } = new List<QuestionResponse>();
        public bool? IsExistingClient { get; set; }
        public string? ExistingClientId { get; set; }
        public int? CompletionTime { get; set; }

        // Navigation property
        public Survey? Survey { get; set; }
    }

    public class QuestionResponse
    {
        public int Id { get; set; }
        public int SurveyResponseId { get; set; }
        public string QuestionId { get; set; } = string.Empty;
        public string QuestionTitle { get; set; } = string.Empty;
        public string QuestionType { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public bool IsValid { get; set; }

        // Navigation property
        public SurveyResponse? SurveyResponse { get; set; }
    }
}
