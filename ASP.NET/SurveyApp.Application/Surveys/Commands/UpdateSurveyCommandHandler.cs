
using MediatR;
using SurveyApp.Domain.Models;
using SurveyApp.Domain.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace SurveyApp.Application.Surveys.Commands
{
    public class UpdateSurveyCommandHandler : IRequestHandler<UpdateSurveyCommand, bool>
    {
        private readonly ISurveyRepository _surveyRepository;

        public UpdateSurveyCommandHandler(ISurveyRepository surveyRepository)
        {
            _surveyRepository = surveyRepository;
        }

        public async Task<bool> Handle(UpdateSurveyCommand request, CancellationToken cancellationToken)
        {
            var survey = await _surveyRepository.GetByIdAsync(request.Id);
            if (survey == null)
            {
                return false;
            }

            // Actualizar propiedades
            survey.Title = request.Title;
            survey.Description = request.Description;
            survey.Status = request.Status;
            survey.Questions = request.Questions;
            survey.DeliveryConfig = request.DeliveryConfig;

            return await _surveyRepository.UpdateAsync(survey);
        }
    }
}
