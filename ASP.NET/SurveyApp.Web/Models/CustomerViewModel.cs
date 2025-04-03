
using System.Collections.Generic;
using SurveyApp.Domain.Models;

namespace SurveyApp.Web.Models
{
    public class CustomersViewModel
    {
        public List<Customer> Customers { get; set; } = new List<Customer>();
        public List<Service> Services { get; set; } = new List<Service>();
        public List<ServiceUsageData> ServiceUsageData { get; set; } = new List<ServiceUsageData>();
        public string TimeRange { get; set; } = "3";
        public string ChartType { get; set; } = "services";
        public string SearchTerm { get; set; } = "";
    }

    // Modelo para representar un cliente individual (eliminamos duplicidad)
    public class CustomerViewModel
    {
        public int Id { get; set; }
        public string BrandName { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string CustomerType { get; set; }
        public List<string> Services { get; set; } = new List<string>();
        
        // Añadimos las propiedades faltantes
        public List<Customer> Customers { get; set; } = new List<Customer>();
        public string TimeRange { get; set; } = "3";
        public string ChartType { get; set; } = "services";
        public List<ServiceUsageData> ServiceUsageData { get; set; } = new List<ServiceUsageData>();
    }
}
