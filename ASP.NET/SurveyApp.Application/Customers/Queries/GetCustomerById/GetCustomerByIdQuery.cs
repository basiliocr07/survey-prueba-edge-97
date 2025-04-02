
using MediatR;
using SurveyApp.Domain.Models;

namespace SurveyApp.Application.Customers.Queries.GetCustomerById
{
    public class GetCustomerByIdQuery : IRequest<Customer>
    {
        public int Id { get; set; }
    }
}
