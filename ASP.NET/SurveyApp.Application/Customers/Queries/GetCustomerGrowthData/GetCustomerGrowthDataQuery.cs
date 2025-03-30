
using MediatR;
using SurveyApp.Domain.Models;

namespace SurveyApp.Application.Customers.Queries.GetCustomerGrowthData
{
    public class GetCustomerGrowthDataQuery : IRequest<CustomerGrowthDataViewModel>
    {
    }

    public class CustomerGrowthDataViewModel
    {
        public IEnumerable<Customer> Customers { get; set; } = new List<Customer>();
        public IEnumerable<Service> Services { get; set; } = new List<Service>();
        public IEnumerable<ServiceUsageData> ServiceUsageData { get; set; } = new List<ServiceUsageData>();
    }
}
