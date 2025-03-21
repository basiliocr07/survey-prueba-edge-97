
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SurveyApp.Application.DTOs;
using SurveyApp.WebMvc.Models;

namespace SurveyApp.Application.Services
{
    public interface IRequirementService
    {
        Task<RequirementDto> GetRequirementByIdAsync(Guid id);
        Task<List<RequirementDto>> GetAllRequirementsAsync();
        Task<RequirementDto> CreateRequirementAsync(CreateRequirementDto requirementDto);
        Task UpdateRequirementAsync(Guid id, UpdateRequirementDto requirementDto);
        Task DeleteRequirementAsync(Guid id);
        Task UpdateRequirementStatusAsync(Guid id, RequirementStatusUpdateDto statusUpdateDto);
        Task<List<RequirementDto>> GetRecentRequirementsAsync(int count);
        Task<List<RequirementDto>> GetRequirementsByStatusAsync(string status);
        Task<List<RequirementDto>> GetRequirementsByPriorityAsync(string priority);
        Task<List<RequirementDto>> GetRequirementsByCategoryAsync(string category);
        Task<List<RequirementDto>> GetRequirementsByProjectAreaAsync(string projectArea);
        Task<List<RequirementDto>> SearchRequirementsAsync(string searchTerm);
        Task<RequirementReportsViewModel> GetRequirementReportsAsync();
        Task AddResponseToRequirementAsync(Guid id, string response);
    }
}
