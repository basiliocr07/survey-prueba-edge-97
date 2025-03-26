
using Microsoft.AspNetCore.Mvc;
using SurveyApp.Web.Models;
using System;
using System.Collections.Generic;

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
            var sampleQuestions = new List<QuestionViewModel>
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
            
            var result = new List<IActionResult>();
            
            for (int i = 0; i < sampleQuestions.Count; i++)
            {
                result.Add(ViewComponent("QuestionBuilder", new { 
                    question = sampleQuestions[i], 
                    index = startIndex + i, 
                    total = startIndex + sampleQuestions.Count 
                }));
            }
            
            return new ContentResult
            {
                Content = string.Join("", result.Select(r => r.ToString())),
                ContentType = "text/html"
            };
        }
    }
}
