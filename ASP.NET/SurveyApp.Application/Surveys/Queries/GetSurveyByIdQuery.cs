
using MediatR;
using SurveyApp.Domain.Models;

namespace SurveyApp.Application.Surveys.Queries
{
    public class GetSurveyByIdQuery : IRequest<Survey?>
    {
        public int Id { get; set; }
    }
}
