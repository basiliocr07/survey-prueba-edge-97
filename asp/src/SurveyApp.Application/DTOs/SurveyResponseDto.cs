
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
        public string RespondentPhone { get; set; } = string.Empty;
        public string RespondentCompany { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; }
        public List<QuestionResponseDto> Answers { get; set; } = new List<QuestionResponseDto>();
        public bool IsExistingClient { get; set; }
        public Guid? ExistingClientId { get; set; }
        public double CompletionTime { get; set; }
        public string DeviceType { get; set; } = string.Empty;
        public string Browser { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
    }

    public class QuestionResponseDto
    {
        public Guid QuestionId { get; set; }
        public string QuestionTitle { get; set; } = string.Empty;
        public string QuestionType { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public List<string> MultipleAnswers { get; set; } = new List<string>();
    }
}
