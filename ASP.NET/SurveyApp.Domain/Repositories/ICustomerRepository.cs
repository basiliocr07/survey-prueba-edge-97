
using SurveyApp.Domain.Models;

namespace SurveyApp.Domain.Repositories
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetAllCustomersAsync();
        Task<IEnumerable<Customer>> GetCustomersByTypeAsync(string customerType);
        Task<IEnumerable<Service>> GetAllServicesAsync();
        Task<int> AddCustomerAsync(Customer customer);
        Task AddCustomerServiceAsync(int customerId, string serviceId);
        Task<string> GetServiceIdByNameAsync(string serviceName);
        Task<IEnumerable<string>> GetCustomerEmailsAsync();
        Task<Customer> GetCustomerByIdAsync(int id);
        Task UpdateCustomerAsync(Customer customer);
        Task DeleteCustomerAsync(int id);
    }
}
