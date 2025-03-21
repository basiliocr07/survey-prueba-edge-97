
using System;
using System.Collections.Generic;

namespace SurveyApp.Domain.Entities
{
    public class QuestionResponse
    {
        public Guid QuestionId { get; set; }
        public string QuestionTitle { get; set; }
        public string QuestionType { get; set; }
        public string Answer { get; set; }
        public List<string> MultipleAnswers { get; set; }
        public bool IsValid { get; set; } = true;
        
        // Propiedades adicionales para análisis detallado
        public double ScoreValue { get; set; }
        public int CompletionTimeSeconds { get; set; }
        public string Category { get; set; }
        
        // Constructor básico
        public QuestionResponse() 
        {
            MultipleAnswers = new List<string>();
        }
        
        // Constructor con parámetros
        public QuestionResponse(Guid questionId, string questionTitle, string questionType, string answer)
        {
            QuestionId = questionId;
            QuestionTitle = questionTitle;
            QuestionType = questionType;
            Answer = answer;
            MultipleAnswers = new List<string>();
            IsValid = true;
        }
        
        // Constructor para respuestas de opción múltiple
        public QuestionResponse(Guid questionId, string questionTitle, string questionType, List<string> multipleAnswers)
        {
            QuestionId = questionId;
            QuestionTitle = questionTitle;
            QuestionType = questionType;
            MultipleAnswers = multipleAnswers ?? new List<string>();
            IsValid = true;
        }
        
        // Método para establecer metadatos de análisis
        public void SetAnalyticsMetadata(string category, double scoreValue = 0, int completionTimeSeconds = 0)
        {
            Category = category;
            ScoreValue = scoreValue;
            CompletionTimeSeconds = completionTimeSeconds;
        }
        
        // Método para validar la respuesta
        public void Validate(bool isValid)
        {
            IsValid = isValid;
        }
        
        // Método faltante que se utiliza en SurveyResponseRepository
        public void SetValidationStatus(bool isValid)
        {
            IsValid = isValid;
        }
    }
}
