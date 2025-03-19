
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SurveyApp.Domain.Entities;

namespace SurveyApp.Application.Ports
{
    public interface IRequirementRepository
    {
        Task<Requirement> GetByIdAsync(Guid id);
        Task<List<Requirement>> GetAllAsync();
        Task<Requirement> CreateAsync(Requirement requirement);
        Task UpdateAsync(Requirement requirement);
        Task DeleteAsync(Guid id);
        Task<List<Requirement>> GetRecentRequirementsAsync(int count);
        Task<List<Requirement>> GetRequirementsByStatusAsync(string status);
        Task<List<Requirement>> GetRequirementsByPriorityAsync(string priority);
    }
}
