
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
                    _dbContext.Entry(existingSurvey.DeliveryConfig).CurrentValues.SetValues(survey.DeliveryConfig);
                    
                    // Actualizar Schedule
                    if (survey.DeliveryConfig.Schedule != null)
                    {
                        if (existingSurvey.DeliveryConfig.Schedule == null)
                        {
                            existingSurvey.DeliveryConfig.Schedule = survey.DeliveryConfig.Schedule;
                        }
                        else
                        {
                            _dbContext.Entry(existingSurvey.DeliveryConfig.Schedule).CurrentValues.SetValues(survey.DeliveryConfig.Schedule);
                        }
                    }
                    
                    // Actualizar Trigger
                    if (survey.DeliveryConfig.Trigger != null)
                    {
                        if (existingSurvey.DeliveryConfig.Trigger == null)
                        {
                            existingSurvey.DeliveryConfig.Trigger = survey.DeliveryConfig.Trigger;
                        }
                        else
                        {
                            _dbContext.Entry(existingSurvey.DeliveryConfig.Trigger).CurrentValues.SetValues(survey.DeliveryConfig.Trigger);
                        }
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
    }
}
