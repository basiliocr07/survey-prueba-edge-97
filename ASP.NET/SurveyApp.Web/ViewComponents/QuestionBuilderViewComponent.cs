
using Microsoft.AspNetCore.Mvc;
using SurveyApp.Web.Models;
using System.Threading.Tasks;

namespace SurveyApp.Web.ViewComponents
{
    public class QuestionBuilderViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(QuestionViewModel question, int index, int total)
        {
            // Ensure question and options are properly initialized
            if (question.Options == null)
            {
                question.Options = new System.Collections.Generic.List<string>();
            }

            var model = new Tuple<QuestionViewModel, int, int>(question, index, total);
            return View(model);
        }
    }
}
