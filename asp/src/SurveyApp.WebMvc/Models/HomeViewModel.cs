
using System.Collections.Generic;

namespace SurveyApp.WebMvc.Models
{
    public class HomeViewModel
    {
        public List<FeatureViewModel> Features { get; set; } = new List<FeatureViewModel>();
        public bool IsAuthenticated { get; set; }
        public string Username { get; set; } = string.Empty;
        public string? UserRole { get; set; }
    }

    public class FeatureViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
    }
}
