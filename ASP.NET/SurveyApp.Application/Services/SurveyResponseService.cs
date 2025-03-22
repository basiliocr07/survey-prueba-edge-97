
using SurveyApp.Application.Interfaces;
using SurveyApp.Domain.Models;
using SurveyApp.Domain.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SurveyApp.Application.Services
{
    public class SurveyResponseService : ISurveyResponseService
    {
        private readonly ISurveyResponseRepository _surveyResponseRepository;

        public SurveyResponseService(ISurveyResponseRepository surveyResponseRepository)
        {
            _surveyResponseRepository = surveyResponseRepository;
        }

        public async Task<IEnumerable<SurveyResponse>> GetAllResponsesAsync()
        {
            return await _surveyResponseRepository.GetAllAsync();
        }

        public async Task<SurveyResponse?> GetResponseByIdAsync(int id)
        {
            return await _surveyResponseRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<SurveyResponse>> GetResponsesBySurveyIdAsync(int surveyId)
        {
            return await _surveyResponseRepository.GetBySurveyIdAsync(surveyId);
        }

        public async Task<bool> SubmitResponseAsync(SurveyResponse surveyResponse)
        {
            return await _surveyResponseRepository.AddAsync(surveyResponse);
        }

        public async Task<bool> UpdateResponseAsync(SurveyResponse surveyResponse)
        {
            return await _surveyResponseRepository.UpdateAsync(surveyResponse);
        }

        public async Task<bool> DeleteResponseAsync(int id)
        {
            return await _surveyResponseRepository.DeleteAsync(id);
        }
    }
}
