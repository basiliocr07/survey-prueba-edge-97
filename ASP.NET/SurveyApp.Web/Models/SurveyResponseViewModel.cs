
using System;
using System.Collections.Generic;

namespace SurveyApp.Web.Models
{
    public class SurveyResponseViewModel
    {
        public int Id { get; set; }
        public int SurveyId { get; set; }
        public string SurveyTitle { get; set; }
        public string RespondentName { get; set; }
        public string RespondentEmail { get; set; }
        public string RespondentPhone { get; set; }
        public string RespondentCompany { get; set; }
        public DateTime SubmittedAt { get; set; }
        public List<QuestionResponseViewModel> Answers { get; set; } = new List<QuestionResponseViewModel>();
        public bool IsExistingClient { get; set; }
        public string ExistingClientId { get; set; }
        public int CompletionTime { get; set; }
    }
}
