
using MediatR;
using System.Collections.Generic;
using SurveyApp.Domain.Models;

namespace SurveyApp.Application.Customers.Queries.GetServiceUsageData
{
    public class GetServiceUsageDataQuery : IRequest<List<ServiceUsageData>>
    {
        public string TimeRange { get; set; } = "3";
        public string CustomerType { get; set; }
    }
}
