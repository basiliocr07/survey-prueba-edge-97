
using System;
using System.Collections.Generic;

namespace SurveyApp.Application.DTOs
{
    public class UpdateSurveyDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<CreateQuestionDto> Questions { get; set; }
        public DeliveryConfigDto DeliveryConfig { get; set; }
    }
}
