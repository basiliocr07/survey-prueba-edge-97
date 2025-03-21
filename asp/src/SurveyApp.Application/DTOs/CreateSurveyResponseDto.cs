
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
        public string OperatingSystem { get; set; }
        public string Location { get; set; }
        public string IpAddress { get; set; }
        public string Source { get; set; }
        public string UserAgent { get; set; }
        public bool WasAbandoned { get; set; }
        public int PageViews { get; set; }
        public string ReferrerUrl { get; set; }
        public string ScreenResolution { get; set; }
        public string Language { get; set; }
        public string TimeZone { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime CompletionTime { get; set; }
        public bool IsMobile { get; set; }
        public List<string> QuestionVisitLog { get; set; } = new List<string>();
        public Dictionary<string, double> QuestionResponseTimes { get; set; } = new Dictionary<string, double>();
    }
}
