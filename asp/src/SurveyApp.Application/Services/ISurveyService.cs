
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SurveyApp.Application.DTOs;

namespace SurveyApp.Application.Services
{
    public interface ISurveyService
    {
        Task<IEnumerable<SurveyDto>> GetAllSurveysAsync();
        Task<SurveyDto?> GetSurveyByIdAsync(Guid id);
        Task<SurveyDto> CreateSurveyAsync(CreateSurveyDto surveyDto);
        Task UpdateSurveyAsync(Guid id, CreateSurveyDto surveyDto);
        Task DeleteSurveyAsync(Guid id);
    }
}
