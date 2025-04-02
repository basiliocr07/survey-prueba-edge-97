
using MediatR;
using SurveyApp.Application.Interfaces;
using SurveyApp.Application.Surveys.Commands.CreateSurvey;
using SurveyApp.Application.Surveys.Commands.UpdateSurvey;
using SurveyApp.Application.Surveys.Queries.GetAllSurveys;
using SurveyApp.Application.Surveys.Queries.GetSurveyById;
using SurveyApp.Domain.Models;
using SurveyApp.Domain.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SurveyApp.Application.Services
{
    public class SurveyService : ISurveyService
    {
        private readonly IMediator _mediator;
        private readonly ISurveyRepository _surveyRepository; // Para operaciones que aún no se han migrado a CQRS

        public SurveyService(IMediator mediator, ISurveyRepository surveyRepository)
        {
            _mediator = mediator;
            _surveyRepository = surveyRepository;
        }

        public async Task<IEnumerable<Survey>> GetAllSurveysAsync()
        {
            return await _mediator.Send(new GetAllSurveysQuery());
        }

        public async Task<IEnumerable<Survey>> GetSurveysByStatusAsync(string status)
        {
            return await _mediator.Send(new GetAllSurveysQuery(status));
        }

        public async Task<Survey?> GetSurveyByIdAsync(int id)
        {
            return await _mediator.Send(new GetSurveyByIdQuery(id));
        }

        public async Task<bool> CreateSurveyAsync(Survey survey)
        {
            var command = new CreateSurveyCommand(
                survey.Title,
                survey.Description,
                survey.Questions,
                survey.Status,
                survey.DeliveryConfig
            );

            var id = await _mediator.Send(command);
            return id > 0;
        }

        public async Task<bool> UpdateSurveyAsync(Survey survey)
        {
            var command = new UpdateSurveyCommand
            {
                Id = survey.Id,
                Title = survey.Title,
                Description = survey.Description,
                Status = survey.Status,
                Questions = survey.Questions,
                DeliveryConfig = survey.DeliveryConfig
            };

            return await _mediator.Send(command);
        }

        public async Task<bool> DeleteSurveyAsync(int id)
        {
            // Todavía usamos el repositorio directo para esta operación
            // En un futuro podríamos migrarla a un comando DeleteSurveyCommand
            return await _surveyRepository.DeleteAsync(id);
        }

        public async Task<bool> SendSurveyEmailsAsync(int surveyId, List<string> emailAddresses)
        {
            // Todavía usamos el repositorio directo para esta operación
            return await _surveyRepository.SendEmailsAsync(surveyId, emailAddresses);
        }

        public async Task<SurveyStatistics> GetSurveyStatisticsAsync(int surveyId)
        {
            // Todavía usamos el repositorio directo para esta operación
            return await _surveyRepository.GetStatisticsAsync(surveyId);
        }
    }
}
