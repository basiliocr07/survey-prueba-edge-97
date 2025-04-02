
using SurveyApp.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SurveyApp.Application.Interfaces
{
    public interface ISurveyService
    {
        Task<IEnumerable<Survey>> GetAllSurveysAsync();
        Task<Survey?> GetSurveyByIdAsync(int id);
        Task<bool> CreateSurveyAsync(Survey survey);
        Task<bool> UpdateSurveyAsync(Survey survey);
        Task<bool> DeleteSurveyAsync(int id);
        Task<IEnumerable<Survey>> GetSurveysByStatusAsync(string status);
        Task<bool> SendSurveyEmailsAsync(int surveyId, List<string> emailAddresses);
        Task<SurveyStatistics> GetSurveyStatisticsAsync(int surveyId);
        
        // Nuevos métodos
        Task<bool> UpdateSurveyDeliveryConfigAsync(int surveyId, DeliveryConfiguration deliveryConfig);
        Task<IEnumerable<Survey>> GetSurveysForCustomerAsync(int customerId);
        Task<bool> AssignSurveyToCustomersAsync(int surveyId, List<int> customerIds);
    }
}
