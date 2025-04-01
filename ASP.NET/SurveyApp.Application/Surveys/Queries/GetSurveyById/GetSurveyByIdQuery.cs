
using MediatR;
using SurveyApp.Domain.Models;

namespace SurveyApp.Application.Surveys.Queries.GetSurveyById
{
    public class GetSurveyByIdQuery : IRequest<Survey?>
    {
        public int Id { get; set; }

        public GetSurveyByIdQuery(int id)
        {
            Id = id;
        }
    }
}
