
using MediatR;
using SurveyApp.Domain.Models;
using System.Collections.Generic;

namespace SurveyApp.Application.Customers.Queries.GetAllCustomers
{
    public class GetAllCustomersQuery : IRequest<IEnumerable<Customer>>
    {
    }
}
