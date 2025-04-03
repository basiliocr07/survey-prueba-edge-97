
using System.Collections.Generic;
using MediatR;

namespace SurveyApp.Application.Customers.Commands.AddCustomer
{
    public class AddCustomerCommand : IRequest<AddCustomerResult>
    {
        public string BrandName { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string CustomerType { get; set; } = "client";
        public List<string> AcquiredServices { get; set; } = new List<string>();
    }

    public class AddCustomerResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int CustomerId { get; set; }
    }
}
