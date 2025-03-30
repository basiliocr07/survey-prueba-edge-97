
using SurveyApp.Domain.Models;

namespace SurveyApp.Web.Models
{
    public class CustomerGrowthViewModel
    {
        public IEnumerable<Customer> Customers { get; set; } = new List<Customer>();
        public IEnumerable<Service> Services { get; set; } = new List<Service>();
        public IEnumerable<ServiceUsageData> ServiceUsageData { get; set; } = new List<ServiceUsageData>();
        public CustomerFormViewModel CustomerForm { get; set; } = new CustomerFormViewModel();
        public bool IsAdmin { get; set; } = false;
        public string UserType { get; set; } = "client";
    }

    public class CustomerFormViewModel
    {
        public string BrandName { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string? ContactPhone { get; set; }
        public List<string> SelectedServices { get; set; } = new List<string>();
        public string UserType { get; set; } = "client"; // "admin" o "client"
    }
}
