
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SurveyApp.Domain.Entities;

namespace SurveyApp.Application.Ports
{
    public interface ISurveyRepository
    {
        Task<IEnumerable<Survey>> GetAllAsync();
        Task<Survey?> GetByIdAsync(Guid id);
        Task<Survey> CreateAsync(Survey survey);
        Task UpdateAsync(Survey survey);
        Task DeleteAsync(Guid id);
    }
}
