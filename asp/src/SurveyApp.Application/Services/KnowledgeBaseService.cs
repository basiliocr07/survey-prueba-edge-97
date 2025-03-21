
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SurveyApp.Application.DTOs;
using SurveyApp.Application.Ports;
using SurveyApp.Domain.Entities;

namespace SurveyApp.Application.Services
{
    public class KnowledgeBaseService : IKnowledgeBaseService
    {
        private readonly IKnowledgeBaseRepository _knowledgeBaseRepository;
        private readonly ILogger<KnowledgeBaseService> _logger;

        public KnowledgeBaseService(
            IKnowledgeBaseRepository knowledgeBaseRepository,
            ILogger<KnowledgeBaseService> logger)
        {
            _knowledgeBaseRepository = knowledgeBaseRepository;
            _logger = logger;
        }

        public async Task<KnowledgeBaseItemDto> GetKnowledgeBaseItemByIdAsync(Guid id)
        {
            try
            {
                var item = await _knowledgeBaseRepository.GetByIdAsync(id);
                return item != null ? MapToDto(item) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting knowledge base item by ID: {id}");
                throw;
            }
        }

        public async Task<List<KnowledgeBaseItemDto>> GetAllKnowledgeBaseItemsAsync()
        {
            try
            {
                var items = await _knowledgeBaseRepository.GetAllAsync();
                return items.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all knowledge base items");
                throw;
            }
        }

        public async Task<KnowledgeBaseItemDto> CreateKnowledgeBaseItemAsync(CreateKnowledgeBaseItemDto itemDto)
        {
            try
            {
                var newItem = new KnowledgeBaseItem(
                    itemDto.Title,
                    itemDto.Content,
                    itemDto.Category,
                    itemDto.Tags);

                var createdItem = await _knowledgeBaseRepository.CreateAsync(newItem);
                return MapToDto(createdItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating knowledge base item");
                throw;
            }
        }

        public async Task UpdateKnowledgeBaseItemAsync(Guid id, UpdateKnowledgeBaseItemDto itemDto)
        {
            try
            {
                var item = await _knowledgeBaseRepository.GetByIdAsync(id);
                if (item == null)
                {
                    throw new InvalidOperationException($"Knowledge base item with ID {id} not found");
                }

                item.UpdateTitle(itemDto.Title);
                item.UpdateContent(itemDto.Content);
                item.UpdateCategory(itemDto.Category);
                item.UpdateTags(itemDto.Tags);

                await _knowledgeBaseRepository.UpdateAsync(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating knowledge base item with ID: {id}");
                throw;
            }
        }

        public async Task DeleteKnowledgeBaseItemAsync(Guid id)
        {
            try
            {
                await _knowledgeBaseRepository.DeleteAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting knowledge base item with ID: {id}");
                throw;
            }
        }

        public async Task<List<KnowledgeBaseItemDto>> SearchKnowledgeBaseAsync(string searchTerm)
        {
            try
            {
                var items = await _knowledgeBaseRepository.SearchAsync(searchTerm);
                return items.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error searching knowledge base with term: {searchTerm}");
                throw;
            }
        }

        public async Task<List<KnowledgeBaseItemDto>> GetRelatedItemsAsync(string topic, int count)
        {
            try
            {
                var items = await _knowledgeBaseRepository.GetRelatedItemsAsync(topic, count);
                return items.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting related knowledge base items for topic: {topic}");
                throw;
            }
        }

        public async Task<List<KnowledgeBaseItemDto>> GetMostRecentItemsAsync(int count)
        {
            try
            {
                var items = await _knowledgeBaseRepository.GetMostRecentAsync(count);
                return items.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting most recent knowledge base items");
                throw;
            }
        }

        private KnowledgeBaseItemDto MapToDto(KnowledgeBaseItem item)
        {
            return new KnowledgeBaseItemDto
            {
                Id = item.Id,
                Title = item.Title,
                Content = item.Content,
                Category = item.Category,
                Tags = item.Tags,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt
            };
        }
    }
}
