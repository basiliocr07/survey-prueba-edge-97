
using System;
using System.Collections.Generic;
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

        public async Task<IEnumerable<Survey>> GetAllAsync()
        {
            return await _dbContext.Surveys
                .Include(s => s.Questions)
                    .ThenInclude(q => q.Settings)
                .Include(s => s.DeliveryConfig)
                    .ThenInclude(dc => dc!.Schedule)
                .Include(s => s.DeliveryConfig)
                    .ThenInclude(dc => dc!.Trigger)
                .ToListAsync();
        }

        public async Task<Survey?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Surveys
                .Include(s => s.Questions)
                    .ThenInclude(q => q.Settings)
                .Include(s => s.DeliveryConfig)
                    .ThenInclude(dc => dc!.Schedule)
                .Include(s => s.DeliveryConfig)
                    .ThenInclude(dc => dc!.Trigger)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Survey> CreateAsync(Survey survey)
        {
            await _dbContext.Surveys.AddAsync(survey);
            await _dbContext.SaveChangesAsync();
            return survey;
        }

        public async Task UpdateAsync(Survey survey)
        {
            _dbContext.Surveys.Update(survey);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var survey = await _dbContext.Surveys.FindAsync(id);
            if (survey != null)
            {
                _dbContext.Surveys.Remove(survey);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
