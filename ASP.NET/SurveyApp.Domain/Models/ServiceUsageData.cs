
namespace SurveyApp.Domain.Models
{
    public class ServiceUsageData
    {
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
        public string UserType { get; set; } = "client"; // "admin" o "client"
    }
}
