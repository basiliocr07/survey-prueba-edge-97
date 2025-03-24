
namespace SurveyApp.Web.Models
{
    public class QuestionResponseViewModel
    {
        public string QuestionId { get; set; }
        public string QuestionTitle { get; set; }
        public string QuestionType { get; set; }
        public string Value { get; set; }
        public bool IsValid { get; set; }
    }
}
