
using System;
using System.Collections.Generic;

namespace SurveyApp.Domain.Entities
{
    public class SurveyResponse
    {
        public Guid Id { get; private set; }
        public Guid SurveyId { get; private set; }
        public string ResponseDate { get; private set; }
        public List<QuestionResponse> Answers { get; private set; } = new List<QuestionResponse>();
        public DateTime SubmittedAt { get; private set; }

        // For EF Core
        private SurveyResponse() { }

        public SurveyResponse(Guid surveyId, string responseDate)
        {
            Id = Guid.NewGuid();
            SurveyId = surveyId;
            ResponseDate = responseDate;
            SubmittedAt = DateTime.UtcNow;
        }

        public void AddAnswer(Guid questionId, string answer)
        {
            Answers.Add(new QuestionResponse { QuestionId = questionId, Answer = answer });
        }

        public void AddRangeOfAnswers(List<QuestionResponse> answers)
        {
            Answers.AddRange(answers);
        }
    }

    public class QuestionResponse
    {
        public Guid QuestionId { get; set; }
        public string Answer { get; set; }
    }
}
