
using Microsoft.EntityFrameworkCore;
using SurveyApp.Domain.Models;
using SurveyApp.Domain.Repositories;
using SurveyApp.Infrastructure.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SurveyApp.Infrastructure.Repositories
{
    public class SurveyRepository : ISurveyRepository
    {
        private readonly AppDbContext _context;

        public SurveyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Survey>> GetAllAsync()
        {
            return await _context.Surveys.ToListAsync();
        }

        public async Task<Survey?> GetByIdAsync(int id)
        {
            return await _context.Surveys.FindAsync(id);
        }

        public async Task<bool> AddAsync(Survey survey)
        {
            _context.Surveys.Add(survey);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(Survey survey)
        {
            _context.Surveys.Update(survey);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var survey = await _context.Surveys.FindAsync(id);
            if (survey == null)
                return false;
                
            _context.Surveys.Remove(survey);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
