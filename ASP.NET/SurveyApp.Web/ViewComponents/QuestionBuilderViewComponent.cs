
using Microsoft.AspNetCore.Mvc;
using SurveyApp.Web.Models;
using System.Threading.Tasks;

namespace SurveyApp.Web.ViewComponents
{
    public class QuestionBuilderViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(QuestionViewModel question, int index, int total = 0)
        {
            // Safely convert QuestionViewModel to SurveyQuestionViewModel
            // and ensure all properties are properly mapped
            var surveyQuestion = SurveyQuestionViewModel.FromQuestionViewModel(question);
            
            // Ensure consistency across all fields
            surveyQuestion.EnsureConsistency();
            
            return View(new QuestionBuilderViewModel
            {
                Question = surveyQuestion,
                Index = index,
                Total = total,
                IsFirst = index == 0,
                IsLast = index == total - 1
            });
        }
    }
}
