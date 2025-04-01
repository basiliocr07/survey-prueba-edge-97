
using MediatR;
using SurveyApp.Domain.Models;
using SurveyApp.Domain.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SurveyApp.Application.Surveys.Commands.CreateSurvey
{
    public class CreateSurveyCommandHandler : IRequestHandler<CreateSurveyCommand, int>
    {
        private readonly ISurveyRepository _surveyRepository;

        public CreateSurveyCommandHandler(ISurveyRepository surveyRepository)
        {
            _surveyRepository = surveyRepository;
        }

        public async Task<int> Handle(CreateSurveyCommand request, CancellationToken cancellationToken)
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

            var success = await _surveyRepository.AddAsync(survey);
            
            if (success)
            {
                // En un escenario real, el repositorio debería devolver el ID de la encuesta creada
                // Para este ejemplo, asumimos que podemos obtener el ID de alguna manera
                return survey.Id;
            }
            
            throw new Exception("Failed to create survey");
        }
    }
}
