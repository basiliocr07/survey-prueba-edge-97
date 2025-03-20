
using System;
using System.Collections.Generic;
using System.Linq;

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
        
        // Propiedades para visualizaciones
        public Dictionary<string, int> QuestionTypeDistribution => Answers
            .GroupBy(a => a.QuestionType)
            .ToDictionary(g => g.Key, g => g.Count());
            
        public Dictionary<string, double> CompletionRateByQuestionType => Answers
            .GroupBy(a => a.QuestionType)
            .ToDictionary(g => g.Key, g => g.Count(a => !a.IsSkipped) / (double)g.Count() * 100);
            
        public Dictionary<string, double> AverageScoreByCategory => Answers
            .Where(a => a.IsRating && !string.IsNullOrEmpty(a.Category))
            .GroupBy(a => a.Category)
            .ToDictionary(g => g.Key, g => g.Average(a => a.ScoreValue));

        // Para gráficos de visualización
        public Dictionary<string, int> AnswerDistribution(Guid questionId)
        {
            var answer = Answers.FirstOrDefault(a => a.QuestionId == questionId);
            if (answer == null || (!answer.HasSingleChoice && !answer.HasMultipleChoice))
                return new Dictionary<string, int>();
                
            var result = new Dictionary<string, int>();
            
            if (answer.HasSingleChoice)
            {
                if (!string.IsNullOrEmpty(answer.Answer))
                    result[answer.Answer] = 1;
            }
            else if (answer.HasMultipleChoice && answer.MultipleAnswers != null)
            {
                foreach (var option in answer.MultipleAnswers)
                {
                    result[option] = 1;
                }
            }
            
            return result;
        }
        
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
        public bool HasFreeformInput => QuestionType.Equals("text", StringComparison.OrdinalIgnoreCase) || QuestionType.Equals("textarea", StringComparison.OrdinalIgnoreCase);
        public bool HasSingleChoice => QuestionType.Equals("single-choice", StringComparison.OrdinalIgnoreCase) || QuestionType.Equals("dropdown", StringComparison.OrdinalIgnoreCase);
        public bool HasMultipleChoice => QuestionType.Equals("multiple-choice", StringComparison.OrdinalIgnoreCase);
        public bool IsRating => QuestionType.Equals("rating", StringComparison.OrdinalIgnoreCase) || QuestionType.Equals("nps", StringComparison.OrdinalIgnoreCase);
        
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
