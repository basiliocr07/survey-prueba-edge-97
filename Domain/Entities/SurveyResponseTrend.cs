
// Este archivo se ha revisado por error de duplicación de espacio de nombres.
// Se utiliza el nombre de la clase 'SurveyResponseTrend' para tendencias de respuestas

namespace SurveyApp.Domain.Entities
{
    // Si hay un conflicto, renombramos esta clase para evitar duplicados
    public class SurveyResponseStats
    {
        public string Date { get; set; } = string.Empty;
        public int Responses { get; set; }
    }
}
