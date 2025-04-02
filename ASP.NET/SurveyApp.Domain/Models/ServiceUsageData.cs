
// NOTA: Esta clase está duplicada en Service.cs y debería considerarse eliminar este archivo
// y usar solamente la definición en Service.cs para evitar conflictos
namespace SurveyApp.Domain.Models
{
    public class ServiceUsageData
    {
        public string Name { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
