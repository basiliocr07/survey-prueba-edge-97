
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
    public class SurveyRepository : ISurveyRepository
    {
        private readonly AppDbContext _dbContext;

        public SurveyRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Survey> GetByIdAsync(Guid id)
        {
            return await _dbContext.Surveys
                .Include(s => s.Questions)
                .Include(s => s.DeliveryConfig)
                    .ThenInclude(d => d.Schedule)
                .Include(s => s.DeliveryConfig)
                    .ThenInclude(d => d.Trigger)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<Survey>> GetAllAsync()
        {
            return await _dbContext.Surveys
                .Include(s => s.Questions)
                .Include(s => s.DeliveryConfig)
                    .ThenInclude(d => d.Schedule)
                .Include(s => s.DeliveryConfig)
                    .ThenInclude(d => d.Trigger)
                .ToListAsync();
        }
        
        public async Task<(List<Survey> Surveys, int TotalCount)> GetPagedSurveysAsync(int pageNumber, int pageSize, string searchTerm = null, string statusFilter = null, string categoryFilter = null)
        {
            var query = _dbContext.Surveys
                .Include(s => s.Questions)
                .Include(s => s.DeliveryConfig)
                    .ThenInclude(d => d.Schedule)
                .Include(s => s.DeliveryConfig)
                    .ThenInclude(d => d.Trigger)
                .AsQueryable();
                
            // Apply filters
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(s => s.Title.ToLower().Contains(searchTerm) || 
                                         s.Description.ToLower().Contains(searchTerm));
            }
            
            // We'll need to add Status and Category properties to the Survey entity in a future update
            // For now, we'll just return the full list
            
            // Get total count before pagination
            var totalCount = await query.CountAsync();
            
            // Apply pagination
            var pagedSurveys = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
                
            return (pagedSurveys, totalCount);
        }

        public async Task<Survey> CreateAsync(Survey survey)
        {
            await _dbContext.Surveys.AddAsync(survey);
            await _dbContext.SaveChangesAsync();
            return survey;
        }

        public async Task UpdateAsync(Survey survey)
        {
            // Primero obtenemos las preguntas actuales para comparar
            var existingSurvey = await _dbContext.Surveys
                .Include(s => s.Questions)
                .Include(s => s.DeliveryConfig)
                    .ThenInclude(d => d.Schedule)
                .Include(s => s.DeliveryConfig)
                    .ThenInclude(d => d.Trigger)
                .FirstOrDefaultAsync(s => s.Id == survey.Id);

            if (existingSurvey == null)
            {
                throw new KeyNotFoundException($"Survey with ID {survey.Id} not found.");
            }

            // Actualizamos propiedades básicas
            _dbContext.Entry(existingSurvey).CurrentValues.SetValues(survey);

            // Actualizar preguntas
            // Eliminar preguntas que ya no existen
            foreach (var existingQuestion in existingSurvey.Questions.ToList())
            {
                if (!survey.Questions.Any(q => q.Id == existingQuestion.Id))
                {
                    existingSurvey.Questions.Remove(existingQuestion);
                }
            }

            // Actualizar o agregar preguntas
            foreach (var question in survey.Questions)
            {
                var existingQuestion = existingSurvey.Questions.FirstOrDefault(q => q.Id == question.Id);
                
                if (existingQuestion != null)
                {
                    // Actualizar pregunta existente
                    _dbContext.Entry(existingQuestion).CurrentValues.SetValues(question);
                }
                else
                {
                    // Agregar nueva pregunta
                    existingSurvey.Questions.Add(question);
                }
            }

            // Actualizar configuración de entrega si existe
            if (survey.DeliveryConfig != null)
            {
                if (existingSurvey.DeliveryConfig == null)
                {
                    existingSurvey.DeliveryConfig = survey.DeliveryConfig;
                }
                else
                {
                    // Set the delivery type
                    existingSurvey.DeliveryConfig.SetType(survey.DeliveryConfig.Type);
                    
                    // Update email addresses
                    existingSurvey.DeliveryConfig.EmailAddresses.Clear();
                    foreach (var email in survey.DeliveryConfig.EmailAddresses)
                    {
                        existingSurvey.DeliveryConfig.AddEmailAddress(email);
                    }
                    
                    // Update Schedule using the SetSchedule method
                    if (survey.DeliveryConfig.Schedule != null)
                    {
                        existingSurvey.DeliveryConfig.SetSchedule(survey.DeliveryConfig.Schedule);
                    }
                    
                    // Update Trigger using the SetTrigger method
                    if (survey.DeliveryConfig.Trigger != null)
                    {
                        existingSurvey.DeliveryConfig.SetTrigger(survey.DeliveryConfig.Trigger);
                    }
                }
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var survey = await _dbContext.Surveys
                .Include(s => s.Questions)
                .Include(s => s.DeliveryConfig)
                    .ThenInclude(d => d.Schedule)
                .Include(s => s.DeliveryConfig)
                    .ThenInclude(d => d.Trigger)
                .FirstOrDefaultAsync(s => s.Id == id);
                
            if (survey != null)
            {
                _dbContext.Surveys.Remove(survey);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await _dbContext.Surveys.AnyAsync(s => s.Id == id);
        }
        
        public async Task<List<string>> GetAllCategoriesAsync()
        {
            // For now, we'll return a list of predefined categories
            // In the future, this should be dynamic based on the data
            return await Task.FromResult(new List<string> 
            { 
                "Customer Satisfaction", 
                "Product Feedback", 
                "Employee Engagement", 
                "Market Research", 
                "Event Feedback"
            });
        }
    }
}
