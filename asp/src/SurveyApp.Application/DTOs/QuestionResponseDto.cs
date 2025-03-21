
using System;

namespace SurveyApp.Application.DTOs
{
    public class QuestionResponseDto
    {
        public Guid QuestionId { get; set; }
        public string Answer { get; set; }
        public int? Rating { get; set; }
        public string TextAnswer { get; set; }
        public bool? BooleanAnswer { get; set; }
        public string[] MultipleChoiceAnswers { get; set; }
    }
}
