
using System;
using System.Collections.Generic;
using SurveyApp.Domain.Entities;

namespace SurveyApp.Application.Ports
{
    public interface ISurveyResponseRepository
    {
        IEnumerable<SurveyResponse> GetAll();
        SurveyResponse GetById(Guid id);
        IEnumerable<SurveyResponse> GetBySurveyId(Guid surveyId);
        int GetCountBySurveyId(Guid surveyId);
        void Add(SurveyResponse surveyResponse);
        void Update(SurveyResponse surveyResponse);
        void Delete(Guid id);
    }
}
