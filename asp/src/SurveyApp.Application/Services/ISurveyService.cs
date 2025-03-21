
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SurveyApp.Domain.Entities;

namespace SurveyApp.Application.Services
{
    public interface ISurveyService
    {
        Task<IEnumerable<Survey>> GetAllSurveysAsync();
        Task<Survey> GetSurveyByIdAsync(Guid id);
        Task<Survey> CreateSurveyAsync(string title, string description, string category);
        Task UpdateSurveyAsync(Guid id, string title, string description, string category);
        Task DeleteSurveyAsync(Guid id);
        Task AddQuestionToSurveyAsync(Guid surveyId, Question question);
        Task PublishSurveyAsync(Guid id);
    }
}
