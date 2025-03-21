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
        public DateTime? UpdatedAt { get; private set; }
        public string Status { get; private set; }
        public string ProjectArea { get; private set; }
        public string CustomerName { get; private set; }
        public string CustomerEmail { get; private set; }
        public bool IsAnonymous { get; private set; }
        public string Response { get; private set; }
        public DateTime? ResponseDate { get; private set; }
        public int? CompletionPercentage { get; private set; }
        public string Category { get; private set; }
        public string AcceptanceCriteria { get; private set; }
        public DateTime? TargetDate { get; private set; }
        public string Content { get; set; }
        public string TargetCompletionDate => TargetDate?.ToString("yyyy-MM-dd");

        // Constructor para crear un nuevo requerimiento
        public Requirement(string title, string description, string priority)
        {
            Id = Guid.NewGuid();
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Priority = priority ?? "medium";
            Status = "proposed";
            ProjectArea = "general";
            CreatedAt = DateTime.UtcNow;
            CompletionPercentage = 0;
            IsAnonymous = false;
        }

        // Constructor protegido para Entity Framework
        protected Requirement() { }

        // Métodos para actualizar propiedades
        public void UpdateTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty", nameof(title));
            
            Title = title;
            UpdateLastModified();
        }

        public void UpdateDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be empty", nameof(description));
            
            Description = description;
            UpdateLastModified();
        }

        public void UpdatePriority(string priority)
        {
            Priority = priority ?? "medium";
            UpdateLastModified();
        }
    }
}
