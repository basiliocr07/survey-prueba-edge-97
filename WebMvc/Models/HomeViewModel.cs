
namespace SurveyApp.WebMvc.Models
{
    public class HomeViewModel
    {
        public List<FeatureViewModel> Features { get; set; } = new List<FeatureViewModel>();
        public bool ShowWelcomeBanner { get; set; } = true;
    }

    public class FeatureViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string LinkUrl { get; set; } = string.Empty;
    }

    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
