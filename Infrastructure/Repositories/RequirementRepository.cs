
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
                return await _context.Requirements
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();
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
        
        public async Task<List<Requirement>> GetRecentRequirementsAsync(int count)
        {
            try
            {
                return await _context.Requirements
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(count)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener los requerimientos recientes");
                throw;
            }
        }
        
        public async Task<List<Requirement>> GetRequirementsByStatusAsync(string status)
        {
            try
            {
                return await _context.Requirements
                    .Where(r => r.Status.ToLower() == status.ToLower())
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener los requerimientos por estado {status}");
                throw;
            }
        }
        
        public async Task<List<Requirement>> GetRequirementsByPriorityAsync(string priority)
        {
            try
            {
                return await _context.Requirements
                    .Where(r => r.Priority.ToLower() == priority.ToLower())
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener los requerimientos por prioridad {priority}");
                throw;
            }
        }
        
        public async Task<List<Requirement>> GetRequirementsByCategoryAsync(string category)
        {
            try
            {
                return await _context.Requirements
                    .Where(r => r.Category.ToLower() == category.ToLower())
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener los requerimientos por categoría {category}");
                throw;
            }
        }
        
        public async Task<List<Requirement>> SearchRequirementsAsync(string searchTerm)
        {
            try
            {
                return await _context.Requirements
                    .Where(r => r.Title.Contains(searchTerm) || 
                               r.Description.Contains(searchTerm) ||
                               r.ProjectArea.Contains(searchTerm) ||
                               r.CustomerName.Contains(searchTerm))
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al buscar requerimientos con término '{searchTerm}'");
                throw;
            }
        }
        
        public async Task<List<Requirement>> GetRequirementsByProjectAreaAsync(string projectArea)
        {
            try
            {
                return await _context.Requirements
                    .Where(r => r.ProjectArea.ToLower() == projectArea.ToLower())
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener los requerimientos por área de proyecto {projectArea}");
                throw;
            }
        }

        public async Task<Dictionary<string, int>> GetStatusDistributionAsync()
        {
            try
            {
                return await _context.Requirements
                    .GroupBy(r => r.Status)
                    .Select(g => new { Status = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Status, x => x.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la distribución por estado");
                throw;
            }
        }

        public async Task<Dictionary<string, int>> GetPriorityDistributionAsync()
        {
            try
            {
                return await _context.Requirements
                    .GroupBy(r => r.Priority)
                    .Select(g => new { Priority = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Priority, x => x.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la distribución por prioridad");
                throw;
            }
        }

        public async Task<Dictionary<string, int>> GetCategoryDistributionAsync()
        {
            try
            {
                return await _context.Requirements
                    .Where(r => !string.IsNullOrEmpty(r.Category))
                    .GroupBy(r => r.Category)
                    .Select(g => new { Category = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Category, x => x.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la distribución por categoría");
                throw;
            }
        }

        public async Task<Dictionary<string, int>> GetProjectAreaDistributionAsync()
        {
            try
            {
                return await _context.Requirements
                    .GroupBy(r => r.ProjectArea)
                    .Select(g => new { Area = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.Area, x => x.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la distribución por área de proyecto");
                throw;
            }
        }

        public async Task<Dictionary<string, int>> GetMonthlyRequirementsCountAsync(int months)
        {
            try
            {
                var startDate = DateTime.UtcNow.AddMonths(-months);
                
                var requirements = await _context.Requirements
                    .Where(r => r.CreatedAt >= startDate)
                    .ToListAsync();

                var monthlyData = new Dictionary<string, int>();
                
                for (int i = 0; i < months; i++)
                {
                    var month = DateTime.UtcNow.AddMonths(-i);
                    var key = month.ToString("MMM yyyy");
                    var count = requirements.Count(r => 
                        r.CreatedAt.Month == month.Month && 
                        r.CreatedAt.Year == month.Year);
                    
                    monthlyData.Add(key, count);
                }

                return monthlyData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el conteo mensual de requerimientos");
                throw;
            }
        }
    }
}
