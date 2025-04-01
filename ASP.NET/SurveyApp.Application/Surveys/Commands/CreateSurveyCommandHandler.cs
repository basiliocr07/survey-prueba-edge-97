
using MediatR;
using SurveyApp.Domain.Models;
using SurveyApp.Domain.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SurveyApp.Application.Surveys.Commands
{
    public class CreateSurveyCommandHandler : IRequestHandler<CreateSurveyCommand, bool>
    {
        private readonly ISurveyRepository _surveyRepository;

        public CreateSurveyCommandHandler(ISurveyRepository surveyRepository)
        {
            _surveyRepository = surveyRepository;
        }

        public async Task<bool> Handle(CreateSurveyCommand request, CancellationToken cancellationToken)
        {
            var survey = new Survey
            {
                Title = request.Title,
                Description = request.Description,
                Status = request.Status,
                CreatedAt = DateTime.Now,
                Questions = request.Questions,
                DeliveryConfig = request.DeliveryConfig
            };

            return await _surveyRepository.AddAsync(survey);
        }
    }
}
