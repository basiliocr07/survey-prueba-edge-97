
using System;
using System.Collections.Generic;

namespace SurveyApp.Infrastructure.Data.Entities
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }

        // Relación con CustomerService
        public ICollection<CustomerService> CustomerServices { get; set; } = new List<CustomerService>();
    }
}
