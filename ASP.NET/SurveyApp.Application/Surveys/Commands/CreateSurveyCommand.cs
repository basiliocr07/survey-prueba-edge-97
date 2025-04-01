
using MediatR;
using SurveyApp.Domain.Models;
using System.Collections.Generic;

namespace SurveyApp.Application.Surveys.Commands
{
    public class CreateSurveyCommand : IRequest<bool>
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "draft";
        public List<Question> Questions { get; set; } = new List<Question>();
        public DeliveryConfiguration? DeliveryConfig { get; set; }
    }
}
