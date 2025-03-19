
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
        public string RespondentPhone { get; private set; }
        public string RespondentCompany { get; private set; }
        public DateTime SubmittedAt { get; private set; }
        public List<QuestionResponse> Answers { get; private set; } = new List<QuestionResponse>();
        public bool IsExistingClient { get; private set; }
        public Guid? ExistingClientId { get; private set; }

        // Parameterless constructor for EF Core
        private SurveyResponse() { }

        public SurveyResponse(Guid surveyId, string respondentName, string respondentEmail, string respondentPhone = null, string respondentCompany = null)
        {
            Id = Guid.NewGuid();
            SurveyId = surveyId;
            RespondentName = respondentName ?? string.Empty;
            RespondentEmail = respondentEmail ?? string.Empty;
            RespondentPhone = respondentPhone;
            RespondentCompany = respondentCompany;
            SubmittedAt = DateTime.UtcNow;
            IsExistingClient = false;
        }

        public void SetClientInfo(bool isExistingClient, Guid? existingClientId)
        {
            IsExistingClient = isExistingClient;
            ExistingClientId = existingClientId;
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
        public string QuestionTitle { get; private set; }
        public string QuestionType { get; private set; }
        public string Answer { get; private set; }
        public List<string> MultipleAnswers { get; private set; } = new List<string>();
        public bool IsValid { get; private set; }

        // Parameterless constructor for EF Core
        private QuestionResponse() { }

        public QuestionResponse(Guid questionId, string questionTitle, string questionType, string answer)
        {
            Id = Guid.NewGuid();
            QuestionId = questionId;
            QuestionTitle = questionTitle ?? string.Empty;
            QuestionType = questionType;
            Answer = answer ?? string.Empty;
            IsValid = true;
        }

        public QuestionResponse(Guid questionId, string questionTitle, string questionType, List<string> multipleAnswers)
        {
            Id = Guid.NewGuid();
            QuestionId = questionId;
            QuestionTitle = questionTitle ?? string.Empty;
            QuestionType = questionType;
            MultipleAnswers = multipleAnswers ?? new List<string>();
            IsValid = true;
        }

        public void SetValidationStatus(bool isValid)
        {
            IsValid = isValid;
        }
    }
}
