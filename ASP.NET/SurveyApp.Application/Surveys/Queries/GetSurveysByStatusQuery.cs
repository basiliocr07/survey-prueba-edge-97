
using MediatR;
using SurveyApp.Domain.Models;
using System.Collections.Generic;

namespace SurveyApp.Application.Surveys.Queries
{
    public class GetSurveysByStatusQuery : IRequest<IEnumerable<Survey>>
    {
        public string Status { get; set; } = "all";
    }
}
