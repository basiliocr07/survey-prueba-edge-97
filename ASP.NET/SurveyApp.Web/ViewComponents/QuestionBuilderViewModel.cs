
using SurveyApp.Web.Models;

namespace SurveyApp.Web.ViewComponents
{
    public class QuestionBuilderViewModel
    {
        public SurveyQuestionViewModel Question { get; set; }
        public int Index { get; set; }
        public int Total { get; set; }
        public bool IsFirst { get; set; }
        public bool IsLast { get; set; }
    }
}
