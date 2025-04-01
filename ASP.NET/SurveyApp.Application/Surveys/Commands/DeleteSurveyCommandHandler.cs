
using MediatR;
using SurveyApp.Domain.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace SurveyApp.Application.Surveys.Commands
{
    public class DeleteSurveyCommandHandler : IRequestHandler<DeleteSurveyCommand, bool>
    {
        private readonly ISurveyRepository _surveyRepository;

        public DeleteSurveyCommandHandler(ISurveyRepository surveyRepository)
        {
            _surveyRepository = surveyRepository;
        }

        public async Task<bool> Handle(DeleteSurveyCommand request, CancellationToken cancellationToken)
        {
            return await _surveyRepository.DeleteAsync(request.Id);
        }
    }
}
