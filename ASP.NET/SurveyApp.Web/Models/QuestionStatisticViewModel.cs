
using System.Collections.Generic;

namespace SurveyApp.Web.Models
{
    // Esta clase contiene las estadísticas de una pregunta individual
    public class QuestionStatisticViewModel
    {
        public string QuestionId { get; set; }
        public string QuestionTitle { get; set; }
        public string QuestionText { get; set; }
        public List<ResponseViewModel> Responses { get; set; } = new List<ResponseViewModel>();
        public Dictionary<string, ResponseDistributionViewModel> ResponseDistribution { get; set; } = new Dictionary<string, ResponseDistributionViewModel>();
    }

    // Modelo para representar una respuesta y su distribución estadística
    public class ResponseViewModel
    {
        public string Answer { get; set; }
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    // Modelo para representar la distribución de respuestas
    public class ResponseDistributionViewModel
    {
        public int Count { get; set; }
        public double Percentage { get; set; }
    }
}
