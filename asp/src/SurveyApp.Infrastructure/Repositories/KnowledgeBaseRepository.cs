
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SurveyApp.Application.Ports;
using SurveyApp.Domain.Entities;
using SurveyApp.Infrastructure.Data;

namespace SurveyApp.Infrastructure.Repositories
{
    public class KnowledgeBaseRepository : IKnowledgeBaseRepository
    {
        private readonly AppDbContext _dbContext;

        public KnowledgeBaseRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<KnowledgeBaseItem> GetByIdAsync(Guid id)
        {
            return await _dbContext.KnowledgeBaseItems.FindAsync(id);
        }

        public async Task<List<KnowledgeBaseItem>> GetAllAsync()
        {
            return await _dbContext.KnowledgeBaseItems
                .OrderByDescending(k => k.CreatedAt)
                .ToListAsync();
        }

        public async Task<KnowledgeBaseItem> CreateAsync(KnowledgeBaseItem item)
        {
            await _dbContext.KnowledgeBaseItems.AddAsync(item);
            await _dbContext.SaveChangesAsync();
            return item;
        }

        public async Task UpdateAsync(KnowledgeBaseItem item)
        {
            _dbContext.KnowledgeBaseItems.Update(item);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var item = await _dbContext.KnowledgeBaseItems.FindAsync(id);
            if (item != null)
            {
                _dbContext.KnowledgeBaseItems.Remove(item);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<KnowledgeBaseItem>> SearchAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllAsync();

            return await _dbContext.KnowledgeBaseItems
                .Where(k => k.Title.Contains(searchTerm) || 
                           k.Content.Contains(searchTerm) || 
                           k.Category.Contains(searchTerm) ||
                           k.Tags.Any(t => t.Contains(searchTerm)))
                .OrderByDescending(k => k.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<KnowledgeBaseItem>> GetRelatedItemsAsync(string topic, int count)
        {
            if (string.IsNullOrWhiteSpace(topic))
                return await GetMostRecentAsync(count);

            return await _dbContext.KnowledgeBaseItems
                .Where(k => k.Category == topic || k.Tags.Contains(topic))
                .OrderByDescending(k => k.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task<List<KnowledgeBaseItem>> GetMostRecentAsync(int count)
        {
            return await _dbContext.KnowledgeBaseItems
                .OrderByDescending(k => k.CreatedAt)
                .Take(count)
                .ToListAsync();
        }
    }
}
