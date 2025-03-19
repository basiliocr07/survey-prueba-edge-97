
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SurveyApp.Application.DTOs;
using SurveyApp.Application.Ports;
using SurveyApp.Application.Services;
using SurveyApp.Domain.Entities;

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

        public async Task<RequirementDto> CreateRequirementAsync(CreateRequirementDto requirementDto)
        {
            try
            {
                var requirement = new Requirement(
                    requirementDto.Title,
                    requirementDto.Description,
                    requirementDto.Priority
                );

                var createdRequirement = await _requirementRepository.CreateAsync(requirement);
                return MapToDto(createdRequirement);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo requerimiento");
                throw;
            }
        }

        public async Task UpdateRequirementAsync(Guid id, UpdateRequirementDto requirementDto)
        {
            try
            {
                var requirement = await _requirementRepository.GetByIdAsync(id);
                if (requirement == null)
                {
                    throw new KeyNotFoundException($"No se encontró el requerimiento con ID {id}");
                }

                requirement.UpdateTitle(requirementDto.Title);
                requirement.UpdateDescription(requirementDto.Description);
                requirement.UpdatePriority(requirementDto.Priority);
                
                if (!string.IsNullOrEmpty(requirementDto.Status))
                {
                    requirement.SetStatus(requirementDto.Status);
                }

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

        public async Task UpdateRequirementStatusAsync(Guid id, string status)
        {
            try
            {
                var requirement = await _requirementRepository.GetByIdAsync(id);
                if (requirement == null)
                {
                    throw new KeyNotFoundException($"No se encontró el requerimiento con ID {id}");
                }

                requirement.SetStatus(status);
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
                var requirements = await _requirementRepository.GetAllAsync();
                return requirements
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(count)
                    .Select(MapToDto)
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los requerimientos recientes");
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
                Status = requirement.Status
            };
        }
    }
}
