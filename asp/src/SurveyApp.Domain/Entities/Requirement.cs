
using System;

namespace SurveyApp.Domain.Entities
{
    public class Requirement
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string ProjectArea { get; set; }
        public string Category { get; set; }
        public bool IsAnonymous { get; set; }
        public int? CompletionPercentage { get; set; }

        // For EF Core
        private Requirement() { }

        public Requirement(string title, string description, string priority, string customerName, string customerEmail, string projectArea = "General", string category = null, bool isAnonymous = false)
        {
            Id = Guid.NewGuid();
            Title = title;
            Description = description;
            Priority = priority;
            CreatedAt = DateTime.UtcNow;
            Status = "proposed";
            CustomerName = customerName;
            CustomerEmail = customerEmail;
            ProjectArea = projectArea;
            Category = category;
            IsAnonymous = isAnonymous;
            CompletionPercentage = 0;
        }

        public void UpdateStatus(string status)
        {
            Status = status;
            UpdatedAt = DateTime.UtcNow;
            
            if (status == "implemented")
            {
                CompletionPercentage = 100;
            }
            else if (status == "in-progress" && (!CompletionPercentage.HasValue || CompletionPercentage == 0))
            {
                CompletionPercentage = 10;
            }
        }

        public void UpdateCompletionPercentage(int percentage)
        {
            if (percentage < 0) percentage = 0;
            if (percentage > 100) percentage = 100;
            
            CompletionPercentage = percentage;
            UpdatedAt = DateTime.UtcNow;
            
            if (percentage == 100 && Status != "implemented")
            {
                Status = "implemented";
            }
            else if (percentage > 0 && percentage < 100 && Status == "proposed")
            {
                Status = "in-progress";
            }
        }

        public void UpdatePriority(string priority)
        {
            Priority = priority;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
