
using System;
using System.Collections.Generic;

namespace SurveyApp.Domain.Entities
{
    public class Customer
    {
        public Guid Id { get; private set; }
        public string BrandName { get; private set; }
        public string ContactEmail { get; private set; }
        public string ContactPhone { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public List<string> AcquiredServices { get; private set; } = new List<string>();
        public List<GrowthMetric> GrowthMetrics { get; private set; } = new List<GrowthMetric>();

        // For EF Core
        private Customer() { }

        public Customer(string brandName, string contactEmail, string contactPhone = null)
        {
            Id = Guid.NewGuid();
            BrandName = brandName;
            ContactEmail = contactEmail;
            ContactPhone = contactPhone;
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateBrandName(string brandName)
        {
            BrandName = brandName;
        }

        public void UpdateContactInfo(string email, string phone = null)
        {
            ContactEmail = email;
            if (phone != null)
            {
                ContactPhone = phone;
            }
        }

        public void AddService(string service)
        {
            if (!AcquiredServices.Contains(service))
            {
                AcquiredServices.Add(service);
            }
        }

        public void RemoveService(string service)
        {
            AcquiredServices.Remove(service);
        }

        public void AddGrowthMetric(GrowthMetric metric)
        {
            GrowthMetrics.Add(metric);
        }
    }
}
