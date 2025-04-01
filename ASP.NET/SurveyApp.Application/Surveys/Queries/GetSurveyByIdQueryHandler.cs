
using MediatR;
using SurveyApp.Domain.Models;
using SurveyApp.Domain.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace SurveyApp.Application.Surveys.Queries
{
    public class GetSurveyByIdQueryHandler : IRequestHandler<GetSurveyByIdQuery, Survey?>
    {
        private readonly ISurveyRepository _surveyRepository;

        public GetSurveyByIdQueryHandler(ISurveyRepository surveyRepository)
        {
            _surveyRepository = surveyRepository;
        }

        public async Task<Survey?> Handle(GetSurveyByIdQuery request, CancellationToken cancellationToken)
        {
            return await _surveyRepository.GetByIdAsync(request.Id);
        }
    }
}
