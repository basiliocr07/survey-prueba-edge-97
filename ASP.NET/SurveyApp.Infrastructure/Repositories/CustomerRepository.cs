
using Microsoft.EntityFrameworkCore;
using SurveyApp.Domain.Models;
using SurveyApp.Domain.Repositories;
using SurveyApp.Infrastructure.Data;

namespace SurveyApp.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;

        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Customer>> GetAllCustomersAsync()
        {
            var customers = await _context.Customers
                .Include(c => c.CustomerServices)
                .ThenInclude(cs => cs.Service)
                .ToListAsync();

            // Mapeo de entidades de infraestructura a entidades de dominio
            return customers.Select(c => new Customer
            {
                Id = c.Id.ToString(),
                BrandName = c.BrandName,
                ContactName = c.ContactName,
                ContactEmail = c.ContactEmail,
                ContactPhone = c.ContactPhone,
                AcquiredServices = c.CustomerServices.Select(cs => cs.Service.Name).ToList(),
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            });
        }

        public async Task<IEnumerable<Service>> GetAllServicesAsync()
        {
            var services = await _context.Services.ToListAsync();

            return services.Select(s => new Service
            {
                Id = s.Id.ToString(),
                Name = s.Name,
                Description = s.Description,
                CreatedAt = s.CreatedAt
            });
        }

        public async Task<string> AddCustomerAsync(Customer customer)
        {
            var customerEntity = new Data.Entities.Customer
            {
                BrandName = customer.BrandName,
                ContactName = customer.ContactName,
                ContactEmail = customer.ContactEmail,
                ContactPhone = customer.ContactPhone,
                CreatedAt = customer.CreatedAt,
                UpdatedAt = customer.UpdatedAt
            };

            _context.Customers.Add(customerEntity);
            await _context.SaveChangesAsync();

            return customerEntity.Id.ToString();
        }

        public async Task AddCustomerServiceAsync(string customerId, string serviceId)
        {
            if (int.TryParse(customerId, out int custId) && int.TryParse(serviceId, out int servId))
            {
                var customerServiceEntity = new Data.Entities.CustomerService
                {
                    CustomerId = custId,
                    ServiceId = servId,
                    AssignedAt = DateTime.UtcNow
                };

                _context.CustomerServices.Add(customerServiceEntity);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<string> GetServiceIdByNameAsync(string serviceName)
        {
            var service = await _context.Services.FirstOrDefaultAsync(s => s.Name == serviceName);
            return service?.Id.ToString() ?? string.Empty;
        }

        public async Task<IEnumerable<string>> GetCustomerEmailsAsync()
        {
            return await _context.Customers.Select(c => c.ContactEmail).ToListAsync();
        }
    }
}
