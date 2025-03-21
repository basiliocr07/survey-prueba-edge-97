
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SurveyApp.Application.DTOs;

namespace SurveyApp.Application.Services
{
    public interface ICustomerService
    {
        Task<CustomerDto> GetCustomerByIdAsync(Guid id);
        Task<List<CustomerDto>> GetAllCustomersAsync();
        Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto createCustomerDto);
        Task UpdateCustomerAsync(Guid id, CreateCustomerDto updateCustomerDto);
        Task DeleteCustomerAsync(Guid id);
        Task<Dictionary<string, int>> GetServiceUsageAnalyticsAsync();
        Task<List<CustomerDto>> GetCustomerByEmailAsync(string email);
    }
}
