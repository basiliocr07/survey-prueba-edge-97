
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SurveyApp.Domain.Entities;

namespace SurveyApp.Application.Ports
{
    public interface ISurveyResponseRepository
    {
        Task<IEnumerable<SurveyResponse>> GetAllAsync();
        Task<IEnumerable<SurveyResponse>> GetBySurveyIdAsync(Guid surveyId);
        Task<SurveyResponse?> GetByIdAsync(Guid id);
        Task<SurveyResponse> CreateAsync(SurveyResponse response);
    }
}
