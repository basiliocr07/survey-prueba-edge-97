
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
        // Este mensaje parece ser informativo pero podría no estar siendo utilizado
        // en la capa de presentación
        public string Message { get; set; }
    }
}
