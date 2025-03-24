
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

    public class SurveyQuestionViewModel
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Question { get; set; }
        public List<string> Options { get; set; } = new List<string>();
        public bool Required { get; set; }
    }
}
