
using System;
using System.Collections.Generic;

namespace SurveyApp.WebMvc.Models
{
    public class SurveyViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<QuestionViewModel> Questions { get; set; } = new List<QuestionViewModel>();
    }
}
