
using MediatR;
using System;
using System.Collections.Generic;

namespace SurveyApp.Application.Customers.Commands.AddCustomer
{
    public class AddCustomerCommand : IRequest<AddCustomerResult>
    {
        public string BrandName { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string? ContactPhone { get; set; }
        public List<string> AcquiredServices { get; set; } = new List<string>();
        public string CustomerType { get; set; } = "client";
    }

    public class AddCustomerResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string CustomerId { get; set; } = string.Empty;
    }
}
