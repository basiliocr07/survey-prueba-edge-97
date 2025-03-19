
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SurveyApp.Domain.Entities;

namespace SurveyApp.Application.Ports
{
    public interface ISuggestionRepository
    {
        Task<Suggestion> GetByIdAsync(Guid id);
        Task<List<Suggestion>> GetAllAsync();
        Task<Suggestion> CreateAsync(Suggestion suggestion);
        Task UpdateAsync(Suggestion suggestion);
        Task DeleteAsync(Guid id);
    }
}
