
using Microsoft.AspNetCore.Mvc;
using SurveyApp.Web.Models;
using System.Threading.Tasks;

namespace SurveyApp.Web.ViewComponents
{
    public class QuestionBuilderViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(QuestionViewModel question, int index)
        {
            // Convert QuestionViewModel to SurveyQuestionViewModel
            var surveyQuestion = new SurveyQuestionViewModel
            {
                Id = question.Id,
                Type = question.Type,
                Text = question.Text,
                Description = question.Description,
                Options = question.Options,
                Required = question.Required
            };

            return View(new QuestionBuilderViewModel
            {
                Question = surveyQuestion,
                Index = index
            });
        }
    }
}
