
using System;
using SurveyApp.Domain.Entities;

namespace SurveyApp.Application.Services
{
    public static class SuggestionStatusHelper
    {
        public static SuggestionStatus ParseStatus(string statusString)
        {
            if (Enum.TryParse<SuggestionStatus>(statusString, true, out var status))
            {
                return status;
            }
            return SuggestionStatus.New; // Default value
        }
        
        // Add a useful helper method to convert from status enum to string
        public static string GetStatusString(SuggestionStatus status)
        {
            return status.ToString();
        }
    }
}
