
using MediatR;

namespace SurveyApp.Application.Surveys.Commands
{
    public class DeleteSurveyCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
