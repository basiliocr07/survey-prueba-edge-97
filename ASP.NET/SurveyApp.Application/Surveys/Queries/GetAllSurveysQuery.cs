
using MediatR;
using SurveyApp.Domain.Models;
using System.Collections.Generic;

namespace SurveyApp.Application.Surveys.Queries
{
    public class GetAllSurveysQuery : IRequest<IEnumerable<Survey>>
    {
    }
}
