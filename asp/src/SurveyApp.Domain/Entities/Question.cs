
using System;
using System.Collections.Generic;

namespace SurveyApp.Domain.Entities
{
    public class Question
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public QuestionType Type { get; private set; }
        public bool IsRequired { get; private set; }
        public List<string> Options { get; private set; } = new List<string>();
        public QuestionSettings Settings { get; private set; }

        // For EF Core
        private Question() { }

        public Question(string title, QuestionType type, string description = null, bool isRequired = false)
        {
            Id = Guid.NewGuid();
            Title = title;
            Description = description;
            Type = type;
            IsRequired = isRequired;
            Settings = new QuestionSettings();
        }

        public void UpdateTitle(string title)
        {
            Title = title;
        }

        public void UpdateDescription(string description)
        {
            Description = description;
        }

        public void UpdateType(QuestionType type)
        {
            Type = type;
        }

        public void SetRequired(bool isRequired)
        {
            IsRequired = isRequired;
        }

        public void SetOptions(List<string> options)
        {
            if (Type == QuestionType.MultipleChoice || 
                Type == QuestionType.Dropdown || 
                Type == QuestionType.Checkbox)
            {
                Options.Clear();
                Options.AddRange(options);
            }
        }

        public void UpdateSettings(QuestionSettings settings)
        {
            Settings = settings;
        }
    }

    public enum QuestionType
    {
        ShortAnswer,
        LongAnswer,
        MultipleChoice,
        Checkbox,
        Dropdown,
        Rating,
        Date,
        Time,
        File,
        Email,
        Phone,
        Number
    }

    public class QuestionSettings
    {
        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }
        public bool AllowMultipleSelections { get; set; }
        public int? MaxFileSize { get; set; }
        public string[] AllowedFileTypes { get; set; }
        public string Placeholder { get; set; }
    }
}
