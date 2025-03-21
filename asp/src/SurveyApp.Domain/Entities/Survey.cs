
using System;
using System.Collections.Generic;

namespace SurveyApp.Domain.Entities
{
    public class Survey
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int Responses { get; set; }
        public int CompletionRate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Category { get; set; } = "General"; // Default value for Category
        public List<Question> Questions { get; set; } = new List<Question>();
        public DeliveryConfig? DeliveryConfig { get; set; }
        
        // Additional properties
        public DateTime? ExpiryDate { get; set; }
        public bool AllowAnonymousResponses { get; set; }
        public bool LimitOneResponsePerUser { get; set; }
        public string ThankYouMessage { get; set; } = string.Empty;

        public Survey()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            Status = "Active";
            Responses = 0;
            CompletionRate = 0;
            Questions = new List<Question>();
            AllowAnonymousResponses = true;
            LimitOneResponsePerUser = false;
            ThankYouMessage = "¡Gracias por completar nuestra encuesta!";
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

        public void UpdateCategory(string category)
        {
            Category = !string.IsNullOrWhiteSpace(category) ? category : "General";
        }
    }
}
