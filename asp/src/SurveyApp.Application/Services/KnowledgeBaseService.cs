
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SurveyApp.Application.DTOs;
using SurveyApp.Application.Ports;
using SurveyApp.Domain.Entities;

namespace SurveyApp.Application.Services
{
    public class KnowledgeBaseService : IKnowledgeBaseService
    {
        private readonly IKnowledgeBaseRepository _knowledgeBaseRepository;

        public KnowledgeBaseService(IKnowledgeBaseRepository knowledgeBaseRepository)
        {
            _knowledgeBaseRepository = knowledgeBaseRepository;
        }

        public async Task<KnowledgeBaseItemDto> GetKnowledgeBaseItemByIdAsync(Guid id)
        {
            var item = await _knowledgeBaseRepository.GetByIdAsync(id);
            if (item == null)
            {
                throw new KeyNotFoundException($"Knowledge base item with ID {id} not found");
            }
            return MapToDto(item);
        }

        public async Task<List<KnowledgeBaseItemDto>> GetAllKnowledgeBaseItemsAsync()
        {
            var items = await _knowledgeBaseRepository.GetAllAsync();
            return items.Select(MapToDto).ToList();
        }

        public async Task<KnowledgeBaseItemDto> CreateKnowledgeBaseItemAsync(CreateKnowledgeBaseItemDto itemDto)
        {
            var item = new KnowledgeBaseItem
            {
                Title = itemDto.Title,
                Content = itemDto.Content,
                Category = itemDto.Category,
                Tags = itemDto.Tags ?? new List<string>(),
                Author = itemDto.Author,
                CreatedAt = DateTime.UtcNow,
                IsPublished = true
            };

            var createdItem = await _knowledgeBaseRepository.CreateAsync(item);
            return MapToDto(createdItem);
        }

        public async Task UpdateKnowledgeBaseItemAsync(Guid id, UpdateKnowledgeBaseItemDto itemDto)
        {
            var existingItem = await _knowledgeBaseRepository.GetByIdAsync(id);
            if (existingItem == null)
            {
                throw new KeyNotFoundException($"Knowledge base item with ID {id} not found");
            }

            existingItem.Title = itemDto.Title;
            existingItem.Content = itemDto.Content;
            existingItem.Category = itemDto.Category;
            existingItem.Tags = itemDto.Tags ?? new List<string>();
            existingItem.UpdatedAt = DateTime.UtcNow;
            existingItem.IsPublished = itemDto.IsPublished;

            await _knowledgeBaseRepository.UpdateAsync(existingItem);
        }

        public async Task DeleteKnowledgeBaseItemAsync(Guid id)
        {
            var existingItem = await _knowledgeBaseRepository.GetByIdAsync(id);
            if (existingItem == null)
            {
                throw new KeyNotFoundException($"Knowledge base item with ID {id} not found");
            }

            await _knowledgeBaseRepository.DeleteAsync(id);
        }

        public async Task<List<KnowledgeBaseItemDto>> SearchKnowledgeBaseAsync(string searchTerm)
        {
            var items = await _knowledgeBaseRepository.SearchAsync(searchTerm);
            return items.Select(MapToDto).ToList();
        }

        public async Task<List<KnowledgeBaseItemDto>> GetRelatedItemsAsync(string topic, int count)
        {
            var items = await _knowledgeBaseRepository.GetRelatedItemsAsync(topic, count);
            return items.Select(MapToDto).ToList();
        }

        public async Task<List<KnowledgeBaseItemDto>> GetMostRecentItemsAsync(int count)
        {
            var items = await _knowledgeBaseRepository.GetMostRecentAsync(count);
            return items.Select(MapToDto).ToList();
        }

        private static KnowledgeBaseItemDto MapToDto(KnowledgeBaseItem item)
        {
            return new KnowledgeBaseItemDto
            {
                Id = item.Id,
                Title = item.Title,
                Content = item.Content,
                Category = item.Category,
                Tags = item.Tags,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt,
                Author = item.Author,
                ViewCount = item.ViewCount,
                IsPublished = item.IsPublished
            };
        }
    }
}
