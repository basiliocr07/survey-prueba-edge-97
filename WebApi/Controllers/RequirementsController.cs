
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SurveyApp.Application.DTOs;
using SurveyApp.Application.Services;

namespace SurveyApp.WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RequirementsController : ControllerBase
    {
        private readonly IRequirementService _requirementService;

        public RequirementsController(IRequirementService requirementService)
        {
            _requirementService = requirementService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RequirementDto>>> GetRequirements()
        {
            var requirements = await _requirementService.GetAllRequirementsAsync();
            return Ok(requirements);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RequirementDto>> GetRequirement(Guid id)
        {
            var requirement = await _requirementService.GetRequirementByIdAsync(id);
            if (requirement == null)
            {
                return NotFound();
            }
            return Ok(requirement);
        }

        [HttpPost]
        public async Task<ActionResult<RequirementDto>> CreateRequirement(CreateRequirementDto createRequirementDto)
        {
            var requirement = await _requirementService.CreateRequirementAsync(createRequirementDto);
            return CreatedAtAction(nameof(GetRequirement), new { id = requirement.Id }, requirement);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRequirement(Guid id, UpdateRequirementDto updateRequirementDto)
        {
            var result = await _requirementService.UpdateRequirementAsync(id, updateRequirementDto);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateRequirementStatus(Guid id, RequirementStatusUpdateDto statusUpdateDto)
        {
            var result = await _requirementService.UpdateRequirementStatusAsync(id, statusUpdateDto);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRequirement(Guid id)
        {
            var result = await _requirementService.DeleteRequirementAsync(id);
            if (!result)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpGet("statistics")]
        public async Task<ActionResult<object>> GetStatistics()
        {
            var requirements = await _requirementService.GetAllRequirementsAsync();
            
            var totalCount = requirements.Count;
            var proposedCount = requirements.Count(r => r.Status.ToLower() == "proposed");
            var inProgressCount = requirements.Count(r => r.Status.ToLower() == "in-progress");
            var implementedCount = requirements.Count(r => r.Status.ToLower() == "implemented");
            var rejectedCount = requirements.Count(r => r.Status.ToLower() == "rejected");

            return Ok(new
            {
                totalCount,
                proposedCount,
                inProgressCount,
                implementedCount,
                rejectedCount
            });
        }
    }
}
