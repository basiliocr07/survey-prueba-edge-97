
using System.Collections.Generic;

namespace SurveyApp.WebMvc.Models
{
    public class RequirementReportsViewModel
    {
        public int TotalRequirements { get; set; }
        public int ProposedRequirements { get; set; }
        public int InProgressRequirements { get; set; }
        public int ImplementedRequirements { get; set; }
        public int RejectedRequirements { get; set; }
        public Dictionary<string, int> CategoryDistribution { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> PriorityDistribution { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> ProjectAreaDistribution { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> MonthlyRequirements { get; set; } = new Dictionary<string, int>();
    }
}
