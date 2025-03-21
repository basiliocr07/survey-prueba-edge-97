
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
    public class SurveyResponseRepository : ISurveyResponseRepository
    {
        private readonly AppDbContext _dbContext;

        public SurveyResponseRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<SurveyResponse>> GetAllAsync()
        {
            return await _dbContext.SurveyResponses
                .Include(r => r.Answers)
                .ToListAsync();
        }

        public async Task<IEnumerable<SurveyResponse>> GetBySurveyIdAsync(Guid surveyId)
        {
            return await _dbContext.SurveyResponses
                .Where(r => r.SurveyId == surveyId)
                .Include(r => r.Answers)
                .ToListAsync();
        }

        public async Task<SurveyResponse?> GetByIdAsync(Guid id)
        {
            return await _dbContext.SurveyResponses
                .Include(r => r.Answers)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<SurveyResponse> CreateAsync(SurveyResponse response)
        {
            await _dbContext.SurveyResponses.AddAsync(response);
            await _dbContext.SaveChangesAsync();
            return response;
        }
    }
}
