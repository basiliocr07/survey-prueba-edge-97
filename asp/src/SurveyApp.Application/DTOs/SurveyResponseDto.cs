
using System;
using System.Collections.Generic;

namespace SurveyApp.Application.DTOs
{
    public class SurveyResponseDto
    {
        public Guid Id { get; set; }
        public Guid SurveyId { get; set; }
        public string SurveyTitle { get; set; } = string.Empty;
        public string RespondentName { get; set; } = string.Empty;
        public string RespondentEmail { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; }
        public List<QuestionResponseDto> Answers { get; set; } = new List<QuestionResponseDto>();
    }

    public class QuestionResponseDto
    {
        public Guid QuestionId { get; set; }
        public string QuestionTitle { get; set; } = string.Empty;
        public string QuestionType { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public List<string> MultipleAnswers { get; set; } = new List<string>();
    }

    public class CreateSurveyResponseDto
    {
        public Guid SurveyId { get; set; }
        public string RespondentName { get; set; } = string.Empty;
        public string RespondentEmail { get; set; } = string.Empty;
        public string RespondentPhone { get; set; } = string.Empty;
        public string RespondentCompany { get; set; } = string.Empty;
        public Dictionary<string, object> Answers { get; set; } = new Dictionary<string, object>();
        public bool IsExistingClient { get; set; }
        public Guid? ExistingClientId { get; set; }
    }

    public class RecentResponseDto
    {
        public Guid Id { get; set; }
        public Guid SurveyId { get; set; }
        public string SurveyTitle { get; set; } = string.Empty;
        public string RespondentName { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; }
    }
}
