
using Microsoft.AspNetCore.Mvc;
using SurveyApp.Web.Models;
using System.Threading.Tasks;

namespace SurveyApp.Web.ViewComponents
{
    public class QuestionBuilderViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(SurveyQuestionViewModel question, int index)
        {
            return View(new QuestionBuilderViewModel
            {
                Question = question,
                Index = index
            });
        }
    }

    public class QuestionBuilderViewModel
    {
        public SurveyQuestionViewModel Question { get; set; }
        public int Index { get; set; }
    }
}
