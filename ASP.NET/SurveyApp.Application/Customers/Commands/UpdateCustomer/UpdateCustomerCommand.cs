
using MediatR;
using System.Collections.Generic;

namespace SurveyApp.Application.Customers.Commands.UpdateCustomer
{
    public class UpdateCustomerCommand : IRequest<UpdateCustomerResult>
    {
        public int Id { get; set; }
        public string BrandName { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string CustomerType { get; set; } = "client";
        public List<string> AcquiredServices { get; set; } = new List<string>();
    }

    public class UpdateCustomerResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
