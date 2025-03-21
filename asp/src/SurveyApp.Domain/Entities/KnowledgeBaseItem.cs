
using System;
using System.Collections.Generic;

namespace SurveyApp.Domain.Entities
{
    public class KnowledgeBaseItem
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; }
        public string Content { get; private set; }
        public string Category { get; private set; }
        public List<string> Tags { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        // Constructor for creating a new knowledge base item
        public KnowledgeBaseItem(string title, string content, string category, List<string> tags = null)
        {
            Id = Guid.NewGuid();
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Content = content ?? throw new ArgumentNullException(nameof(content));
            Category = category;
            Tags = tags ?? new List<string>();
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        // Protected constructor for Entity Framework
        protected KnowledgeBaseItem() { }

        // Update methods
        public void UpdateTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty", nameof(title));
            
            Title = title;
            UpdateLastModified();
        }

        public void UpdateContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Content cannot be empty", nameof(content));
            
            Content = content;
            UpdateLastModified();
        }

        public void UpdateCategory(string category)
        {
            Category = category;
            UpdateLastModified();
        }

        public void UpdateTags(List<string> tags)
        {
            Tags = tags ?? new List<string>();
            UpdateLastModified();
        }

        private void UpdateLastModified()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
