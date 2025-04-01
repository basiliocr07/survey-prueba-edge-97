
using SurveyApp.Domain.Models;
using System.Collections.Generic;

namespace SurveyApp.Web.Models
{
    public class SurveysViewModel
    {
        public List<Survey> Surveys { get; set; } = new List<Survey>();
        public string FilterActive { get; set; } = "all";
        public bool IsLoading { get; set; } = false;
    }
}
