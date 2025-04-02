
using MediatR;
using SurveyApp.Domain.Models;
using SurveyApp.Domain.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace SurveyApp.Application.Customers.Queries.GetCustomerById
{
    public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerDetail>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ISurveyResponseRepository _surveyResponseRepository;

        public GetCustomerByIdQueryHandler(
            ICustomerRepository customerRepository,
            ISurveyResponseRepository surveyResponseRepository)
        {
            _customerRepository = customerRepository;
            _surveyResponseRepository = surveyResponseRepository;
        }

        public async Task<CustomerDetail> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.GetCustomerByIdAsync(request.Id);
            
            if (customer == null)
            {
                return null;
            }

            var result = new CustomerDetail 
            { 
                Customer = customer,
                SurveyResponses = new List<SurveyResponse>() 
            };

            // Only load survey responses if specifically requested
            if (request.IncludeSurveyResponses)
            {
                // This would need to be implemented in the repository
                // var responses = await _surveyResponseRepository.GetByCustomerIdAsync(request.Id);
                // result.SurveyResponses = responses.ToList();
            }

            return result;
        }
    }
}
