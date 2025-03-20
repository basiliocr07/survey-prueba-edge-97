
using System.Collections.Generic;
using SurveyApp.Application.DTOs;

namespace SurveyApp.WebMvc.Models
{
    public class SuggestionViewModel
    {
        public List<SuggestionDto> Suggestions { get; set; }
        public List<KnowledgeBaseItemDto> KnowledgeBaseItems { get; set; }
        public string[] Categories { get; set; }
        public string ActiveTab { get; set; } = "submit";
    }

    public class SuggestionListViewModel
    {
        public List<SuggestionDto> Suggestions { get; set; }
        public string[] Categories { get; set; }
        public string StatusFilter { get; set; }
        public string CategoryFilter { get; set; }
        public string SearchTerm { get; set; }
    }

    public class SuggestionReportViewModel
    {
        public MonthlyReportDto Report { get; set; }
        public int MonthsRange { get; set; }
    }

    public class SuggestionIndexViewModel
    {
        public List<SuggestionDto> Suggestions { get; set; }
        public int TotalSuggestions { get; set; }
        public int NewSuggestions { get; set; }
        public int InProgressSuggestions { get; set; }
        public int CompletedSuggestions { get; set; }
        public int RejectedSuggestions { get; set; }
        public string[] Categories { get; set; }
        public Dictionary<string, int> CategoryCounts { get; set; }
        public Dictionary<string, int> MonthlyCounts { get; set; }
    }

    public class SuggestionReportsViewModel
    {
        public List<SuggestionDto> Suggestions { get; set; }
        public MonthlyReportDto MonthlyReport { get; set; }
        public int MonthsRange { get; set; }
        public Dictionary<string, int> CategoryDistribution { get; set; }
        public Dictionary<string, int> StatusDistribution { get; set; }
        public Dictionary<string, double> ImplementationRates { get; set; }
    }
}
