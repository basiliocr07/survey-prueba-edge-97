
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SurveyApp.Application.DTOs;

namespace SurveyApp.Application.Services
{
    // Reporting operations for suggestions
    public partial class SuggestionService
    {
        public async Task<MonthlyReportDto> GenerateMonthlyReportAsync(int months)
        {
            var endDate = DateTime.UtcNow;
            var startDate = endDate.AddMonths(-months);
            
            var allSuggestions = await _suggestionRepository.GetAllAsync();
            var filteredSuggestions = allSuggestions.Where(s => s.CreatedAt >= startDate && s.CreatedAt <= endDate).ToList();
            
            var implementedCount = filteredSuggestions.Count(s => s.Status == SuggestionStatus.Implemented);

            // Group by month
            var monthlyData = filteredSuggestions
                .GroupBy(s => new { Month = s.CreatedAt.Month, Year = s.CreatedAt.Year })
                .Select(g => new MonthlyDataDto
                {
                    Month = g.Key.Month.ToString(),
                    Year = g.Key.Year,
                    TotalSuggestions = g.Count(),
                    ImplementedSuggestions = g.Count(s => s.Status == SuggestionStatus.Implemented)
                })
                .OrderByDescending(m => m.Year)
                .ThenByDescending(m => int.Parse(m.Month))
                .ToList();

            // Get top categories
            var categoryGroups = filteredSuggestions
                .Where(s => !string.IsNullOrEmpty(s.Category))
                .GroupBy(s => s.Category)
                .Select(g => new CategoryCountDto { Category = g.Key, Count = g.Count() })
                .OrderByDescending(c => c.Count)
                .Take(5)
                .ToList();

            return new MonthlyReportDto
            {
                TotalSuggestions = filteredSuggestions.Count,
                ImplementedSuggestions = implementedCount,
                TopCategories = categoryGroups,
                MonthlyData = monthlyData,
                Suggestions = filteredSuggestions.Select(MapToDto).ToList()
            };
        }
    }
}
