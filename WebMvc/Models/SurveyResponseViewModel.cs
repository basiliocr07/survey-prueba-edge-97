
using System;
using System.Collections.Generic;

namespace SurveyApp.WebMvc.Models
{
    public class SurveyResponseViewModel
    {
        public Guid SurveyId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<QuestionViewModel> Questions { get; set; }
        public string RespondentName { get; set; }
        public string RespondentEmail { get; set; }
    }

    public class QuestionViewModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public bool Required { get; set; }
        public List<string> Options { get; set; }
    }

    public class SurveyResponseInputModel
    {
        public string RespondentName { get; set; }
        public string RespondentEmail { get; set; }
        public Dictionary<string, object> Answers { get; set; } = new Dictionary<string, object>();
    }
}
