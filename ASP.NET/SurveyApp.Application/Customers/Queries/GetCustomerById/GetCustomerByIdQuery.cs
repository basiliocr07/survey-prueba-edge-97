
using MediatR;
using SurveyApp.Domain.Models;

namespace SurveyApp.Application.Customers.Queries.GetCustomerById
{
    public class GetCustomerByIdQuery : IRequest<CustomerDetail>
    {
        public int Id { get; set; }
        
        // Esta propiedad no parece estar siendo utilizada en los handlers
        // Mantenerla comentada hasta que se confirme su uso
        public bool IncludeSurveyResponses { get; set; } = false;
    }
    
    public class CustomerDetail
    {
        public Customer Customer { get; set; }
        public List<SurveyResponse> SurveyResponses { get; set; } = new List<SurveyResponse>();
    }
}
