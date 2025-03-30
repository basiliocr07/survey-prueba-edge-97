
namespace SurveyApp.Domain.Models
{
    public class Service
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string UserType { get; set; } = "client"; // "admin" o "client"
    }
}
