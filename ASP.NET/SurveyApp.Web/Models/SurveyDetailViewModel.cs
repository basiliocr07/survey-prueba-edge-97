
using System.Collections.Generic;
using System;

namespace SurveyApp.Web.Models
{
    /// <summary>
    /// ViewModel detallado para mostrar una encuesta completa
    /// </summary>
    public class SurveyDetailViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        
        /// <summary>
        /// Lista de preguntas de la encuesta usando SurveyQuestionViewModel
        /// </summary>
        public List<SurveyQuestionViewModel> Questions { get; set; } = new List<SurveyQuestionViewModel>();
        
        /// <summary>
        /// Configuración de entrega de la encuesta
        /// </summary>
        public DeliveryConfigViewModel DeliveryConfig { get; set; }
        
        /// <summary>
        /// Crea un SurveyDetailViewModel desde un modelo de dominio Survey
        /// </summary>
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
                    ? DeliveryConfigViewModel.FromDeliveryConfiguration(survey.DeliveryConfig)
                    : null
            };
            
            // Convertir las preguntas del dominio a viewmodels
            foreach (var question in survey.Questions)
            {
                var questionViewModel = new SurveyQuestionViewModel
                {
                    Id = question.Id.ToString(),
                    Title = question.Text, // Mapeamos Text del dominio a Title del viewmodel
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
        
        /// <summary>
        /// Convierte este SurveyDetailViewModel a un modelo de dominio Survey
        /// </summary>
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
            
            // Convertir las preguntas del viewmodel a dominio
            if (this.Questions != null)
            {
                foreach (var question in this.Questions)
                {
                    var domainQuestion = new SurveyApp.Domain.Models.Question
                    {
                        Id = int.TryParse(question.Id, out int id) ? id : 0,
                        Text = question.Title, // Mapeamos Title del viewmodel a Text del dominio
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
            
            // Configuración de entrega
            if (this.DeliveryConfig != null)
            {
                domainModel.DeliveryConfig = this.DeliveryConfig.ToDeliveryConfiguration();
            }
            
            return domainModel;
        }
    }
}
