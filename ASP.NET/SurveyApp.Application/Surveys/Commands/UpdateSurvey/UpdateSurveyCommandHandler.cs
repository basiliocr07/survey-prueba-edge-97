
using MediatR;
using SurveyApp.Domain.Models;
using SurveyApp.Domain.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SurveyApp.Application.Surveys.Commands.UpdateSurvey
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
            var survey = new Survey
            {
                Id = request.Id,
                Title = request.Title,
                Description = request.Description,
                Status = request.Status,
                Questions = request.Questions,
                DeliveryConfig = request.DeliveryConfig
            };

            return await _surveyRepository.UpdateAsync(survey);
        }
    }
}
