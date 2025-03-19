
using System;

namespace SurveyApp.Domain.Entities
{
    public class Requirement
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string Priority { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; } // Added UpdatedAt property
        public string Status { get; private set; }
        public string ProjectArea { get; private set; } // Added ProjectArea property

        // Parameterless constructor for EF Core
        private Requirement() { }

        public Requirement(string title, string description, string priority)
        {
            Id = Guid.NewGuid();
            Title = title ?? string.Empty;
            Description = description ?? string.Empty;
            Priority = priority ?? "medium";
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
