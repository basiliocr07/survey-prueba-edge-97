
using System.Collections.Generic;

namespace SurveyApp.Web.Models
{
    public class SurveyQuestionViewModel
    {
        public string Id { get; set; }
        public string Type { get; set; }
        
        // This will be the main text/title of the question
        public string Text { get; set; }
        
        // Keeping Title for backward compatibility, but it's the same as Text
        public string Title { get { return Text; } set { Text = value; } }
        
        // Keeping Question for backward compatibility, but it's the same as Text
        public string Question { get { return Text; } set { Text = value; } }
        
        public string Description { get; set; }
        public List<string> Options { get; set; } = new List<string>();
        public bool Required { get; set; }
        public QuestionSettingsViewModel Settings { get; set; }
    }
}
