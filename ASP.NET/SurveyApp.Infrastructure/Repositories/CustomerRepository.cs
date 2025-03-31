
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
                CustomerType = c.CustomerType,
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
                CustomerType = customer.CustomerType,
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
        
        public async Task<Customer> GetCustomerByIdAsync(string id)
        {
            if (!int.TryParse(id, out int customerId))
            {
                return null;
            }
            
            var customerEntity = await _context.Customers
                .Include(c => c.CustomerServices)
                .ThenInclude(cs => cs.Service)
                .FirstOrDefaultAsync(c => c.Id == customerId);
                
            if (customerEntity == null)
            {
                return null;
            }
            
            return new Customer
            {
                Id = customerEntity.Id.ToString(),
                BrandName = customerEntity.BrandName,
                ContactName = customerEntity.ContactName,
                ContactEmail = customerEntity.ContactEmail,
                ContactPhone = customerEntity.ContactPhone,
                CustomerType = customerEntity.CustomerType,
                AcquiredServices = customerEntity.CustomerServices.Select(cs => cs.Service.Name).ToList(),
                CreatedAt = customerEntity.CreatedAt,
                UpdatedAt = customerEntity.UpdatedAt
            };
        }
        
        public async Task UpdateCustomerAsync(Customer customer)
        {
            if (!int.TryParse(customer.Id, out int customerId))
            {
                throw new ArgumentException("ID de cliente inválido");
            }
            
            var customerEntity = await _context.Customers.FindAsync(customerId);
            
            if (customerEntity == null)
            {
                throw new KeyNotFoundException($"Cliente con ID {customer.Id} no encontrado");
            }
            
            // Actualizar propiedades
            customerEntity.BrandName = customer.BrandName;
            customerEntity.ContactName = customer.ContactName;
            customerEntity.ContactEmail = customer.ContactEmail;
            customerEntity.ContactPhone = customer.ContactPhone;
            customerEntity.CustomerType = customer.CustomerType;
            customerEntity.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();
            
            // Actualizar servicios si es necesario
            if (customer.AcquiredServices != null && customer.AcquiredServices.Any())
            {
                // Eliminar asignaciones existentes
                var existingServices = await _context.CustomerServices
                    .Where(cs => cs.CustomerId == customerId)
                    .ToListAsync();
                
                _context.CustomerServices.RemoveRange(existingServices);
                
                // Añadir nuevas asignaciones
                foreach (var serviceName in customer.AcquiredServices)
                {
                    var serviceId = await GetServiceIdByNameAsync(serviceName);
                    if (!string.IsNullOrEmpty(serviceId) && int.TryParse(serviceId, out int servId))
                    {
                        var customerServiceEntity = new Data.Entities.CustomerService
                        {
                            CustomerId = customerId,
                            ServiceId = servId,
                            AssignedAt = DateTime.UtcNow
                        };
                        
                        _context.CustomerServices.Add(customerServiceEntity);
                    }
                }
                
                await _context.SaveChangesAsync();
            }
        }
        
        public async Task DeleteCustomerAsync(string id)
        {
            if (!int.TryParse(id, out int customerId))
            {
                throw new ArgumentException("ID de cliente inválido");
            }
            
            var customerEntity = await _context.Customers.FindAsync(customerId);
            
            if (customerEntity == null)
            {
                throw new KeyNotFoundException($"Cliente con ID {id} no encontrado");
            }
            
            _context.Customers.Remove(customerEntity);
            await _context.SaveChangesAsync();
        }
    }
}
