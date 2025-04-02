
using SurveyApp.Domain.Models;
using SurveyApp.Application.Customers.Queries.GetCustomerGrowthData;

namespace SurveyApp.Web.Models
{
    public class CustomerGrowthViewModel
    {
        public IEnumerable<Customer> Customers { get; set; } = new List<Customer>();
        public IEnumerable<Service> Services { get; set; } = new List<Service>();
        public IEnumerable<ServiceUsageData> ServiceUsageData { get; set; } = new List<ServiceUsageData>();
        public IEnumerable<MonthlyGrowthData> MonthlyGrowthData { get; set; } = new List<MonthlyGrowthData>();
        public IEnumerable<BrandGrowthData> BrandGrowthData { get; set; } = new List<BrandGrowthData>();
        public CustomerFormViewModel CustomerForm { get; set; } = new CustomerFormViewModel();
        public string? SelectedTimeRange { get; set; }
        public string? SelectedChartType { get; set; }
    }

    public class CustomerFormViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string? ContactPhone { get; set; }
        public List<string> SelectedServices { get; set; } = new List<string>();
        public string? CustomerType { get; set; } = "client";
    }
}
