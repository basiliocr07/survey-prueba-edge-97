namespace SurveyApp.Domain.Entities
{
    // We've renamed SurveyResponseStats in AppDbContext.cs to use existing class
    public class SurveyResponseStats
    {
        public string Date { get; set; } = string.Empty;
        public int Responses { get; set; }
    }
}
