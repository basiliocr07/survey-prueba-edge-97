
using System;
using System.Collections.Generic;

namespace SurveyApp.Application.DTOs
{
    public class CustomerDto
    {
        public Guid Id { get; set; }
        public string BrandName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string ContactName { get; set; }
        public List<string> AcquiredServices { get; set; } = new List<string>();
        public DateTime CreatedAt { get; set; }
        public List<GrowthMetricDto> GrowthMetrics { get; set; } = new List<GrowthMetricDto>();
    }

    public class GrowthMetricDto
    {
        public string Period { get; set; }
        public decimal Revenue { get; set; }
        public int UserCount { get; set; }
    }

    public class CreateCustomerDto
    {
        public string BrandName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string ContactName { get; set; }
        public List<string> AcquiredServices { get; set; } = new List<string>();
    }
}
