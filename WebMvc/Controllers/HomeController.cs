
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SurveyApp.WebMvc.Models;

namespace SurveyApp.WebMvc.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var featuresViewModel = new HomeViewModel
            {
                Features = new List<FeatureViewModel>
                {
                    new FeatureViewModel
                    {
                        Title = "Intuitive Survey Builder",
                        Description = "Create beautiful surveys with our drag-and-drop interface. No coding required.",
                        Icon = "file-lines"
                    },
                    new FeatureViewModel
                    {
                        Title = "Powerful Analytics",
                        Description = "Get real-time insights with customizable reports and advanced visualizations.",
                        Icon = "chart-bar"
                    },
                    new FeatureViewModel
                    {
                        Title = "Multiple Question Types",
                        Description = "Choose from a variety of question types to gather the precise data you need.",
                        Icon = "check-square"
                    },
                    new FeatureViewModel
                    {
                        Title = "Customer Feedback",
                        Description = "Collect and manage customer suggestions to improve your products.",
                        Icon = "message"
                    },
                    new FeatureViewModel
                    {
                        Title = "Customer Growth",
                        Description = "Track your customer base growth and analyze service usage trends.",
                        Icon = "users"
                    },
                    new FeatureViewModel
                    {
                        Title = "Knowledge Base",
                        Description = "Build and maintain a comprehensive knowledge base for your team and customers.",
                        Icon = "book"
                    }
                }
            };

            return View(featuresViewModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
