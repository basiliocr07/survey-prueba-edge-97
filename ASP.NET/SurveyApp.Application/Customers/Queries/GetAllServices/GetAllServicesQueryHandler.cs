
using MediatR;
using SurveyApp.Domain.Models;
using SurveyApp.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SurveyApp.Application.Customers.Queries.GetAllServices
{
    public class GetAllServicesQueryHandler : IRequestHandler<GetAllServicesQuery, List<Service>>
    {
        private readonly ICustomerRepository _customerRepository;

        public GetAllServicesQueryHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<List<Service>> Handle(GetAllServicesQuery request, CancellationToken cancellationToken)
        {
            var services = await _customerRepository.GetAllServicesAsync();
            return services.ToList();
        }
    }
}
