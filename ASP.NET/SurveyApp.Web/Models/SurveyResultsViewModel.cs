
using System;
using System.Collections.Generic;

namespace SurveyApp.Web.Models
{
    public class SurveyResultsViewModel
    {
        public SurveyViewModel Survey { get; set; }
        public SurveyStatisticsViewModel Statistics { get; set; }
        public List<QuestionResultViewModel> QuestionResults { get; set; } = new List<QuestionResultViewModel>();
    }

    public class QuestionResultViewModel
    {
        public string QuestionId { get; set; }
        public string QuestionTitle { get; set; }
        public string QuestionType { get; set; }
        public List<AnswerResultViewModel> Answers { get; set; } = new List<AnswerResultViewModel>();
    }

    public class AnswerResultViewModel
    {
        public string Answer { get; set; }
        public int Count { get; set; }
        public double Percentage { get; set; }
        public string Color { get; set; } // Para gráficos
    }
}
