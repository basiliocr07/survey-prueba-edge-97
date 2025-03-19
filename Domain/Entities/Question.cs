
using System;
using System.Collections.Generic;

namespace SurveyApp.Domain.Entities
{
    public class Question
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public bool Required { get; set; }
        public List<string> Options { get; set; }

        public Question()
        {
            Id = Guid.NewGuid();
            Options = new List<string>();
        }

        // Added the SetOptions method
        public void SetOptions(List<string> options)
        {
            Options = options ?? new List<string>();
        }
    }
}
