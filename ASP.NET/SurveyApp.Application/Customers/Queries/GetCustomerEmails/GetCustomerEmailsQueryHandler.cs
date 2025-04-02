
using MediatR;
using SurveyApp.Domain.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SurveyApp.Application.Customers.Queries.GetCustomerEmails
{
    public class GetCustomerEmailsQueryHandler : IRequestHandler<GetCustomerEmailsQuery, List<string>>
    {
        private readonly ICustomerRepository _customerRepository;

        public GetCustomerEmailsQueryHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<List<string>> Handle(GetCustomerEmailsQuery request, CancellationToken cancellationToken)
        {
            var emails = await _customerRepository.GetCustomerEmailsAsync(request.CustomerType);
            return emails.ToList();
        }
    }
}
