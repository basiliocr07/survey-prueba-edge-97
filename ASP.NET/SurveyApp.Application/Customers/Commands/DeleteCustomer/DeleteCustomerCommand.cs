
using MediatR;

namespace SurveyApp.Application.Customers.Commands.DeleteCustomer
{
    public class DeleteCustomerCommand : IRequest<DeleteCustomerResult>
    {
        public int Id { get; set; }
    }
    
    public class DeleteCustomerResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
    }
}
