
using System;
using System.Collections.Generic;
using SurveyApp.Domain.Models;

namespace SurveyApp.Web.Models
{
    public class SurveyResultsViewModel
    {
        public SurveyViewModel Survey { get; set; }
        public SurveyStatistics Statistics { get; set; }
    }
}
