
using SurveyApp.Domain.Entities;

namespace SurveyApp.Application.DTOs
{
    public class UpdateSuggestionStatusDto
    {
        public SuggestionStatus Status { get; set; }
        public string Response { get; set; }
    }
}
