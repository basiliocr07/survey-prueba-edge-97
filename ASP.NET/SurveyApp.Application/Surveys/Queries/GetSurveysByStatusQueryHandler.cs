
using MediatR;
using SurveyApp.Domain.Models;
using SurveyApp.Domain.Repositories;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SurveyApp.Application.Surveys.Queries
{
    public class GetSurveysByStatusQueryHandler : IRequestHandler<GetSurveysByStatusQuery, IEnumerable<Survey>>
    {
        private readonly ISurveyRepository _surveyRepository;

        public GetSurveysByStatusQueryHandler(ISurveyRepository surveyRepository)
        {
            _surveyRepository = surveyRepository;
        }

        public async Task<IEnumerable<Survey>> Handle(GetSurveysByStatusQuery request, CancellationToken cancellationToken)
        {
            return await _surveyRepository.GetByStatusAsync(request.Status);
        }
    }
}
