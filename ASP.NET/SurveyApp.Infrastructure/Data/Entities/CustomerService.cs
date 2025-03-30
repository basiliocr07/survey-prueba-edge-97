
using System;

namespace SurveyApp.Infrastructure.Data.Entities
{
    public class CustomerService
    {
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;
        
        public int ServiceId { get; set; }
        public Service Service { get; set; } = null!;
        
        public DateTime AssignedAt { get; set; }
    }
}
