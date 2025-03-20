using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SurveyApp.Application.DTOs;
using SurveyApp.Application.Ports;
using SurveyApp.Domain.Entities;
using SurveyApp.WebMvc.Models;

namespace SurveyApp.Application.Services
{
    public class RequirementService : IRequirementService
    {
        private readonly IRequirementRepository _requirementRepository;
        private readonly ILogger<RequirementService> _logger;

        public RequirementService(IRequirementRepository requirementRepository, ILogger<RequirementService> logger)
        {
            _requirementRepository = requirementRepository;
            _logger = logger;
        }

        public async Task<RequirementDto> GetRequirementByIdAsync(Guid id)
        {
            try
            {
                var requirement = await _requirementRepository.GetByIdAsync(id);
                return requirement != null ? MapToDto(requirement) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el requerimiento con ID {id}");
                throw;
            }
        }

        public async Task<List<RequirementDto>> GetAllRequirementsAsync()
        {
            try
            {
                var requirements = await _requirementRepository.GetAllAsync();
                return requirements.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los requerimientos");
                throw;
            }
        }

        public async Task<RequirementDto> CreateRequirementAsync(CreateRequirementDto dto)
        {
            try
            {
                var requirement = new Requirement(
                    dto.Title,
                    dto.Description,
                    dto.Priority
                );

                // Set additional properties
                requirement.SetProjectArea(dto.ProjectArea);
                requirement.SetCategory(dto.Category);
                requirement.SetCustomerInfo(dto.CustomerName, dto.CustomerEmail, dto.IsAnonymous);
                
                // Set acceptance criteria if provided
                if (!string.IsNullOrEmpty(dto.AcceptanceCriteria))
                    requirement.SetAcceptanceCriteria(dto.AcceptanceCriteria);
                
                // Set target date if provided
                if (dto.TargetDate.HasValue)
                    requirement.SetTargetDate(dto.TargetDate);

                var createdRequirement = await _requirementRepository.CreateAsync(requirement);
                return MapToDto(createdRequirement);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo requerimiento");
                throw;
            }
        }

        public async Task UpdateRequirementAsync(Guid id, UpdateRequirementDto dto)
        {
            try
            {
                var requirement = await _requirementRepository.GetByIdAsync(id);
                if (requirement == null)
                {
                    throw new KeyNotFoundException($"No se encontró el requerimiento con ID {id}");
                }

                if (!string.IsNullOrEmpty(dto.Title))
                    requirement.UpdateTitle(dto.Title);
                
                if (!string.IsNullOrEmpty(dto.Description))
                    requirement.UpdateDescription(dto.Description);
                
                if (!string.IsNullOrEmpty(dto.Priority))
                    requirement.UpdatePriority(dto.Priority);
                
                if (!string.IsNullOrEmpty(dto.Status))
                    requirement.SetStatus(dto.Status);
                
                if (!string.IsNullOrEmpty(dto.ProjectArea))
                    requirement.SetProjectArea(dto.ProjectArea);
                
                if (!string.IsNullOrEmpty(dto.Category))
                    requirement.SetCategory(dto.Category);
                
                if (!string.IsNullOrEmpty(dto.Response))
                    requirement.AddResponse(dto.Response);
                
                if (dto.CompletionPercentage.HasValue)
                    requirement.SetCompletionPercentage(dto.CompletionPercentage.Value);
                
                if (!string.IsNullOrEmpty(dto.AcceptanceCriteria))
                    requirement.SetAcceptanceCriteria(dto.AcceptanceCriteria);
                
                if (dto.TargetDate.HasValue)
                    requirement.SetTargetDate(dto.TargetDate);

                await _requirementRepository.UpdateAsync(requirement);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el requerimiento con ID {id}");
                throw;
            }
        }

        public async Task DeleteRequirementAsync(Guid id)
        {
            try
            {
                await _requirementRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar el requerimiento con ID {id}");
                throw;
            }
        }

        public async Task UpdateRequirementStatusAsync(Guid id, RequirementStatusUpdateDto statusUpdateDto)
        {
            try
            {
                var requirement = await _requirementRepository.GetByIdAsync(id);
                if (requirement == null)
                {
                    throw new KeyNotFoundException($"No se encontró el requerimiento con ID {id}");
                }

                if (!string.IsNullOrEmpty(statusUpdateDto.Status))
                    requirement.SetStatus(statusUpdateDto.Status);
                
                if (!string.IsNullOrEmpty(statusUpdateDto.Response))
                    requirement.AddResponse(statusUpdateDto.Response);
                
                if (statusUpdateDto.CompletionPercentage.HasValue)
                    requirement.SetCompletionPercentage(statusUpdateDto.CompletionPercentage.Value);

                await _requirementRepository.UpdateAsync(requirement);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el estado del requerimiento con ID {id}");
                throw;
            }
        }

        public async Task<List<RequirementDto>> GetRecentRequirementsAsync(int count)
        {
            try
            {
                var requirements = await _requirementRepository.GetRecentRequirementsAsync(count);
                return requirements.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los requerimientos recientes");
                throw;
            }
        }

        public async Task<List<RequirementDto>> GetRequirementsByStatusAsync(string status)
        {
            try
            {
                var requirements = await _requirementRepository.GetRequirementsByStatusAsync(status);
                return requirements.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener los requerimientos por estado {status}");
                throw;
            }
        }

        public async Task<List<RequirementDto>> GetRequirementsByPriorityAsync(string priority)
        {
            try
            {
                var requirements = await _requirementRepository.GetRequirementsByPriorityAsync(priority);
                return requirements.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener los requerimientos por prioridad {priority}");
                throw;
            }
        }

        public async Task<List<RequirementDto>> GetRequirementsByCategoryAsync(string category)
        {
            try
            {
                var requirements = await _requirementRepository.GetRequirementsByCategoryAsync(category);
                return requirements.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener los requerimientos por categoría {category}");
                throw;
            }
        }

        public async Task<List<RequirementDto>> GetRequirementsByProjectAreaAsync(string projectArea)
        {
            try
            {
                var requirements = await _requirementRepository.GetRequirementsByProjectAreaAsync(projectArea);
                return requirements.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener los requerimientos por área de proyecto {projectArea}");
                throw;
            }
        }

        public async Task<List<RequirementDto>> SearchRequirementsAsync(string searchTerm)
        {
            try
            {
                var requirements = await _requirementRepository.SearchRequirementsAsync(searchTerm);
                return requirements.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al buscar requerimientos con término '{searchTerm}'");
                throw;
            }
        }

        public async Task<RequirementReportsViewModel> GetRequirementReportsAsync()
        {
            try
            {
                var allRequirements = await _requirementRepository.GetAllAsync();
                var statusDistribution = await _requirementRepository.GetStatusDistributionAsync();
                var priorityDistribution = await _requirementRepository.GetPriorityDistributionAsync();
                var categoryDistribution = await _requirementRepository.GetCategoryDistributionAsync();
                var areaDistribution = await _requirementRepository.GetProjectAreaDistributionAsync();
                var monthlyRequirements = await _requirementRepository.GetMonthlyRequirementsCountAsync(6);

                return new RequirementReportsViewModel
                {
                    TotalRequirements = allRequirements.Count,
                    ProposedRequirements = allRequirements.Count(r => r.Status.ToLower() == "proposed"),
                    InProgressRequirements = allRequirements.Count(r => r.Status.ToLower() == "in-progress"),
                    ImplementedRequirements = allRequirements.Count(r => r.Status.ToLower() == "implemented"),
                    RejectedRequirements = allRequirements.Count(r => r.Status.ToLower() == "rejected"),
                    CategoryDistribution = categoryDistribution,
                    PriorityDistribution = priorityDistribution,
                    ProjectAreaDistribution = areaDistribution,
                    MonthlyRequirements = monthlyRequirements
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los informes de requerimientos");
                throw;
            }
        }

        public async Task AddResponseToRequirementAsync(Guid id, string response)
        {
            try
            {
                var requirement = await _requirementRepository.GetByIdAsync(id);
                if (requirement == null)
                {
                    throw new KeyNotFoundException($"No se encontró el requerimiento con ID {id}");
                }

                requirement.AddResponse(response);
                await _requirementRepository.UpdateAsync(requirement);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al añadir respuesta al requerimiento con ID {id}");
                throw;
            }
        }

        private RequirementDto MapToDto(Requirement requirement)
        {
            return new RequirementDto
            {
                Id = requirement.Id,
                Title = requirement.Title,
                Description = requirement.Description,
                Priority = requirement.Priority,
                CreatedAt = requirement.CreatedAt,
                UpdatedAt = requirement.UpdatedAt,
                Status = requirement.Status,
                ProjectArea = requirement.ProjectArea,
                CustomerName = requirement.CustomerName,
                CustomerEmail = requirement.CustomerEmail,
                IsAnonymous = requirement.IsAnonymous,
                Response = requirement.Response,
                ResponseDate = requirement.ResponseDate,
                CompletionPercentage = requirement.CompletionPercentage,
                Category = requirement.Category,
                AcceptanceCriteria = requirement.AcceptanceCriteria,
                TargetDate = requirement.TargetDate
            };
        }
    }
}
