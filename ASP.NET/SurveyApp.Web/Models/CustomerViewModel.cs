
using System.Collections.Generic;
using SurveyApp.Domain.Models;

namespace SurveyApp.Web.Models
{
    public class CustomerViewModel
    {
        public List<Customer> Customers { get; set; } = new List<Customer>();
        public List<Service> Services { get; set; } = new List<Service>();
        public List<ServiceUsageData> ServiceUsageData { get; set; } = new List<ServiceUsageData>();
        public string TimeRange { get; set; } = "3";
        public string ChartType { get; set; } = "services";
        public string SearchTerm { get; set; } = "";
    }
}
