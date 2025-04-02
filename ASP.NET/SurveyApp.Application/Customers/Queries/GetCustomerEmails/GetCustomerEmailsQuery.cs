
using MediatR;
using System.Collections.Generic;

namespace SurveyApp.Application.Customers.Queries.GetCustomerEmails
{
    public class GetCustomerEmailsQuery : IRequest<List<string>>
    {
        public string CustomerType { get; set; }
    }
}
