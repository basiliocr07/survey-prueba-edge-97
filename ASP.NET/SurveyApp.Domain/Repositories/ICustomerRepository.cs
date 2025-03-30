
using SurveyApp.Domain.Models;

namespace SurveyApp.Domain.Repositories
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetAllCustomersAsync();
        Task<IEnumerable<Service>> GetAllServicesAsync();
        Task<string> AddCustomerAsync(Customer customer);
        Task AddCustomerServiceAsync(string customerId, string serviceId);
        Task<string> GetServiceIdByNameAsync(string serviceName);
        Task<IEnumerable<string>> GetCustomerEmailsAsync();
    }
}
