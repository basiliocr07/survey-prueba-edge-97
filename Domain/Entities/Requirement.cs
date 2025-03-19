
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
        public DateTime? UpdatedAt { get; set; }
        public string Status { get; set; }
        public string ProjectArea { get; set; }

        public Requirement()
        {
            Id = Guid.NewGuid();
            CreatedAt = DateTime.UtcNow;
            Status = "pending";
            ProjectArea = "general";
        }

        public void UpdateTitle(string title)
        {
            Title = title ?? string.Empty;
        }

        public void UpdateDescription(string description)
        {
            Description = description ?? string.Empty;
        }

        public void UpdatePriority(string priority)
        {
            Priority = priority ?? "medium";
        }

        public void SetStatus(string status)
        {
            Status = status ?? "pending";
        }

        public void SetProjectArea(string area)
        {
            ProjectArea = area ?? "general";
        }

        public void UpdateLastModified()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
