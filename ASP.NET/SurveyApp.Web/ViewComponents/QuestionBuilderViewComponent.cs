
using Microsoft.AspNetCore.Mvc;
using SurveyApp.Web.Models;
using System.Threading.Tasks;

namespace SurveyApp.Web.ViewComponents
{
    public class QuestionBuilderViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(QuestionViewModel question, int index)
        {
            // Safely convert QuestionViewModel to SurveyQuestionViewModel
            var surveyQuestion = SurveyQuestionViewModel.FromQuestionViewModel(question);

            return View(new QuestionBuilderViewModel
            {
                Question = surveyQuestion,
                Index = index
            });
        }
    }
}
