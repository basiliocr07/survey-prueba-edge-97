
using System;
using System.Collections.Generic;

namespace SurveyApp.WebMvc.Models
{
    public class SurveyResponseAnalyticsViewModel
    {
        public Guid Id { get; set; }
        public Guid SurveyId { get; set; }
        public string SurveyTitle { get; set; }
        public string RespondentName { get; set; }
        public string RespondentEmail { get; set; }
        public string RespondentPhone { get; set; }
        public string RespondentCompany { get; set; }
        public DateTime SubmittedAt { get; set; }
        public bool IsValidated { get; set; }
        public double CompletionTime { get; set; }
        public List<QuestionAnswerViewModel> Answers { get; set; } = new List<QuestionAnswerViewModel>();
        
        // Propiedades adicionales para coincidir con la funcionalidad de React
        public string DeviceType { get; set; }
        public string Browser { get; set; }
        public string OperatingSystem { get; set; }
        public string Location { get; set; }
        public string IpAddress { get; set; }
        
        // Propiedades ampliadas para análisis detallado
        public int QuestionCount { get; set; }
        public int ValidAnswersCount { get; set; }
        public double ValidationRate => QuestionCount > 0 ? (ValidAnswersCount / (double)QuestionCount) * 100 : 0;
        public string FormattedCompletionTime => FormatTimeSpan(TimeSpan.FromSeconds(CompletionTime));
        public List<string> Tags { get; set; } = new List<string>();
        public string Source { get; set; }
        public string UserAgent { get; set; }
        public bool WasAbandoned { get; set; }
        public int PageViews { get; set; }
        public double AverageTimePerQuestion => Answers.Count > 0 ? CompletionTime / Answers.Count : 0;
        
        // Método para formatear el tiempo de manera legible
        private string FormatTimeSpan(TimeSpan timeSpan)
        {
            if (timeSpan.TotalMinutes < 1)
                return $"{timeSpan.Seconds}s";
            if (timeSpan.TotalHours < 1)
                return $"{timeSpan.Minutes}m {timeSpan.Seconds}s";
            
            return $"{(int)timeSpan.TotalHours}h {timeSpan.Minutes}m";
        }
    }

    public class QuestionAnswerViewModel
    {
        public Guid QuestionId { get; set; }
        public string QuestionTitle { get; set; }
        public string QuestionType { get; set; }
        public string Answer { get; set; }
        public List<string> MultipleAnswers { get; set; }
        public bool IsValid { get; set; }
        
        // Propiedades adicionales para análisis más detallados
        public double ScoreValue { get; set; }
        public int CompletionTimeSeconds { get; set; }
        public string Category { get; set; }
        
        // Propiedades ampliadas para visualizaciones
        public bool IsSkipped => string.IsNullOrEmpty(Answer) && (MultipleAnswers == null || MultipleAnswers.Count == 0);
        public string FormattedCompletionTime => FormatTimeSpan(TimeSpan.FromSeconds(CompletionTimeSeconds));
        public int CharacterCount => Answer?.Length ?? 0;
        public bool HasFreeformInput => QuestionType == "text" || QuestionType == "textarea";
        public bool HasSingleChoice => QuestionType == "single-choice" || QuestionType == "dropdown";
        public bool HasMultipleChoice => QuestionType == "multiple-choice";
        public bool IsRating => QuestionType == "rating" || QuestionType == "nps";
        
        // Método para formatear el tiempo de manera legible
        private string FormatTimeSpan(TimeSpan timeSpan)
        {
            if (timeSpan.TotalMinutes < 1)
                return $"{timeSpan.Seconds}s";
            if (timeSpan.TotalHours < 1)
                return $"{timeSpan.Minutes}m {timeSpan.Seconds}s";
            
            return $"{(int)timeSpan.TotalHours}h {timeSpan.Minutes}m";
        }
    }
}
