
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SurveyApp.Application.DTOs;
using SurveyApp.Application.Ports;
using SurveyApp.Domain.Entities;

namespace SurveyApp.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<CustomerDto> GetCustomerByIdAsync(Guid id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
            {
                throw new KeyNotFoundException($"Customer with ID {id} not found.");
            }
            return MapToDto(customer);
        }

        public async Task<List<CustomerDto>> GetAllCustomersAsync()
        {
            var customers = await _customerRepository.GetAllAsync();
            return customers.Select(MapToDto).ToList();
        }

        public async Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto createCustomerDto)
        {
            var customer = new Customer(
                createCustomerDto.BrandName,
                createCustomerDto.ContactEmail,
                createCustomerDto.ContactPhone,
                createCustomerDto.ContactName
            );

            foreach (var service in createCustomerDto.AcquiredServices)
            {
                customer.AddService(service);
            }

            var createdCustomer = await _customerRepository.CreateAsync(customer);
            return MapToDto(createdCustomer);
        }

        public async Task UpdateCustomerAsync(Guid id, CreateCustomerDto updateCustomerDto)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
            {
                throw new KeyNotFoundException($"Customer with ID {id} not found.");
            }

            customer.UpdateBrandName(updateCustomerDto.BrandName);
            customer.UpdateContactEmail(updateCustomerDto.ContactEmail);
            customer.UpdateContactPhone(updateCustomerDto.ContactPhone);
            customer.UpdateContactName(updateCustomerDto.ContactName);

            // Update services
            var currentServices = new List<string>(customer.AcquiredServices);
            foreach (var service in currentServices)
            {
                if (!updateCustomerDto.AcquiredServices.Contains(service))
                {
                    customer.RemoveService(service);
                }
            }

            foreach (var service in updateCustomerDto.AcquiredServices)
            {
                if (!customer.AcquiredServices.Contains(service))
                {
                    customer.AddService(service);
                }
            }

            await _customerRepository.UpdateAsync(customer);
        }

        public async Task DeleteCustomerAsync(Guid id)
        {
            if (!await _customerRepository.ExistsAsync(id))
            {
                throw new KeyNotFoundException($"Customer with ID {id} not found.");
            }

            await _customerRepository.DeleteAsync(id);
        }

        public async Task<Dictionary<string, int>> GetServiceUsageAnalyticsAsync()
        {
            var customers = await _customerRepository.GetAllAsync();
            var serviceUsage = new Dictionary<string, int>();

            foreach (var customer in customers)
            {
                foreach (var service in customer.AcquiredServices)
                {
                    if (serviceUsage.ContainsKey(service))
                    {
                        serviceUsage[service]++;
                    }
                    else
                    {
                        serviceUsage[service] = 1;
                    }
                }
            }

            return serviceUsage;
        }

        private CustomerDto MapToDto(Customer customer)
        {
            return new CustomerDto
            {
                Id = customer.Id,
                BrandName = customer.BrandName,
                ContactEmail = customer.ContactEmail,
                ContactPhone = customer.ContactPhone,
                ContactName = customer.ContactName,
                AcquiredServices = customer.AcquiredServices,
                CreatedAt = customer.CreatedAt,
                GrowthMetrics = customer.GrowthMetrics.Select(m => new GrowthMetricDto
                {
                    Period = m.Period,
                    Revenue = m.Revenue,
                    UserCount = m.UserCount
                }).ToList()
            };
        }
    }
}
