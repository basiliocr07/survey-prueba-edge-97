
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SurveyApp.Application.DTOs;

namespace SurveyApp.Application.Services
{
    public interface ISurveyService
    {
        Task<SurveyDto> GetSurveyByIdAsync(Guid id);
        Task<List<SurveyDto>> GetAllSurveysAsync();
        Task<SurveyDto> CreateSurveyAsync(CreateSurveyDto createSurveyDto);
        Task UpdateSurveyAsync(Guid id, CreateSurveyDto updateSurveyDto);
        Task DeleteSurveyAsync(Guid id);
        Task SendSurveyEmailsAsync(Guid id);
        Task SendSurveyOnTicketClosedAsync(string customerEmail, Guid? specificSurveyId = null);
    }
}
