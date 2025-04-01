
using MediatR;
using SurveyApp.Application.Interfaces;
using SurveyApp.Application.Surveys.Commands;
using SurveyApp.Application.Surveys.Queries;
using SurveyApp.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SurveyApp.Application.Services
{
    public class SurveyService : ISurveyService
    {
        private readonly IMediator _mediator;

        public SurveyService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IEnumerable<Survey>> GetAllSurveysAsync()
        {
            return await _mediator.Send(new GetAllSurveysQuery());
        }

        public async Task<Survey?> GetSurveyByIdAsync(int id)
        {
            return await _mediator.Send(new GetSurveyByIdQuery { Id = id });
        }

        public async Task<bool> CreateSurveyAsync(Survey survey)
        {
            var command = new CreateSurveyCommand
            {
                Title = survey.Title,
                Description = survey.Description,
                Status = survey.Status,
                Questions = survey.Questions,
                DeliveryConfig = survey.DeliveryConfig
            };

            return await _mediator.Send(command);
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
            return await _mediator.Send(new DeleteSurveyCommand { Id = id });
        }

        public async Task<IEnumerable<Survey>> GetSurveysByStatusAsync(string status)
        {
            return await _mediator.Send(new GetSurveysByStatusQuery { Status = status });
        }

        public async Task<bool> SendSurveyEmailsAsync(int surveyId, List<string> emailAddresses)
        {
            // Implementación pendiente - podría ser otra Command
            return true;
        }

        public async Task<SurveyStatistics> GetSurveyStatisticsAsync(int surveyId)
        {
            return await _mediator.Send(new GetSurveyStatisticsQuery { SurveyId = surveyId });
        }
    }
}
