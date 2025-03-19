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
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<Survey>> GetAllAsync()
        {
            return await _dbContext.Surveys
                .Include(s => s.Questions)
                .Include(s => s.DeliveryConfig)
                .ToListAsync();
        }
        
        public async Task<(List<Survey> Surveys, int TotalCount)> GetPagedSurveysAsync(int pageNumber, int pageSize, string searchTerm = null, string statusFilter = null, string categoryFilter = null)
        {
            var query = _dbContext.Surveys
                .Include(s => s.Questions)
                .Include(s => s.DeliveryConfig)
                .AsQueryable();
                
            // Apply filters
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(s => s.Title.ToLower().Contains(searchTerm) || 
                                         s.Description.ToLower().Contains(searchTerm));
            }
            
            if (!string.IsNullOrWhiteSpace(statusFilter))
            {
                query = query.Where(s => s.Status == statusFilter);
            }
            
            if (!string.IsNullOrWhiteSpace(categoryFilter))
            {
                query = query.Where(s => s.Category == categoryFilter);
            }
            
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
                .FirstOrDefaultAsync(s => s.Id == survey.Id);

            if (existingSurvey == null)
            {
                throw new KeyNotFoundException($"Survey with ID {survey.Id} not found.");
            }

            // Actualizamos propiedades básicas
            existingSurvey.UpdateTitle(survey.Title);
            existingSurvey.UpdateDescription(survey.Description);
            existingSurvey.SetStatus(survey.Status);
            existingSurvey.SetCategory(survey.Category);
            existingSurvey.SetFeatured(survey.IsFeatured);
            
            if (!string.IsNullOrEmpty(survey.CreatedBy))
            {
                existingSurvey.SetCreatedBy(survey.CreatedBy);
            }

            // Actualizar preguntas
            // Primero, hacemos un seguimiento de los IDs de preguntas actuales para evitar eliminar y recrear
            var existingQuestionIds = existingSurvey.Questions.Select(q => q.Id).ToList();
            var updatedQuestionIds = survey.Questions.Select(q => q.Id).ToList();
            
            // Eliminar preguntas que ya no existen
            foreach (var existingQuestion in existingSurvey.Questions.ToList())
            {
                if (!updatedQuestionIds.Contains(existingQuestion.Id))
                {
                    _dbContext.Questions.Remove(existingQuestion);
                }
            }

            // Actualizar o agregar preguntas
            foreach (var question in survey.Questions)
            {
                var existingQuestion = existingSurvey.Questions.FirstOrDefault(q => q.Id == question.Id);
                
                if (existingQuestion != null)
                {
                    // Actualizar pregunta existente
                    existingQuestion.UpdateTitle(question.Title);
                    existingQuestion.UpdateDescription(question.Description);
                    existingQuestion.SetRequired(question.Required);
                    
                    // Solo actualizar tipo si es diferente para evitar problemas con las opciones
                    if (existingQuestion.Type != question.Type)
                    {
                        existingQuestion.ChangeType(question.Type);
                    }
                    
                    // Actualizar opciones
                    if (question.Options != null && question.Options.Any())
                    {
                        existingQuestion.SetOptions(question.Options);
                    }
                    
                    _dbContext.Entry(existingQuestion).State = EntityState.Modified;
                }
                else
                {
                    // Agregar nueva pregunta al contexto
                    _dbContext.Questions.Add(question);
                    
                    // También añadir referencia a la encuesta
                    existingSurvey.AddQuestion(question);
                }
            }

            // Actualizar configuración de entrega si existe
            if (survey.DeliveryConfig != null)
            {
                if (existingSurvey.DeliveryConfig == null)
                {
                    existingSurvey.SetDeliveryConfig(survey.DeliveryConfig);
                }
                else
                {
                    // Update the existing delivery config
                    var deliveryConfig = existingSurvey.DeliveryConfig;
                    
                    // Set the delivery type
                    deliveryConfig.SetType(survey.DeliveryConfig.Type);
                    
                    // Update email addresses
                    deliveryConfig.EmailAddresses.Clear();
                    foreach (var email in survey.DeliveryConfig.EmailAddresses)
                    {
                        deliveryConfig.AddEmailAddress(email);
                    }
                    
                    // Update Schedule
                    if (survey.DeliveryConfig.Schedule != null)
                    {
                        deliveryConfig.SetSchedule(survey.DeliveryConfig.Schedule);
                    }
                    
                    // Update Trigger
                    if (survey.DeliveryConfig.Trigger != null)
                    {
                        deliveryConfig.SetTrigger(survey.DeliveryConfig.Trigger);
                    }
                }
            }

            _dbContext.Entry(existingSurvey).State = EntityState.Modified;
            
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var survey = await _dbContext.Surveys
                .Include(s => s.Questions)
                .Include(s => s.DeliveryConfig)
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
            var categories = await _dbContext.Surveys
                .Where(s => !string.IsNullOrEmpty(s.Category))
                .Select(s => s.Category)
                .Distinct()
                .ToListAsync();

            // If no categories exist yet, return some default categories
            if (categories == null || categories.Count == 0)
            {
                return new List<string> 
                { 
                    "Customer Satisfaction", 
                    "Product Feedback", 
                    "Employee Engagement", 
                    "Market Research", 
                    "Event Feedback"
                };
            }

            return categories;
        }
        
        public async Task<bool> SaveChangesAsync()
        {
            return await _dbContext.SaveChangesAsync() > 0;
        }
    }
}
