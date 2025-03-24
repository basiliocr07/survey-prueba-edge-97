
using System.Collections.Generic;

namespace SurveyApp.Web.Models
{
    public class SurveyQuestionViewModel
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Question { get; set; }
        public string Text { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> Options { get; set; } = new List<string>();
        public bool Required { get; set; }
    }
}
