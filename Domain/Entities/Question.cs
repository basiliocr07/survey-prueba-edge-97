
using System;
using System.Collections.Generic;
using System.Linq;

namespace SurveyApp.Domain.Entities
{
    public class Question
    {
        public Guid Id { get; private set; }
        public QuestionType Type { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public bool Required { get; private set; }
        public List<string> Options { get; private set; }

        // Parameterless constructor for EF Core
        private Question() { }

        public Question(string title, QuestionType type, bool required)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Question title cannot be empty", nameof(title));
                
            Id = Guid.NewGuid();
            Title = title;
            Type = type;
            Required = required;
            Description = string.Empty;
            Options = new List<string>();
        }

        public void UpdateTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Question title cannot be empty", nameof(title));
                
            Title = title;
        }

        public void UpdateDescription(string description)
        {
            Description = description ?? string.Empty;
        }

        public void SetOptions(List<string> options)
        {
            // Validate options for choice-based questions
            if ((Type == QuestionType.SingleChoice || Type == QuestionType.MultipleChoice) 
                && (options == null || options.Count < 2))
            {
                throw new ArgumentException($"Choice questions must have at least 2 options");
            }
            
            // Remove any null or empty options
            Options = options?.Where(o => !string.IsNullOrWhiteSpace(o)).ToList() ?? new List<string>();
        }

        public void AddOption(string option)
        {
            if (string.IsNullOrWhiteSpace(option))
                throw new ArgumentException("Option text cannot be empty", nameof(option));
                
            if (Options.Contains(option))
                return; // Don't add duplicates
                
            Options.Add(option);
        }

        public void RemoveOption(string option)
        {
            if (string.IsNullOrWhiteSpace(option))
                return;
                
            Options.Remove(option);
            
            // Ensure minimum number of options for choice questions
            if ((Type == QuestionType.SingleChoice || Type == QuestionType.MultipleChoice) && Options.Count < 2)
            {
                throw new InvalidOperationException(
                    "Cannot remove option: Choice questions must have at least 2 options");
            }
        }

        public void SetRequired(bool required)
        {
            Required = required;
        }
        
        public void ChangeType(QuestionType newType)
        {
            // Validate options count for choice-based questions
            if ((newType == QuestionType.SingleChoice || newType == QuestionType.MultipleChoice) 
                && Options.Count < 2)
            {
                throw new InvalidOperationException(
                    $"Cannot change to {newType}: Need at least 2 options");
            }
            
            Type = newType;
        }
    }

    public enum QuestionType
    {
        SingleChoice,
        MultipleChoice,
        Text,
        Rating
    }
}
