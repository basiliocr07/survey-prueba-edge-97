
using MediatR;
using SurveyApp.Domain.Repositories;

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
            var customers = await _customerRepository.GetAllCustomersAsync();
            var services = await _customerRepository.GetAllServicesAsync();
            var serviceUsageData = CalculateServiceUsage(customers);

            return new CustomerGrowthDataViewModel
            {
                Customers = customers,
                Services = services,
                ServiceUsageData = serviceUsageData
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
    }
}
