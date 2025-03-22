
using SurveyApp.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SurveyApp.Domain.Repositories
{
    public interface ISurveyResponseRepository
    {
        Task<IEnumerable<SurveyResponse>> GetAllAsync();
        Task<SurveyResponse?> GetByIdAsync(int id);
        Task<IEnumerable<SurveyResponse>> GetBySurveyIdAsync(int surveyId);
        Task<bool> AddAsync(SurveyResponse surveyResponse);
        Task<bool> UpdateAsync(SurveyResponse surveyResponse);
        Task<bool> DeleteAsync(int id);
    }
}
