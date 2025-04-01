
using MediatR;
using SurveyApp.Domain.Models;
using System;
using System.Collections.Generic;

namespace SurveyApp.Application.Surveys.Commands.CreateSurvey
{
    public class CreateSurveyCommand : IRequest<int>
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "draft";
        public List<Question> Questions { get; set; } = new List<Question>();
        public DeliveryConfiguration? DeliveryConfig { get; set; }
        
        // Constructor para facilitar la creación desde el controlador
        public CreateSurveyCommand()
        {
        }
        
        public CreateSurveyCommand(string title, string description, List<Question> questions, string status = "draft", DeliveryConfiguration? deliveryConfig = null)
        {
            Title = title;
            Description = description;
            Status = status;
            Questions = questions;
            DeliveryConfig = deliveryConfig;
        }
    }
}
