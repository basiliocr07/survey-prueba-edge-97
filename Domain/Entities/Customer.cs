
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
        public string ContactName { get; private set; }
        public List<string> AcquiredServices { get; private set; } = new List<string>();
        public DateTime CreatedAt { get; private set; }
        public List<GrowthMetric> GrowthMetrics { get; private set; } = new List<GrowthMetric>();

        public Customer(string brandName, string contactEmail, string contactPhone)
        {
            Id = Guid.NewGuid();
            BrandName = brandName;
            ContactEmail = contactEmail;
            ContactPhone = contactPhone;
            CreatedAt = DateTime.UtcNow;
        }

        public Customer(string brandName, string contactEmail, string contactPhone, string contactName) 
            : this(brandName, contactEmail, contactPhone)
        {
            ContactName = contactName;
        }

        public void UpdateBrandName(string brandName)
        {
            BrandName = brandName;
        }

        public void UpdateContactEmail(string contactEmail)
        {
            ContactEmail = contactEmail;
        }

        public void UpdateContactPhone(string contactPhone)
        {
            ContactPhone = contactPhone;
        }

        public void UpdateContactName(string contactName)
        {
            ContactName = contactName;
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
