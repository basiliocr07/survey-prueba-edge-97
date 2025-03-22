
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

    public class QuestionResponseViewModel
    {
        public string QuestionId { get; set; }
        public string QuestionTitle { get; set; }
        public string QuestionType { get; set; }
        public string Value { get; set; }
        public bool IsValid { get; set; }
    }

    public class SurveySubmissionViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        public string Phone { get; set; }
        public string Company { get; set; }

        public Dictionary<string, string> Answers { get; set; } = new Dictionary<string, string>();
    }

    public class SurveyQuestionViewModel
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Question { get; set; }
        public List<string> Options { get; set; } = new List<string>();
        public bool Required { get; set; }
    }

    public class SurveyDetailViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public List<SurveyQuestionViewModel> Questions { get; set; } = new List<SurveyQuestionViewModel>();
    }
}
