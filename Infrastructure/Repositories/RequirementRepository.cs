
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SurveyApp.Application.Ports;
using SurveyApp.Domain.Entities;
using SurveyApp.Infrastructure.Data;

namespace SurveyApp.Infrastructure.Repositories
{
    public class RequirementRepository : IRequirementRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<RequirementRepository> _logger;

        public RequirementRepository(AppDbContext context, ILogger<RequirementRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Requirement> GetByIdAsync(Guid id)
        {
            try
            {
                return await _context.Requirements.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el requerimiento con ID {id}");
                throw;
            }
        }

        public async Task<List<Requirement>> GetAllAsync()
        {
            try
            {
                return await _context.Requirements.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los requerimientos");
                throw;
            }
        }

        public async Task<Requirement> CreateAsync(Requirement requirement)
        {
            try
            {
                _context.Requirements.Add(requirement);
                await _context.SaveChangesAsync();
                return requirement;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear un nuevo requerimiento");
                throw;
            }
        }

        public async Task UpdateAsync(Requirement requirement)
        {
            try
            {
                _context.Entry(requirement).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el requerimiento con ID {requirement.Id}");
                throw;
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            try
            {
                var requirement = await _context.Requirements.FindAsync(id);
                if (requirement != null)
                {
                    _context.Requirements.Remove(requirement);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar el requerimiento con ID {id}");
                throw;
            }
        }
    }
}
