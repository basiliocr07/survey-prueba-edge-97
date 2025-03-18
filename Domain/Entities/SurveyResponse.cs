
using System;
using System.Collections.Generic;

namespace SurveyApp.Domain.Entities
{
    public class SurveyResponse
    {
        public Guid Id { get; private set; }
        public Guid SurveyId { get; private set; }
        public string RespondentName { get; private set; }
        public string RespondentEmail { get; private set; }
        public DateTime SubmittedAt { get; private set; }
        public List<QuestionResponse> Answers { get; private set; } = new List<QuestionResponse>();

        // Parameterless constructor for EF Core
        private SurveyResponse() { }

        public SurveyResponse(Guid surveyId, string respondentName, string respondentEmail)
        {
            Id = Guid.NewGuid();
            SurveyId = surveyId;
            RespondentName = respondentName ?? string.Empty;
            RespondentEmail = respondentEmail ?? string.Empty;
            SubmittedAt = DateTime.UtcNow;
        }

        public void AddAnswer(QuestionResponse answer)
        {
            if (answer == null)
                throw new ArgumentNullException(nameof(answer));

            Answers.Add(answer);
        }
    }

    public class QuestionResponse
    {
        public Guid Id { get; private set; }
        public Guid QuestionId { get; private set; }
        public string QuestionType { get; private set; }
        public string Answer { get; private set; }
        public List<string> MultipleAnswers { get; private set; } = new List<string>();

        // Parameterless constructor for EF Core
        private QuestionResponse() { }

        public QuestionResponse(Guid questionId, string questionType, string answer)
        {
            Id = Guid.NewGuid();
            QuestionId = questionId;
            QuestionType = questionType;
            Answer = answer ?? string.Empty;
        }

        public QuestionResponse(Guid questionId, string questionType, List<string> multipleAnswers)
        {
            Id = Guid.NewGuid();
            QuestionId = questionId;
            QuestionType = questionType;
            MultipleAnswers = multipleAnswers ?? new List<string>();
        }
    }
}
