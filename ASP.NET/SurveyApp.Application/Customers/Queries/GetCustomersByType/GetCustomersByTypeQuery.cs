
using MediatR;
using SurveyApp.Domain.Models;
using System.Collections.Generic;

namespace SurveyApp.Application.Customers.Queries.GetCustomersByType
{
    public class GetCustomersByTypeQuery : IRequest<List<Customer>>
    {
        public string CustomerType { get; set; }
    }
}
