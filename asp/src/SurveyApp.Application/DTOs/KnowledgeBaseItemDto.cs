
using System;
using System.Collections.Generic;

namespace SurveyApp.Application.DTOs
{
    public class KnowledgeBaseItemDto
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
    }

    public class CreateKnowledgeBaseItemDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Category { get; set; }
        public List<string> Tags { get; set; }
        public string Author { get; set; }
    }

    public class UpdateKnowledgeBaseItemDto
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Category { get; set; }
        public List<string> Tags { get; set; }
        public bool IsPublished { get; set; }
    }
}
