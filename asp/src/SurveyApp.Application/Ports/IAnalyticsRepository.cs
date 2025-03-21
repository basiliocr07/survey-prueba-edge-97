
using System;
using System.Threading.Tasks;
using SurveyApp.Domain.Entities;

namespace SurveyApp.Application.Ports
{
    public interface IAnalyticsRepository
    {
        Task<AnalyticsData> GetAnalyticsDataAsync();
        Task UpdateAnalyticsDataAsync(AnalyticsData analyticsData);
    }
}
