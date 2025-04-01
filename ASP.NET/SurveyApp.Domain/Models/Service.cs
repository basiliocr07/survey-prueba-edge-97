
namespace SurveyApp.Domain.Models
{
    public class Service
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
    
    public class ServiceUsageData
    {
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
