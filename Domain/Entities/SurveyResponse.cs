
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
        public Dictionary<string, string> Answers { get; set; } = new Dictionary<string, string>();
        public bool IsExistingClient { get; set; }
        public Guid? ExistingClientId { get; set; }

        public SurveyResponse()
        {
            Id = Guid.NewGuid();
            SubmittedAt = DateTime.UtcNow;
            ResponseDate = DateTime.UtcNow;
        }
    }
}
