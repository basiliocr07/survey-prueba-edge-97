
using System;
using System.Collections.Generic;

namespace SurveyApp.Domain.Entities
{
    public class Survey
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Responses { get; set; }
        public int CompletionRate { get; set; }
        public string Status { get; set; }
        public string Category { get; set; } // Added Category property
        public List<Question> Questions { get; set; }
        public DeliveryConfig DeliveryConfig { get; set; }

        public Survey()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            Status = "Active";
            Responses = 0;
            CompletionRate = 0;
            Questions = new List<Question>();
        }

        public void AddQuestion(Question question)
        {
            Questions.Add(question);
        }

        public void IncrementResponses()
        {
            Responses++;
        }

        public void SetDeliveryConfig(DeliveryConfig config)
        {
            DeliveryConfig = config;
        }
    }
}
