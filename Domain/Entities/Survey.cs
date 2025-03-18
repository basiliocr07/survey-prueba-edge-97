
using System;
using System.Collections.Generic;
using System.Linq;

namespace SurveyApp.Domain.Entities
{
    public class Survey
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public List<Question> Questions { get; private set; } = new List<Question>();
        public DateTime CreatedAt { get; private set; }
        public int Responses { get; private set; }
        public int CompletionRate { get; private set; }
        public DeliveryConfig DeliveryConfig { get; private set; }
        public string Status { get; private set; } = "Active";
        public string Category { get; private set; }
        public bool IsFeatured { get; private set; }
        public DateTime? LastUpdated { get; private set; }
        public DateTime? LastResponseDate { get; private set; }
        public string CreatedBy { get; private set; }

        // Parameterless constructor for EF Core
        private Survey() { }

        public Survey(string title, string description)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Survey title cannot be empty", nameof(title));
                
            Id = Guid.NewGuid();
            Title = title;
            Description = description ?? string.Empty;
            CreatedAt = DateTime.UtcNow;
            Responses = 0;
            CompletionRate = 0;
            DeliveryConfig = new DeliveryConfig();
            Status = "Active";
        }

        public void UpdateTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Survey title cannot be empty", nameof(title));
                
            Title = title;
            LastUpdated = DateTime.UtcNow;
        }

        public void UpdateDescription(string description)
        {
            Description = description ?? string.Empty;
            LastUpdated = DateTime.UtcNow;
        }

        public void SetCategory(string category)
        {
            Category = category;
            LastUpdated = DateTime.UtcNow;
        }

        public void SetFeatured(bool isFeatured)
        {
            IsFeatured = isFeatured;
            LastUpdated = DateTime.UtcNow;
        }

        public void SetStatus(string status)
        {
            Status = status;
            LastUpdated = DateTime.UtcNow;
        }

        public void SetCreatedBy(string createdBy)
        {
            CreatedBy = createdBy;
        }

        public void AddQuestion(Question question)
        {
            if (question == null)
                throw new ArgumentNullException(nameof(question));
                
            Questions.Add(question);
            LastUpdated = DateTime.UtcNow;
        }

        public void RemoveQuestion(Question question)
        {
            if (question == null)
                throw new ArgumentNullException(nameof(question));
                
            Questions.Remove(question);
            LastUpdated = DateTime.UtcNow;
        }

        public void RemoveQuestionById(Guid questionId)
        {
            var question = Questions.FirstOrDefault(q => q.Id == questionId);
            if (question != null)
            {
                Questions.Remove(question);
                LastUpdated = DateTime.UtcNow;
            }
        }

        public void ClearQuestions()
        {
            Questions.Clear();
            LastUpdated = DateTime.UtcNow;
        }

        public void SetDeliveryConfig(DeliveryConfig deliveryConfig)
        {
            DeliveryConfig = deliveryConfig ?? new DeliveryConfig();
            LastUpdated = DateTime.UtcNow;
        }

        public void IncrementResponses()
        {
            Responses++;
            LastResponseDate = DateTime.UtcNow;
            RecalculateCompletionRate();
        }

        public void SetCompletionRate(int completionPercentage)
        {
            if (completionPercentage < 0 || completionPercentage > 100)
                throw new ArgumentOutOfRangeException(nameof(completionPercentage), "Completion rate must be between 0 and 100");
                
            CompletionRate = completionPercentage;
        }

        private void RecalculateCompletionRate()
        {
            // This is a placeholder for actual completion rate calculation logic
            // In a real application, this would calculate based on completed vs. partial responses
            // For now, we'll assume a fixed rate based on the number of responses
            if (Responses > 0)
            {
                // This is just an example calculation
                CompletionRate = Math.Min(100, (int)(80 + (Responses * 2.0 / 10.0)));
            }
            else
            {
                CompletionRate = 0;
            }
        }
    }
}
