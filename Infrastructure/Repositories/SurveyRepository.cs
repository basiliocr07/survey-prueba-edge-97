
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
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<Survey>> GetAllAsync()
        {
            return await _dbContext.Surveys
                .Include(s => s.Questions)
                .Include(s => s.DeliveryConfig)
                .AsNoTracking()
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
                .AsNoTracking()
                .ToListAsync();
                
            return (pagedSurveys, totalCount);
        }

        public async Task<Survey> CreateAsync(Survey survey)
        {
            // Ensure entity is properly set up for insert
            survey.Id = survey.Id == Guid.Empty ? Guid.NewGuid() : survey.Id;
            
            foreach (var question in survey.Questions)
            {
                question.Id = question.Id == Guid.Empty ? Guid.NewGuid() : question.Id;
            }
            
            await _dbContext.Surveys.AddAsync(survey);
            await _dbContext.SaveChangesAsync();
            
            // Detach entity after save to prevent tracking issues
            _dbContext.Entry(survey).State = EntityState.Detached;
            
            return survey;
        }

        public async Task UpdateAsync(Survey survey)
        {
            // Use a disconnected approach to avoid tracking issues
            _dbContext.ChangeTracker.Clear();
            
            // First, get the existing survey
            var existingSurvey = await _dbContext.Surveys
                .Include(s => s.Questions)
                .FirstOrDefaultAsync(s => s.Id == survey.Id);

            if (existingSurvey == null)
            {
                throw new KeyNotFoundException($"Survey with ID {survey.Id} not found.");
            }

            // Update basic properties
            _dbContext.Entry(existingSurvey).CurrentValues.SetValues(survey);
            
            // Handle questions - remove ones that no longer exist
            foreach (var existingQuestion in existingSurvey.Questions.ToList())
            {
                if (!survey.Questions.Any(q => q.Id == existingQuestion.Id))
                {
                    _dbContext.Questions.Remove(existingQuestion);
                }
            }
            
            // Update or add questions
            foreach (var questionModel in survey.Questions)
            {
                var existingQuestion = existingSurvey.Questions.FirstOrDefault(q => q.Id == questionModel.Id);
                
                if (existingQuestion != null)
                {
                    // Update existing question
                    _dbContext.Entry(existingQuestion).CurrentValues.SetValues(questionModel);
                    
                    // Update options if needed
                    if (questionModel.Options != null)
                    {
                        existingQuestion.SetOptions(questionModel.Options);
                    }
                }
                else
                {
                    // Ensure new question has an ID
                    if (questionModel.Id == Guid.Empty)
                    {
                        questionModel.Id = Guid.NewGuid();
                    }
                    
                    // Add new question
                    existingSurvey.AddQuestion(questionModel);
                }
            }
            
            // Update delivery config if present
            if (survey.DeliveryConfig != null)
            {
                if (existingSurvey.DeliveryConfig == null)
                {
                    existingSurvey.SetDeliveryConfig(survey.DeliveryConfig);
                }
                else
                {
                    var deliveryConfig = existingSurvey.DeliveryConfig;
                    deliveryConfig.SetType(survey.DeliveryConfig.Type);
                    
                    // Update email addresses
                    deliveryConfig.EmailAddresses.Clear();
                    foreach (var email in survey.DeliveryConfig.EmailAddresses)
                    {
                        deliveryConfig.AddEmailAddress(email);
                    }
                    
                    // Update Schedule and Trigger if present
                    if (survey.DeliveryConfig.Schedule != null)
                    {
                        deliveryConfig.SetSchedule(survey.DeliveryConfig.Schedule);
                    }
                    
                    if (survey.DeliveryConfig.Trigger != null)
                    {
                        deliveryConfig.SetTrigger(survey.DeliveryConfig.Trigger);
                    }
                }
            }
            
            await _dbContext.SaveChangesAsync();
            
            // Detach entities after save
            _dbContext.ChangeTracker.Clear();
        }

        public async Task DeleteAsync(Guid id)
        {
            var survey = await _dbContext.Surveys
                .Include(s => s.Questions)
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
            // Corregimos la comparación con el método SaveChangesAsync()
            // El método SaveChangesAsync() retorna un int, y necesitamos compararlo con 0
            return (await _dbContext.SaveChangesAsync()) > 0;
        }
    }
}
