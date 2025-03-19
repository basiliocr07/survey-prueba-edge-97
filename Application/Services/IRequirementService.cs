
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SurveyApp.Application.DTOs;

namespace SurveyApp.Application.Services
{
    public interface IRequirementService
    {
        Task<RequirementDto> GetRequirementByIdAsync(Guid id);
        Task<List<RequirementDto>> GetAllRequirementsAsync();
        Task<RequirementDto> CreateRequirementAsync(CreateRequirementDto requirementDto);
        Task UpdateRequirementAsync(Guid id, UpdateRequirementDto requirementDto);
        Task DeleteRequirementAsync(Guid id);
        Task UpdateRequirementStatusAsync(Guid id, string status);
        Task<List<RequirementDto>> GetRecentRequirementsAsync(int count);
        Task<List<RequirementDto>> GetRequirementsByStatusAsync(string status);
        Task<List<RequirementDto>> GetRequirementsByPriorityAsync(string priority);
    }
}
