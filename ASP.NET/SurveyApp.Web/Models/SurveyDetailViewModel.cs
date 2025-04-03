
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
        public DeliveryConfigViewModel DeliveryConfig { get; set; }
        
        public static SurveyDetailViewModel FromDomainModel(SurveyApp.Domain.Models.Survey survey)
        {
            var viewModel = new SurveyDetailViewModel
            {
                Id = survey.Id,
                Title = survey.Title,
                Description = survey.Description,
                CreatedAt = survey.CreatedAt,
                Status = survey.Status,
                Questions = new List<SurveyQuestionViewModel>(),
                DeliveryConfig = survey.DeliveryConfig != null 
                    ? new DeliveryConfigViewModel
                    {
                        Type = survey.DeliveryConfig.Type,
                        EmailAddresses = survey.DeliveryConfig.EmailAddresses,
                        IncludeAllCustomers = survey.DeliveryConfig.IncludeAllCustomers,
                        CustomerTypeFilter = survey.DeliveryConfig.CustomerTypeFilter,
                        Schedule = survey.DeliveryConfig.Schedule != null 
                            ? new ScheduleSettingsViewModel
                            {
                                Frequency = survey.DeliveryConfig.Schedule.Frequency,
                                DayOfMonth = survey.DeliveryConfig.Schedule.DayOfMonth,
                                DayOfWeek = survey.DeliveryConfig.Schedule.DayOfWeek,
                                Time = survey.DeliveryConfig.Schedule.Time,
                                StartDate = survey.DeliveryConfig.Schedule.StartDate
                            } 
                            : null,
                        Trigger = survey.DeliveryConfig.Trigger != null
                            ? new TriggerSettingsViewModel
                            {
                                Type = survey.DeliveryConfig.Trigger.Type,
                                DelayHours = survey.DeliveryConfig.Trigger.DelayHours,
                                SendAutomatically = survey.DeliveryConfig.Trigger.SendAutomatically
                            }
                            : null
                    }
                    : null
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
        
        public SurveyApp.Domain.Models.Survey ToDomainModel()
        {
            var domainModel = new SurveyApp.Domain.Models.Survey
            {
                Id = this.Id,
                Title = this.Title,
                Description = this.Description,
                CreatedAt = this.CreatedAt,
                Status = this.Status,
                Questions = new List<SurveyApp.Domain.Models.Question>()
            };
            
            if (this.Questions != null)
            {
                foreach (var question in this.Questions)
                {
                    var domainQuestion = new SurveyApp.Domain.Models.Question
                    {
                        Id = int.TryParse(question.Id, out int id) ? id : 0,
                        Text = question.Title,
                        Description = question.Description ?? "",
                        Type = question.Type,
                        Required = question.Required,
                        Options = question.Options
                    };
                    
                    if (question.Settings != null)
                    {
                        domainQuestion.Settings = new SurveyApp.Domain.Models.QuestionSettings
                        {
                            Min = question.Settings.Min,
                            Max = question.Settings.Max
                        };
                    }
                    
                    domainModel.Questions.Add(domainQuestion);
                }
            }
            
            if (this.DeliveryConfig != null)
            {
                domainModel.DeliveryConfig = new SurveyApp.Domain.Models.DeliveryConfiguration
                {
                    Type = this.DeliveryConfig.Type,
                    EmailAddresses = this.DeliveryConfig.EmailAddresses ?? new List<string>(),
                    IncludeAllCustomers = this.DeliveryConfig.IncludeAllCustomers,
                    CustomerTypeFilter = this.DeliveryConfig.CustomerTypeFilter
                };
                
                if (this.DeliveryConfig.Schedule != null)
                {
                    domainModel.DeliveryConfig.Schedule = new SurveyApp.Domain.Models.ScheduleSettings
                    {
                        Frequency = this.DeliveryConfig.Schedule.Frequency,
                        DayOfMonth = this.DeliveryConfig.Schedule.DayOfMonth ?? 1,
                        DayOfWeek = this.DeliveryConfig.Schedule.DayOfWeek,
                        Time = this.DeliveryConfig.Schedule.Time ?? "09:00",
                        StartDate = this.DeliveryConfig.Schedule.StartDate
                    };
                }
                
                if (this.DeliveryConfig.Trigger != null)
                {
                    domainModel.DeliveryConfig.Trigger = new SurveyApp.Domain.Models.TriggerSettings
                    {
                        Type = this.DeliveryConfig.Trigger.Type,
                        DelayHours = this.DeliveryConfig.Trigger.DelayHours,
                        SendAutomatically = this.DeliveryConfig.Trigger.SendAutomatically
                    };
                }
            }
            
            return domainModel;
        }
    }
}
