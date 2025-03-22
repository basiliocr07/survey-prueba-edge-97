
using Microsoft.EntityFrameworkCore;
using SurveyApp.Domain.Models;
using SurveyApp.Domain.Repositories;
using SurveyApp.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyApp.Infrastructure.Repositories
{
    public class SurveyResponseRepository : ISurveyResponseRepository
    {
        private readonly AppDbContext _context;

        public SurveyResponseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SurveyResponse>> GetAllAsync()
        {
            return await _context.SurveyResponses
                .Include(r => r.Answers)
                .ToListAsync();
        }

        public async Task<SurveyResponse?> GetByIdAsync(int id)
        {
            return await _context.SurveyResponses
                .Include(r => r.Answers)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<SurveyResponse>> GetBySurveyIdAsync(int surveyId)
        {
            return await _context.SurveyResponses
                .Include(r => r.Answers)
                .Where(r => r.SurveyId == surveyId)
                .ToListAsync();
        }

        public async Task<bool> AddAsync(SurveyResponse surveyResponse)
        {
            _context.SurveyResponses.Add(surveyResponse);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(SurveyResponse surveyResponse)
        {
            _context.SurveyResponses.Update(surveyResponse);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var surveyResponse = await _context.SurveyResponses.FindAsync(id);
            if (surveyResponse == null)
                return false;
                
            _context.SurveyResponses.Remove(surveyResponse);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
