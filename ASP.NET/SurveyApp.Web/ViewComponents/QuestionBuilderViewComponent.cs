
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
            
            // Ensure settings is initialized based on question type
            if (question.Settings == null)
            {
                question.Settings = new QuestionSettingsViewModel();
                
                // Set default settings based on question type
                if (question.Type == "rating")
                {
                    question.Settings.Min = 1;
                    question.Settings.Max = 5;
                }
                else if (question.Type == "nps")
                {
                    question.Settings.Min = 0;
                    question.Settings.Max = 10;
                }
            }

            var model = new Tuple<QuestionViewModel, int, int>(question, index, total);
            return View(model);
        }
    }
}
