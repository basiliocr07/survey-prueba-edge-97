
using SurveyApp.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SurveyApp.Application.Interfaces
{
    public interface ISurveyResponseService
    {
        Task<IEnumerable<SurveyResponse>> GetAllResponsesAsync();
        Task<SurveyResponse?> GetResponseByIdAsync(int id);
        Task<IEnumerable<SurveyResponse>> GetResponsesBySurveyIdAsync(int surveyId);
        Task<bool> SubmitResponseAsync(SurveyResponse surveyResponse);
        Task<bool> UpdateResponseAsync(SurveyResponse surveyResponse);
        Task<bool> DeleteResponseAsync(int id);
    }
}
