
using SurveyApp.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SurveyApp.Domain.Repositories
{
    public interface ISurveyResponseRepository
    {
        Task<IEnumerable<SurveyResponse>> GetBySurveyIdAsync(int surveyId);
        Task<SurveyResponse?> GetByIdAsync(int id);
        Task<bool> AddAsync(SurveyResponse response);
        Task<bool> DeleteAsync(int id);
    }
}
