
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SurveyApp.Web.Models;

namespace SurveyApp.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var features = new List<FeatureViewModel>
        {
            new FeatureViewModel
            {
                Title = "Intuitive Survey Builder",
                Description = "Create beautiful surveys with our drag-and-drop interface. No coding required.",
                IconName = "FileText"
            },
            new FeatureViewModel
            {
                Title = "Powerful Analytics",
                Description = "Get real-time insights with customizable reports and advanced visualizations.",
                IconName = "BarChart3"
            },
            new FeatureViewModel
            {
                Title = "Multiple Question Types",
                Description = "Choose from a variety of question types to gather the precise data you need.",
                IconName = "CheckSquare"
            },
            new FeatureViewModel
            {
                Title = "Rating Scales",
                Description = "Measure sentiment and satisfaction with customizable rating scales.",
                IconName = "Star"
            },
            new FeatureViewModel
            {
                Title = "Ranking Questions",
                Description = "Allow respondents to rank items in order of preference or importance.",
                IconName = "MoveVertical"
            },
            new FeatureViewModel
            {
                Title = "Logic Branching",
                Description = "Create dynamic surveys that adapt based on previous answers.",
                IconName = "PlusCircle"
            }
        };

        return View(features);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
