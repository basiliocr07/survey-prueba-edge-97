
using MediatR;
using SurveyApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SurveyApp.Application.Customers.Queries.GetCustomerGrowthData
{
    public class GetCustomerGrowthDataQueryHandler : IRequestHandler<GetCustomerGrowthDataQuery, CustomerGrowthDataViewModel>
    {
        private readonly ICustomerRepository _customerRepository;

        public GetCustomerGrowthDataQueryHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<CustomerGrowthDataViewModel> Handle(GetCustomerGrowthDataQuery request, CancellationToken cancellationToken)
        {
            // Obtener datos necesarios de los repositorios
            var customers = await _customerRepository.GetAllCustomersAsync();
            var services = await _customerRepository.GetAllServicesAsync();
            
            // Aplicar filtrado opcional si se especifica un tipo de cliente
            if (!string.IsNullOrEmpty(request.CustomerType))
            {
                customers = customers.Where(c => c.CustomerType.Equals(request.CustomerType, StringComparison.OrdinalIgnoreCase));
            }
            
            // Filtrar por período de tiempo si está especificado
            if (request.TimeRangeInMonths.HasValue)
            {
                var cutoffDate = DateTime.UtcNow.AddMonths(-request.TimeRangeInMonths.Value);
                customers = customers.Where(c => c.CreatedAt >= cutoffDate);
            }
            
            // Procesar y calcular los datos
            var serviceUsageData = CalculateServiceUsage(customers);
            var monthlyGrowthData = CalculateMonthlyGrowth(customers, request.TimeRangeInMonths ?? 12);
            var brandGrowthData = CalculateBrandGrowth(customers, request.TimeRangeInMonths);

            // Construir el ViewModel para la vista
            return new CustomerGrowthDataViewModel
            {
                Customers = customers,
                Services = services,
                ServiceUsageData = serviceUsageData,
                MonthlyGrowthData = monthlyGrowthData,
                BrandGrowthData = brandGrowthData
            };
        }

        private IEnumerable<Domain.Models.ServiceUsageData> CalculateServiceUsage(IEnumerable<Domain.Models.Customer> customers)
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

            return serviceUsage.Select(item => new Domain.Models.ServiceUsageData
            {
                Name = item.Key,
                Count = item.Value
            }).OrderByDescending(s => s.Count).ToList();
        }
        
        private IEnumerable<MonthlyGrowthData> CalculateMonthlyGrowth(IEnumerable<Domain.Models.Customer> customers, int months)
        {
            var result = new List<MonthlyGrowthData>();
            var endDate = DateTime.UtcNow;
            
            for (int i = 0; i < months; i++)
            {
                var startDate = endDate.AddMonths(-1);
                var customersInMonth = customers.Count(c => c.CreatedAt >= startDate && c.CreatedAt < endDate);
                
                result.Add(new MonthlyGrowthData
                {
                    Month = startDate.ToString("MMM yyyy"),
                    NewCustomers = customersInMonth
                });
                
                endDate = startDate;
            }
            
            return result.OrderBy(m => DateTime.Parse(m.Month)).ToList();
        }
        
        private IEnumerable<BrandGrowthData> CalculateBrandGrowth(IEnumerable<Domain.Models.Customer> customers, int? months)
        {
            var result = new List<BrandGrowthData>();
            var brandGroups = customers.GroupBy(c => c.BrandName);
            
            foreach (var group in brandGroups)
            {
                var total = group.Count();
                var recent = total;
                
                if (months.HasValue)
                {
                    var cutoffDate = DateTime.UtcNow.AddMonths(-months.Value);
                    recent = group.Count(c => c.CreatedAt >= cutoffDate);
                }
                
                result.Add(new BrandGrowthData
                {
                    BrandName = group.Key,
                    TotalCustomers = total,
                    RecentCustomers = recent
                });
            }
            
            return result.OrderByDescending(b => b.TotalCustomers).ToList();
        }
    }
}
