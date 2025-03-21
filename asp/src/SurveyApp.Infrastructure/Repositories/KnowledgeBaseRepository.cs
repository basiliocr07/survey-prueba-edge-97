
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
    public class KnowledgeBaseRepository : IKnowledgeBaseRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<KnowledgeBaseRepository> _logger;

        public KnowledgeBaseRepository(AppDbContext context, ILogger<KnowledgeBaseRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<KnowledgeBaseItem> GetByIdAsync(Guid id)
        {
            try
            {
                return await _context.KnowledgeBaseItems.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting knowledge base item with ID: {id}");
                throw;
            }
        }

        public async Task<List<KnowledgeBaseItem>> GetAllAsync()
        {
            try
            {
                return await _context.KnowledgeBaseItems
                    .OrderByDescending(k => k.UpdatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all knowledge base items");
                throw;
            }
        }

        public async Task<KnowledgeBaseItem> CreateAsync(KnowledgeBaseItem item)
        {
            try
            {
                _context.KnowledgeBaseItems.Add(item);
                await _context.SaveChangesAsync();
                return item;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating knowledge base item");
                throw;
            }
        }

        public async Task UpdateAsync(KnowledgeBaseItem item)
        {
            try
            {
                _context.Entry(item).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating knowledge base item with ID: {item.Id}");
                throw;
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            try
            {
                var item = await _context.KnowledgeBaseItems.FindAsync(id);
                if (item != null)
                {
                    _context.KnowledgeBaseItems.Remove(item);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting knowledge base item with ID: {id}");
                throw;
            }
        }

        public async Task<List<KnowledgeBaseItem>> SearchAsync(string searchTerm)
        {
            try
            {
                return await _context.KnowledgeBaseItems
                    .Where(k => k.Title.Contains(searchTerm) || 
                               k.Content.Contains(searchTerm) ||
                               k.Tags.Any(t => t.Contains(searchTerm)))
                    .OrderByDescending(k => k.UpdatedAt)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error searching knowledge base with term: {searchTerm}");
                throw;
            }
        }

        public async Task<List<KnowledgeBaseItem>> GetRelatedItemsAsync(string topic, int count)
        {
            try
            {
                // Simple implementation to find items with similar words in title or content
                var words = topic.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Where(w => w.Length > 3)  // Skip small words
                    .ToList();

                // If no significant words, return most recent items
                if (words.Count == 0)
                {
                    return await GetMostRecentAsync(count);
                }

                // Find items containing any of the significant words
                var query = _context.KnowledgeBaseItems.AsQueryable();
                foreach (var word in words)
                {
                    query = query.Where(k => 
                        k.Title.Contains(word) || 
                        k.Content.Contains(word) ||
                        k.Tags.Any(t => t.Contains(word)));
                }

                return await query
                    .OrderByDescending(k => k.UpdatedAt)
                    .Take(count)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting related knowledge base items for topic: {topic}");
                // Fallback to recent items on error
                return await GetMostRecentAsync(count);
            }
        }

        public async Task<List<KnowledgeBaseItem>> GetMostRecentAsync(int count)
        {
            try
            {
                return await _context.KnowledgeBaseItems
                    .OrderByDescending(k => k.UpdatedAt)
                    .Take(count)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting most recent knowledge base items");
                throw;
            }
        }
    }
}
