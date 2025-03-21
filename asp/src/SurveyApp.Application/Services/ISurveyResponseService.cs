
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SurveyApp.Application.DTOs;

namespace SurveyApp.Application.Services
{
    public interface ISurveyResponseService
    {
        Task<IEnumerable<SurveyResponseDto>> GetAllResponsesAsync();
        Task<IEnumerable<SurveyResponseDto>> GetResponsesBySurveyIdAsync(Guid surveyId);
        Task<SurveyResponseDto?> GetResponseByIdAsync(Guid id);
        Task<SurveyResponseDto> CreateResponseAsync(CreateSurveyResponseDto responseDto);
    }
}
