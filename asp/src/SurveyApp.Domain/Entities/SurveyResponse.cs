
using System;
using System.Collections.Generic;

namespace SurveyApp.Domain.Entities
{
    public class SurveyResponse
    {
        public Guid Id { get; set; }
        public Guid SurveyId { get; set; }
        public string RespondentName { get; set; } = string.Empty;
        public string RespondentEmail { get; set; } = string.Empty;
        public string RespondentPhone { get; set; } = string.Empty;
        public string RespondentCompany { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; }
        public List<QuestionResponse> Answers { get; set; } = new List<QuestionResponse>();
        public bool IsExistingClient { get; set; }
        public Guid? ExistingClientId { get; set; }
        
        public SurveyResponse()
        {
            Id = Guid.NewGuid();
            SubmittedAt = DateTime.UtcNow;
            Answers = new List<QuestionResponse>();
        }
    }

    public class QuestionResponse
    {
        public Guid Id { get; set; }
        public Guid QuestionId { get; set; }
        public string QuestionTitle { get; set; } = string.Empty;
        public string QuestionType { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public List<string> MultipleAnswers { get; set; } = new List<string>();
        
        public QuestionResponse()
        {
            Id = Guid.NewGuid();
            MultipleAnswers = new List<string>();
        }
    }
}
