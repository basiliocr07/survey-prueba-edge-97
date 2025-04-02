
using MediatR;
using SurveyApp.Domain.Models;

namespace SurveyApp.Application.Customers.Queries.GetCustomerById
{
    public class GetCustomerByIdQuery : IRequest<CustomerDetail>
    {
        public int Id { get; set; }
        
        public bool IncludeSurveyResponses { get; set; } = false;
    }
    
    public class CustomerDetail
    {
        public Customer Customer { get; set; }
        public List<SurveyResponse> SurveyResponses { get; set; } = new List<SurveyResponse>();
    }
}
