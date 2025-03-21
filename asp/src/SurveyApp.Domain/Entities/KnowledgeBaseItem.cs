
using System;
using System.Collections.Generic;

namespace SurveyApp.Domain.Entities
{
    public class KnowledgeBaseItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Category { get; set; }
        public List<string> Tags { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string Author { get; set; }
        public int ViewCount { get; set; }
        public bool IsPublished { get; set; }

        public KnowledgeBaseItem()
        {
            Id = Guid.NewGuid();
            Tags = new List<string>();
            CreatedAt = DateTime.UtcNow;
            IsPublished = true;
        }
    }
}
