
using System;

namespace SurveyApp.Domain.Entities
{
    public class Suggestion
    {
        public Guid Id { get; private set; }
        public string Content { get; private set; }
        public string CustomerName { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public string Status { get; private set; }

        // Parameterless constructor for EF Core
        private Suggestion() { }

        public Suggestion(string content, string customerName)
        {
            Id = Guid.NewGuid();
            Content = content ?? string.Empty;
            CustomerName = customerName ?? string.Empty;
            CreatedAt = DateTime.UtcNow;
            Status = "pending";
        }

        public void UpdateContent(string content)
        {
            Content = content ?? string.Empty;
        }

        public void UpdateCustomerName(string customerName)
        {
            CustomerName = customerName ?? string.Empty;
        }

        public void SetStatus(string status)
        {
            Status = status ?? "pending";
        }
    }
}
