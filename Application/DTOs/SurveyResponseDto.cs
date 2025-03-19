
using System;
using System.Collections.Generic;

namespace SurveyApp.Application.DTOs
{
    public class SurveyResponseDto
    {
        public Guid Id { get; set; }
        public Guid SurveyId { get; set; }
        public string RespondentName { get; set; }
        public string RespondentEmail { get; set; }
        public string RespondentPhone { get; set; }
        public string RespondentCompany { get; set; }
        public DateTime SubmittedAt { get; set; }
        public List<QuestionResponseDto> Answers { get; set; }
        public bool IsExistingClient { get; set; }
        public Guid? ExistingClientId { get; set; }
    }

    public class QuestionResponseDto
    {
        public Guid QuestionId { get; set; }
        public string QuestionTitle { get; set; }
        public string QuestionType { get; set; }
        public string Answer { get; set; }
        public List<string> MultipleAnswers { get; set; }
        public bool IsValid { get; set; } = true;
    }

    public class CreateSurveyResponseDto
    {
        public Guid SurveyId { get; set; }
        public string RespondentName { get; set; }
        public string RespondentEmail { get; set; }
        public string RespondentPhone { get; set; }
        public string RespondentCompany { get; set; }
        public Dictionary<string, object> Answers { get; set; }
        public bool IsExistingClient { get; set; }
        public Guid? ExistingClientId { get; set; }
    }
}
