
using System.Collections.Generic;

namespace SurveyApp.Web.Models
{
    // Esta clase se mantiene por compatibilidad con código existente,
    // pero sus propiedades ahora redirigen a QuestionStatViewModel
    public class QuestionStatisticViewModel
    {
        public string QuestionId { get; set; }
        public string QuestionTitle { get; set; }
        public string QuestionText { get; set; }
        public List<ResponseViewModel> Responses { get; set; } = new List<ResponseViewModel>();
        public Dictionary<string, ResponseDistributionViewModel> ResponseDistribution { get; set; } = new Dictionary<string, ResponseDistributionViewModel>();
    }

    // Estas clases se mantienen por compatibilidad con código existente
    public class ResponseViewModel
    {
        public string Answer { get; set; }
        public int Count { get; set; }
        public double Percentage { get; set; }
    }

    public class ResponseDistributionViewModel
    {
        public int Count { get; set; }
        public double Percentage { get; set; }
    }
}
