
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SurveyApp.Domain.Entities;

namespace SurveyApp.Application.Ports
{
    public interface IKnowledgeBaseRepository
    {
        Task<List<KnowledgeBaseItem>> GetAllAsync();
        Task<KnowledgeBaseItem> GetByIdAsync(Guid id);
        Task<List<KnowledgeBaseItem>> GetByCategoryAsync(string category);
        Task<List<KnowledgeBaseItem>> SearchAsync(string searchTerm);
        Task<KnowledgeBaseItem> CreateAsync(KnowledgeBaseItem item);
        Task UpdateAsync(KnowledgeBaseItem item);
        Task DeleteAsync(Guid id);
    }
}
