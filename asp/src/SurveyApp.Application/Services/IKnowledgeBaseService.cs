
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SurveyApp.Domain.Entities;

namespace SurveyApp.Application.Services
{
    public interface IKnowledgeBaseService
    {
        Task<List<KnowledgeBaseItem>> GetAllItemsAsync();
        Task<KnowledgeBaseItem> GetItemByIdAsync(Guid id);
        Task<KnowledgeBaseItem> CreateItemAsync(KnowledgeBaseItem item);
        Task UpdateItemAsync(KnowledgeBaseItem item);
        Task DeleteItemAsync(Guid id);
        Task<List<KnowledgeBaseItem>> GetItemsByCategoryAsync(string category);
        Task<List<KnowledgeBaseItem>> GetItemsByTagAsync(string tag);
        Task<List<KnowledgeBaseItem>> SearchItemsAsync(string searchTerm);
        Task<List<KnowledgeBaseItem>> GetRelatedItemsAsync(string category, int count);
        Task<List<KnowledgeBaseItem>> GetPopularItemsAsync(int count);
    }
}
