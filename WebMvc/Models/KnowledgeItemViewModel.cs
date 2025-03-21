
using System;
using System.Collections.Generic;

namespace SurveyApp.WebMvc.Models
{
    public class KnowledgeItemViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Category { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
    }
}
