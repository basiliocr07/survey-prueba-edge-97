
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SurveyApp.Application.DTOs;

namespace SurveyApp.Application.Services
{
    public interface ISurveyResponseService
    {
        Task<IEnumerable<SurveyResponseDto>> GetAllResponsesAsync();
        Task<IEnumerable<SurveyResponseDto>> GetResponsesBySurveyIdAsync(Guid surveyId);
        Task<SurveyResponseDto?> GetResponseByIdAsync(Guid id);
        Task<SurveyResponseDto> CreateResponseAsync(CreateSurveyResponseDto responseDto);
        Task<SurveyResponseAnalyticsDto> GetResponseAnalyticsAsync(Guid surveyId);
    }

    public class SurveyResponseAnalyticsDto
    {
        public Guid SurveyId { get; set; }
        public int TotalResponses { get; set; }
        public double AverageCompletionTime { get; set; }
        public double CompletionRate { get; set; }
        public Dictionary<string, int> ResponsesByDate { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> DeviceBreakdown { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> BrowserBreakdown { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> LocationBreakdown { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, object> QuestionBreakdown { get; set; } = new Dictionary<string, object>();
    }

    public class CreateSurveyResponseDto
    {
        public Guid SurveyId { get; set; }
        public string RespondentName { get; set; } = string.Empty;
        public string RespondentEmail { get; set; } = string.Empty;
        public string RespondentPhone { get; set; } = string.Empty;
        public string RespondentCompany { get; set; } = string.Empty;
        public Dictionary<string, object> Answers { get; set; } = new Dictionary<string, object>();
        public bool IsExistingClient { get; set; }
        public Guid? ExistingClientId { get; set; }
        public string DeviceType { get; set; } = string.Empty;
        public string Browser { get; set; } = string.Empty;
        public string OperatingSystem { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
    }
}
