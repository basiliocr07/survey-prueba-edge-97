
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SurveyApp.Application.DTOs;

namespace SurveyApp.Application.Services
{
    public interface IKnowledgeBaseService
    {
        Task<KnowledgeBaseItemDto> GetKnowledgeBaseItemByIdAsync(Guid id);
        Task<List<KnowledgeBaseItemDto>> GetAllKnowledgeBaseItemsAsync();
        Task<KnowledgeBaseItemDto> CreateKnowledgeBaseItemAsync(CreateKnowledgeBaseItemDto itemDto);
        Task UpdateKnowledgeBaseItemAsync(Guid id, UpdateKnowledgeBaseItemDto itemDto);
        Task DeleteKnowledgeBaseItemAsync(Guid id);
        Task<List<KnowledgeBaseItemDto>> SearchKnowledgeBaseAsync(string searchTerm);
        Task<List<KnowledgeBaseItemDto>> GetRelatedItemsAsync(string topic, int count);
        Task<List<KnowledgeBaseItemDto>> GetMostRecentItemsAsync(int count);
    }
}
