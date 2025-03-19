
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

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
        
        // Para almacenar configuraciones específicas por tipo de pregunta
        public QuestionSettings Settings { get; set; }

        public Question()
        {
            Id = Guid.NewGuid();
            Options = new List<string>();
            Settings = new QuestionSettings();
        }

        // Added the SetOptions method
        public void SetOptions(List<string> options)
        {
            Options = options ?? new List<string>();
        }
        
        public void SetSettings(QuestionSettings settings)
        {
            Settings = settings ?? new QuestionSettings();
        }
    }
    
    public class QuestionSettings
    {
        // Para preguntas tipo Rating
        public int? MinValue { get; set; }
        public int? MaxValue { get; set; }
        
        // Para preguntas tipo NPS
        public string LowLabel { get; set; }
        public string MiddleLabel { get; set; }
        public string HighLabel { get; set; }
        
        public QuestionSettings()
        {
            // Valores por defecto
            MinValue = 1;
            MaxValue = 5;
            LowLabel = "No es probable";
            MiddleLabel = "Neutral";
            HighLabel = "Muy probable";
        }
    }
}
