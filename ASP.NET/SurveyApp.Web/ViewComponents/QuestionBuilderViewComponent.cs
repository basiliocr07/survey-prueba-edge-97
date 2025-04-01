
using Microsoft.AspNetCore.Mvc;
using SurveyApp.Web.Models;

namespace SurveyApp.Web.ViewComponents
{
    public class QuestionBuilderViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(QuestionViewModel question, int index, int total)
        {
            // Asegurar que la pregunta tenga las propiedades necesarias
            question.EnsureConsistency();
            
            ViewData["QuestionIndex"] = index;
            ViewData["TotalQuestions"] = total;
            ViewData["IsFirst"] = index == 0;
            ViewData["IsLast"] = index == total - 1;
            
            return View(question);
        }
    }
}
