
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SurveyApp.Application.DTOs;

namespace SurveyApp.Application.Services
{
    public partial class SurveyService
    {
        public async Task<SurveyDto> GetSurveyByIdAsync(Guid id)
        {
            var survey = await _surveyRepository.GetByIdAsync(id);
            if (survey == null)
                return null;

            return MapToSurveyDto(survey);
        }

        public async Task<List<SurveyDto>> GetAllSurveysAsync()
        {
            var surveys = await _surveyRepository.GetAllAsync();
            var surveyDtos = new List<SurveyDto>();
            
            foreach (var survey in surveys)
            {
                surveyDtos.Add(MapToSurveyDto(survey));
            }
            
            return surveyDtos;
        }

        public async Task<bool> SurveyExistsAsync(Guid id)
        {
            return await _surveyRepository.ExistsAsync(id);
        }

        public async Task<List<string>> GetAllCategoriesAsync()
        {
            return await _surveyRepository.GetAllCategoriesAsync();
        }

        public async Task<(List<SurveyDto> Surveys, int TotalCount)> GetPagedSurveysAsync(
            int pageNumber, 
            int pageSize, 
            string searchTerm = null, 
            string statusFilter = null, 
            string categoryFilter = null)
        {
            var (surveys, totalCount) = await _surveyRepository.GetPagedSurveysAsync(
                pageNumber, 
                pageSize, 
                searchTerm, 
                statusFilter, 
                categoryFilter);
                
            var surveyDtos = new List<SurveyDto>();
            
            foreach (var survey in surveys)
            {
                surveyDtos.Add(MapToSurveyDto(survey));
            }
            
            return (surveyDtos, totalCount);
        }
    }
}
