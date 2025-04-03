
using System.ComponentModel.DataAnnotations;

namespace SurveyApp.Web.Models
{
    public class QuestionSettingsViewModel
    {
        [Range(0, 100, ErrorMessage = "Minimum value must be between 0 and 100")]
        public int? Min { get; set; }
        
        [Range(1, 100, ErrorMessage = "Maximum value must be between 1 and 100")]
        public int? Max { get; set; }
        
        // Ensure min is less than max
        public bool ValidateMinMax()
        {
            if (Min.HasValue && Max.HasValue)
            {
                return Min.Value < Max.Value;
            }
            
            return true;
        }
    }
}
