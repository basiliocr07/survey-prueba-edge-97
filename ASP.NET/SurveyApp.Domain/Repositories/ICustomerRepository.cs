
using SurveyApp.Domain.Models;

namespace SurveyApp.Domain.Repositories
{
    public interface ICustomerRepository
    {
        // Consultas (Queries)
        Task<IEnumerable<Customer>> GetAllCustomersAsync();
        Task<IEnumerable<Customer>> GetCustomersByTypeAsync(string customerType);
        Task<Customer> GetCustomerByIdAsync(int id);
        Task<IEnumerable<Service>> GetAllServicesAsync();
        Task<string> GetServiceIdByNameAsync(string serviceName);
        Task<IEnumerable<string>> GetCustomerEmailsAsync(string customerType = null);
        
        // Comandos (Commands)
        Task<int> AddCustomerAsync(Customer customer);
        Task UpdateCustomerAsync(Customer customer);
        Task DeleteCustomerAsync(int id);
        Task AddCustomerServiceAsync(int customerId, string serviceId);
    }
}
