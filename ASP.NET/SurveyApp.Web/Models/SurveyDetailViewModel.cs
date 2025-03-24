
using System.Collections.Generic;

namespace SurveyApp.Web.Models
{
    public class SurveyDetailViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<SurveyQuestionViewModel> Questions { get; set; } = new List<SurveyQuestionViewModel>();
    }
}
