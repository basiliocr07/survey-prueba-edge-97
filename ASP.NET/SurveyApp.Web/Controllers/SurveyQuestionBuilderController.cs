
using Microsoft.AspNetCore.Mvc;
using SurveyApp.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SurveyApp.Web.Controllers
{
    public class SurveyBuilderController : Controller
    {
        [HttpGet]
        public IActionResult AddQuestion(int index, string id)
        {
            // Create a new question
            var question = new QuestionViewModel
            {
                Id = id ?? Guid.NewGuid().ToString(),
                Text = "New Question",
                Type = "text",
                Required = true
            };
            
            // Return the partial view
            return ViewComponent("QuestionBuilder", new { question = question, index = index, total = index + 1 });
        }
        
        [HttpGet]
        public IActionResult AddSampleQuestions(int startIndex)
        {
            var sampleQuestions = new List<Tuple<QuestionViewModel, int, int>>();
            var questions = new List<QuestionViewModel>
            {
                new QuestionViewModel
                {
                    Id = Guid.NewGuid().ToString(),
                    Text = "How satisfied are you with our service?",
                    Type = "rating",
                    Required = true,
                    Settings = new QuestionSettingsViewModel { Min = 1, Max = 5 }
                },
                new QuestionViewModel
                {
                    Id = Guid.NewGuid().ToString(),
                    Text = "What features do you like most?",
                    Type = "multiple-choice",
                    Required = true,
                    Options = new List<string> { "User Interface", "Performance", "Customer Support", "Price" }
                },
                new QuestionViewModel
                {
                    Id = Guid.NewGuid().ToString(),
                    Text = "Please provide any additional feedback",
                    Type = "text",
                    Required = false
                }
            };
            
            for (int i = 0; i < questions.Count; i++)
            {
                sampleQuestions.Add(new Tuple<QuestionViewModel, int, int>(
                    questions[i], 
                    startIndex + i, 
                    startIndex + questions.Count
                ));
            }
            
            return PartialView("_MultipleSampleQuestionsPartial", sampleQuestions);
        }
    }
}
