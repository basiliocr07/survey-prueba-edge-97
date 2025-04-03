
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SurveyApp.Domain.Models;
using SurveyApp.Domain.Repositories;

namespace SurveyApp.Application.Customers.Queries.GetServiceUsageData
{
    public class GetServiceUsageDataQueryHandler : IRequestHandler<GetServiceUsageDataQuery, List<ServiceUsageData>>
    {
        private readonly ICustomerRepository _customerRepository;

        public GetServiceUsageDataQueryHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<List<ServiceUsageData>> Handle(GetServiceUsageDataQuery request, CancellationToken cancellationToken)
        {
            // Obtener todos los clientes o filtrados por tipo
            IEnumerable<Customer> customers;
            if (!string.IsNullOrEmpty(request.CustomerType))
            {
                customers = await _customerRepository.GetCustomersByTypeAsync(request.CustomerType);
            }
            else
            {
                customers = await _customerRepository.GetAllCustomersAsync();
            }

            // Filtrar por período de tiempo si está especificado
            if (!string.IsNullOrEmpty(request.TimeRange) && request.TimeRange != "all")
            {
                int months = int.Parse(request.TimeRange);
                var cutoffDate = DateTime.UtcNow.AddMonths(-months);
                customers = customers.Where(c => c.CreatedAt >= cutoffDate);
            }

            // Calcular el uso de servicios
            return CalculateServiceUsage(customers).ToList();
        }

        private IEnumerable<ServiceUsageData> CalculateServiceUsage(IEnumerable<Customer> customers)
        {
            var serviceUsage = new Dictionary<string, int>();

            foreach (var customer in customers)
            {
                if (customer.AcquiredServices != null)
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
            }

            return serviceUsage.Select(item => new ServiceUsageData
            {
                Name = item.Key,
                Count = item.Value
            }).OrderByDescending(s => s.Count);
        }
    }
}
