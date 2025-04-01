
using MediatR;
using SurveyApp.Domain.Models;

namespace SurveyApp.Application.Surveys.Queries
{
    public class GetSurveyStatisticsQuery : IRequest<SurveyStatistics>
    {
        public int SurveyId { get; set; }
    }
}
