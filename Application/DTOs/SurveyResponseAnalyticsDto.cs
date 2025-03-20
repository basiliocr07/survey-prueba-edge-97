
using System;
using System.Collections.Generic;

namespace SurveyApp.Application.DTOs
{
    public class SurveyResponseAnalyticsDto
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
        public List<QuestionAnswerDto> Answers { get; set; } = new List<QuestionAnswerDto>();
        
        // Propiedades adicionales para coincidir con la funcionalidad de React
        public string DeviceType { get; set; }
        public string Browser { get; set; }
        public string OperatingSystem { get; set; }
        public string Location { get; set; }
        public string IpAddress { get; set; }
        
        // Propiedades ampliadas para análisis detallado
        public int QuestionCount { get; set; }
        public int ValidAnswersCount { get; set; }
        public double ValidationRate { get; set; }
        public string FormattedCompletionTime { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public string Source { get; set; }
        public string UserAgent { get; set; }
        public bool WasAbandoned { get; set; }
        public int PageViews { get; set; }
        public double AverageTimePerQuestion { get; set; }
        
        // Propiedades para visualizaciones
        public Dictionary<string, int> QuestionTypeDistribution { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, double> CompletionRateByQuestionType { get; set; } = new Dictionary<string, double>();
        public Dictionary<string, double> AverageScoreByCategory { get; set; } = new Dictionary<string, double>();
    }

    public class QuestionAnswerDto
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
        public bool IsSkipped { get; set; }
        public string FormattedCompletionTime { get; set; }
        public int CharacterCount { get; set; }
    }
}
