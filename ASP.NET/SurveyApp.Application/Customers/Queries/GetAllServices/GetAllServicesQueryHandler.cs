
using MediatR;
using SurveyApp.Domain.Models;
using SurveyApp.Domain.Repositories;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SurveyApp.Application.Customers.Queries.GetAllServices
{
    public class GetAllServicesQueryHandler : IRequestHandler<GetAllServicesQuery, IEnumerable<Service>>
    {
        private readonly ICustomerRepository _customerRepository;

        public GetAllServicesQueryHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<IEnumerable<Service>> Handle(GetAllServicesQuery request, CancellationToken cancellationToken)
        {
            return await _customerRepository.GetAllServicesAsync();
        }
    }
}
