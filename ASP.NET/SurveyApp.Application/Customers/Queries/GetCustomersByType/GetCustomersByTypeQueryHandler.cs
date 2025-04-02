
using MediatR;
using SurveyApp.Domain.Models;
using SurveyApp.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SurveyApp.Application.Customers.Queries.GetCustomersByType
{
    public class GetCustomersByTypeQueryHandler : IRequestHandler<GetCustomersByTypeQuery, List<Customer>>
    {
        private readonly ICustomerRepository _customerRepository;

        public GetCustomersByTypeQueryHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<List<Customer>> Handle(GetCustomersByTypeQuery request, CancellationToken cancellationToken)
        {
            var customers = await _customerRepository.GetCustomersByTypeAsync(request.CustomerType);
            return customers.ToList();
        }
    }
}
