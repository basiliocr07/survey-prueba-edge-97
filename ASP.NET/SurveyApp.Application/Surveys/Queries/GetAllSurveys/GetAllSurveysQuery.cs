
using MediatR;
using SurveyApp.Domain.Models;
using System.Collections.Generic;

namespace SurveyApp.Application.Surveys.Queries.GetAllSurveys
{
    public class GetAllSurveysQuery : IRequest<IEnumerable<Survey>>
    {
        // Esta clase no necesita propiedades porque simplemente recupera todas las encuestas
        
        public string? StatusFilter { get; set; }
        
        public GetAllSurveysQuery(string? statusFilter = null)
        {
            StatusFilter = statusFilter;
        }
    }
}
