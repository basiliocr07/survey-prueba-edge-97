
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

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
            {
                Debug.WriteLine($"[ERROR] Question constructor: title cannot be empty");
                throw new ArgumentException("Question title cannot be empty", nameof(title));
            }
                
            Id = Guid.NewGuid();
            Title = title;
            Type = type;
            Required = required;
            Description = string.Empty;
            Options = new List<string>();
            
            Debug.WriteLine($"[INFO] Question created: '{title}', Type: {type}, Required: {required}, ID: {Id}");
        }

        public void UpdateTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                Debug.WriteLine($"[ERROR] Question.UpdateTitle: title cannot be empty, QuestionId: {Id}");
                throw new ArgumentException("Question title cannot be empty", nameof(title));
            }
                
            Title = title;
            Debug.WriteLine($"[INFO] Question {Id} title updated to: '{title}'");
        }

        public void UpdateDescription(string description)
        {
            Description = description ?? string.Empty;
            Debug.WriteLine($"[INFO] Question {Id} description updated, Length: {Description.Length}");
        }

        public void SetOptions(List<string> options)
        {
            // Validate options for choice-based questions
            if ((Type == QuestionType.SingleChoice || Type == QuestionType.MultipleChoice) 
                && (options == null || options.Count < 2))
            {
                Debug.WriteLine($"[ERROR] Question.SetOptions: Choice questions must have at least 2 options, QuestionId: {Id}, Type: {Type}");
                throw new ArgumentException($"Choice questions must have at least 2 options");
            }
            
            // Remove any null or empty options
            var validOptions = options?.Where(o => !string.IsNullOrWhiteSpace(o)).ToList() ?? new List<string>();
            
            if (validOptions.Count != options?.Count)
            {
                Debug.WriteLine($"[WARNING] Question.SetOptions: Some options were null or empty and were removed, QuestionId: {Id}, Original count: {options?.Count}, Valid count: {validOptions.Count}");
            }
            
            Options = validOptions;
            Debug.WriteLine($"[INFO] Question {Id} options set, Count: {Options.Count}");
        }

        public void AddOption(string option)
        {
            if (string.IsNullOrWhiteSpace(option))
            {
                Debug.WriteLine($"[ERROR] Question.AddOption: Option text cannot be empty, QuestionId: {Id}");
                throw new ArgumentException("Option text cannot be empty", nameof(option));
            }
                
            if (Options.Contains(option))
            {
                Debug.WriteLine($"[WARNING] Question.AddOption: Duplicate option not added: '{option}', QuestionId: {Id}");
                return; // Don't add duplicates
            }
                
            Options.Add(option);
            Debug.WriteLine($"[INFO] Question {Id} added option: '{option}', Total options: {Options.Count}");
        }

        public void RemoveOption(string option)
        {
            if (string.IsNullOrWhiteSpace(option))
            {
                Debug.WriteLine($"[WARNING] Question.RemoveOption: Attempt to remove empty option, QuestionId: {Id}");
                return;
            }
            
            var initialCount = Options.Count;
            Options.Remove(option);
            
            if (initialCount == Options.Count)
            {
                Debug.WriteLine($"[WARNING] Question.RemoveOption: Option not found: '{option}', QuestionId: {Id}");
            }
            else
            {
                Debug.WriteLine($"[INFO] Question {Id} removed option: '{option}', Remaining options: {Options.Count}");
            }
            
            // Ensure minimum number of options for choice questions
            if ((Type == QuestionType.SingleChoice || Type == QuestionType.MultipleChoice) && Options.Count < 2)
            {
                Debug.WriteLine($"[ERROR] Question.RemoveOption: Cannot remove option - choice questions must have at least 2 options, QuestionId: {Id}, Type: {Type}, Remaining options: {Options.Count}");
                throw new InvalidOperationException(
                    "Cannot remove option: Choice questions must have at least 2 options");
            }
        }

        public void SetRequired(bool required)
        {
            Required = required;
            Debug.WriteLine($"[INFO] Question {Id} required status set to: {required}");
        }
        
        public void ChangeType(QuestionType newType)
        {
            // Validate options count for choice-based questions
            if ((newType == QuestionType.SingleChoice || newType == QuestionType.MultipleChoice) 
                && Options.Count < 2)
            {
                Debug.WriteLine($"[ERROR] Question.ChangeType: Cannot change to {newType} - need at least 2 options, QuestionId: {Id}, Current options: {Options.Count}");
                throw new InvalidOperationException(
                    $"Cannot change to {newType}: Need at least 2 options");
            }
            
            var oldType = Type;
            Type = newType;
            Debug.WriteLine($"[INFO] Question {Id} type changed from {oldType} to {newType}");
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
