
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
            // and ensure all properties are properly mapped
            var surveyQuestion = SurveyQuestionViewModel.FromQuestionViewModel(question);
            
            // Double check that no data was lost in the conversion
            if (question != null && surveyQuestion != null)
            {
                // Ensure required properties are set
                if (string.IsNullOrEmpty(surveyQuestion.Type) && !string.IsNullOrEmpty(question.Type))
                {
                    surveyQuestion.Type = question.Type;
                }
                
                if (string.IsNullOrEmpty(surveyQuestion.Text) && !string.IsNullOrEmpty(question.Text))
                {
                    surveyQuestion.Text = question.Text;
                }
            }

            return View(new QuestionBuilderViewModel
            {
                Question = surveyQuestion,
                Index = index
            });
        }
    }
}
