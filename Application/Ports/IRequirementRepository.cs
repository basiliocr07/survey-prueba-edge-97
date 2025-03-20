
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
        Task<List<Requirement>> GetRequirementsByCategoryAsync(string category);
        Task<List<Requirement>> SearchRequirementsAsync(string searchTerm);
        Task<List<Requirement>> GetRequirementsByProjectAreaAsync(string projectArea);
        Task<Dictionary<string, int>> GetStatusDistributionAsync();
        Task<Dictionary<string, int>> GetPriorityDistributionAsync();
        Task<Dictionary<string, int>> GetCategoryDistributionAsync();
        Task<Dictionary<string, int>> GetProjectAreaDistributionAsync();
        Task<Dictionary<string, int>> GetMonthlyRequirementsCountAsync(int months);
    }
}
