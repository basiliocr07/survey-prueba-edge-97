
using System;
using System.Collections.Generic;

namespace SurveyApp.Infrastructure.Data.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string? ContactPhone { get; set; }
        public string CustomerType { get; set; } = "client"; // Valor predeterminado
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        // Relación con CustomerService
        public ICollection<CustomerService> CustomerServices { get; set; } = new List<CustomerService>();
    }
}
