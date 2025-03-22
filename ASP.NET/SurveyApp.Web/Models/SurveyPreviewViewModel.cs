
using System;
using System.Collections.Generic;

namespace SurveyApp.Web.Models
{
    public class SurveyPreviewViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<QuestionViewModel> Questions { get; set; } = new List<QuestionViewModel>();
    }
}
