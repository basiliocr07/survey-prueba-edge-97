
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SurveyApp.Domain.Entities;

namespace SurveyApp.Application.Ports
{
    public interface IKnowledgeBaseRepository
    {
        Task<KnowledgeBaseItem> GetByIdAsync(Guid id);
        Task<List<KnowledgeBaseItem>> GetAllAsync();
        Task<KnowledgeBaseItem> CreateAsync(KnowledgeBaseItem item);
        Task UpdateAsync(KnowledgeBaseItem item);
        Task DeleteAsync(Guid id);
        Task<List<KnowledgeBaseItem>> SearchAsync(string searchTerm);
        Task<List<KnowledgeBaseItem>> GetRelatedItemsAsync(string topic, int count);
        Task<List<KnowledgeBaseItem>> GetMostRecentAsync(int count);
    }
}
