
using MediatR;
using SurveyApp.Domain.Models;
using SurveyApp.Domain.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace SurveyApp.Application.Surveys.Queries
{
    public class GetSurveyStatisticsQueryHandler : IRequestHandler<GetSurveyStatisticsQuery, SurveyStatistics>
    {
        private readonly ISurveyRepository _surveyRepository;

        public GetSurveyStatisticsQueryHandler(ISurveyRepository surveyRepository)
        {
            _surveyRepository = surveyRepository;
        }

        public async Task<SurveyStatistics> Handle(GetSurveyStatisticsQuery request, CancellationToken cancellationToken)
        {
            return await _surveyRepository.GetStatisticsAsync(request.SurveyId);
        }
    }
}
