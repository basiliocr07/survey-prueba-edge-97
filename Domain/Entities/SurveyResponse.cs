
using System;
using System.Collections.Generic;

namespace SurveyApp.Domain.Entities
{
    public class SurveyResponse
    {
        public Guid Id { get; set; }
        public Guid SurveyId { get; set; }
        public string RespondentName { get; set; }
        public string RespondentEmail { get; set; }
        public string RespondentPhone { get; set; }
        public string RespondentCompany { get; set; }
        public DateTime SubmittedAt { get; set; }
        public DateTime ResponseDate { get; set; }
        public List<QuestionResponse> Answers { get; set; } = new List<QuestionResponse>();
        public bool IsExistingClient { get; set; }
        public Guid? ExistingClientId { get; set; }

        public SurveyResponse()
        {
            Id = Guid.NewGuid();
            SubmittedAt = DateTime.UtcNow;
            ResponseDate = DateTime.UtcNow;
            Answers = new List<QuestionResponse>();
        }

        // Constructor con parámetros básicos
        public SurveyResponse(Guid surveyId, string respondentName, string respondentEmail, string respondentPhone, string respondentCompany)
        {
            Id = Guid.NewGuid();
            SurveyId = surveyId;
            RespondentName = respondentName;
            RespondentEmail = respondentEmail;
            RespondentPhone = respondentPhone;
            RespondentCompany = respondentCompany;
            SubmittedAt = DateTime.UtcNow;
            ResponseDate = DateTime.UtcNow;
            Answers = new List<QuestionResponse>();
        }

        // Método para agregar una respuesta
        public void AddAnswer(QuestionResponse answer)
        {
            Answers.Add(answer);
        }

        // Método para establecer información del cliente
        public void SetClientInfo(bool isExistingClient, Guid? existingClientId)
        {
            IsExistingClient = isExistingClient;
            ExistingClientId = existingClientId;
        }
    }
}
