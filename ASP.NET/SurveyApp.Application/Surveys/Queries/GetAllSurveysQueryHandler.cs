
using MediatR;
using SurveyApp.Domain.Models;
using SurveyApp.Domain.Repositories;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SurveyApp.Application.Surveys.Queries
{
    public class GetAllSurveysQueryHandler : IRequestHandler<GetAllSurveysQuery, IEnumerable<Survey>>
    {
        private readonly ISurveyRepository _surveyRepository;

        public GetAllSurveysQueryHandler(ISurveyRepository surveyRepository)
        {
            _surveyRepository = surveyRepository;
        }

        public async Task<IEnumerable<Survey>> Handle(GetAllSurveysQuery request, CancellationToken cancellationToken)
        {
            return await _surveyRepository.GetAllAsync();
        }
    }
}
