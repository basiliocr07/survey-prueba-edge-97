
using MediatR;
using SurveyApp.Domain.Models;
using SurveyApp.Domain.Repositories;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SurveyApp.Application.Customers.Queries.GetAllCustomers
{
    public class GetAllCustomersQueryHandler : IRequestHandler<GetAllCustomersQuery, IEnumerable<Customer>>
    {
        private readonly ICustomerRepository _customerRepository;

        public GetAllCustomersQueryHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<IEnumerable<Customer>> Handle(GetAllCustomersQuery request, CancellationToken cancellationToken)
        {
            return await _customerRepository.GetAllCustomersAsync();
        }
    }
}
