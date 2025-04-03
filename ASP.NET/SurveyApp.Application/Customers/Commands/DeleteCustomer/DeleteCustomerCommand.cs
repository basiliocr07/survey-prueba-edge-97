
using MediatR;

namespace SurveyApp.Application.Customers.Commands.DeleteCustomer
{
    public class DeleteCustomerCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
