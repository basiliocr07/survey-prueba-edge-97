// This file is intentionally left almost empty as the functionality
// has been moved to partial class files to improve maintainability.
// The SuggestionService class is now implemented across multiple files
// that are combined at compile time.

using SurveyApp.Application.Ports;

namespace SurveyApp.Application.Services
{
    // Main class declaration is here, but implementations are in partial class files
    public partial class SuggestionService
    {
        // Implementation is split across multiple partial class files:
        // - SuggestionService.Core.cs - Core functionality and constructor
        // - SuggestionService.Query.cs - Query methods
        // - SuggestionService.Create.cs - Creation methods
        // - SuggestionService.Update.cs - Update methods
        // - SuggestionService.Reports.cs - Reporting methods
        // - SuggestionService.Similar.cs - Similar suggestion search
    }
}
