
using System.Collections.Generic;
using System;

namespace SurveyApp.Web.Models
{
    public class SurveyDetailViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public List<SurveyQuestionViewModel> Questions { get; set; } = new List<SurveyQuestionViewModel>();
        
        public static SurveyDetailViewModel FromDomainModel(SurveyApp.Domain.Models.Survey survey)
        {
            var viewModel = new SurveyDetailViewModel
            {
                Id = survey.Id,
                Title = survey.Title,
                Description = survey.Description,
                CreatedAt = survey.CreatedAt,
                Status = survey.Status,
                Questions = new List<SurveyQuestionViewModel>()
            };
            
            foreach (var question in survey.Questions)
            {
                var questionViewModel = new SurveyQuestionViewModel
                {
                    Id = question.Id.ToString(),
                    Title = question.Text,
                    Description = question.Description,
                    Type = question.Type,
                    Required = question.Required,
                    Options = question.Options ?? new List<string>(),
                    Settings = question.Settings != null ? new QuestionSettingsViewModel
                    {
                        Min = question.Settings.Min,
                        Max = question.Settings.Max
                    } : null
                };
                
                questionViewModel.EnsureConsistency();
                viewModel.Questions.Add(questionViewModel);
            }
            
            return viewModel;
        }
    }
}
