
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SurveyApp.Application.Ports;
using SurveyApp.Domain.Entities;
using SurveyApp.Infrastructure.Data;

namespace SurveyApp.Infrastructure.Repositories
{
    public class AnalyticsRepository : IAnalyticsRepository
    {
        private readonly AppDbContext _dbContext;

        public AnalyticsRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<AnalyticsData> GetAnalyticsDataAsync()
        {
            var analyticsData = await _dbContext.AnalyticsData
                .Include(a => a.ResponseTrends)
                .FirstOrDefaultAsync();

            return analyticsData ?? new AnalyticsData();
        }

        public async Task UpdateAnalyticsDataAsync(AnalyticsData analyticsData)
        {
            var existingData = await _dbContext.AnalyticsData.FirstOrDefaultAsync();

            if (existingData != null)
            {
                _dbContext.Entry(existingData).CurrentValues.SetValues(analyticsData);

                // Update ResponseTrends
                var existingTrends = await _dbContext.ResponseTrends
                    .Where(rt => EF.Property<Guid>(rt, "AnalyticsDataId") == existingData.Id)
                    .ToListAsync();
                
                _dbContext.ResponseTrends.RemoveRange(existingTrends);
                
                foreach (var trend in analyticsData.ResponseTrends)
                {
                    existingData.AddResponseTrend(trend);
                }
            }
            else
            {
                await _dbContext.AnalyticsData.AddAsync(analyticsData);
            }

            await _dbContext.SaveChangesAsync();
        }
    }
}
