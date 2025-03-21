
using System;
using System.Collections.Generic;

namespace SurveyApp.Application.DTOs
{
    public class CreateSurveyResponseDto
    {
        public Guid SurveyId { get; set; }
        public string RespondentName { get; set; }
        public string RespondentEmail { get; set; }
        public string RespondentPhone { get; set; }
        public string RespondentCompany { get; set; }
        public Dictionary<string, object> Answers { get; set; }
        public bool IsExistingClient { get; set; }
        public Guid? ExistingClientId { get; set; }
        
        // Propiedades adicionales para análisis
        public double CompletionTime { get; set; }
        public string DeviceType { get; set; }
        public string Browser { get; set; }
        public string Location { get; set; }
    }
}
