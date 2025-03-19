
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

        // Constructor por defecto para EF Core
        public QuestionResponse()
        {
            MultipleAnswers = new List<string>();
        }

        // Constructor para crear una respuesta de pregunta
        public QuestionResponse(Guid questionId, string questionTitle, string questionType, string answer)
        {
            QuestionId = questionId;
            QuestionTitle = questionTitle;
            QuestionType = questionType;
            Answer = answer;
            MultipleAnswers = new List<string>();
            IsValid = true;
        }

        // Constructor para respuestas de tipo múltiple
        public QuestionResponse(Guid questionId, string questionTitle, string questionType, List<string> multipleAnswers)
        {
            QuestionId = questionId;
            QuestionTitle = questionTitle;
            QuestionType = questionType;
            Answer = string.Empty;
            MultipleAnswers = multipleAnswers ?? new List<string>();
            IsValid = true;
        }

        // Método para establecer el estado de validación
        public void SetValidationStatus(bool isValid)
        {
            IsValid = isValid;
        }
    }
}
