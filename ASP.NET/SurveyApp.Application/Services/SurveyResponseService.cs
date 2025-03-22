
using SurveyApp.Application.Interfaces;
using SurveyApp.Domain.Models;
using SurveyApp.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyApp.Application.Services
{
    public class SurveyResponseService : ISurveyResponseService
    {
        private readonly ISurveyResponseRepository _surveyResponseRepository;
        private readonly ISurveyRepository _surveyRepository;

        public SurveyResponseService(
            ISurveyResponseRepository surveyResponseRepository,
            ISurveyRepository surveyRepository)
        {
            _surveyResponseRepository = surveyResponseRepository;
            _surveyRepository = surveyRepository;
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
            // Validate that the survey exists
            var survey = await _surveyRepository.GetByIdAsync(surveyResponse.SurveyId);
            if (survey == null)
                return false;

            // Set submission timestamp
            surveyResponse.SubmittedAt = DateTime.UtcNow;

            // Validate each answer based on the question type
            ValidateAnswers(survey, surveyResponse);

            // Update survey response count
            survey.ResponseCount++;
            await _surveyRepository.UpdateAsync(survey);

            // Save the response
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

        private void ValidateAnswers(Survey survey, SurveyResponse response)
        {
            foreach (var answer in response.Answers)
            {
                var question = survey.Questions.FirstOrDefault(q => q.Id.ToString() == answer.QuestionId);
                if (question == null)
                {
                    answer.IsValid = false;
                    continue;
                }

                // Basic validation based on question type
                switch (question.Type.ToLower())
                {
                    case "text":
                        answer.IsValid = !string.IsNullOrWhiteSpace(answer.Value);
                        break;
                    case "rating":
                    case "nps":
                        answer.IsValid = !string.IsNullOrWhiteSpace(answer.Value) &&
                                        int.TryParse(answer.Value, out int rating);
                        break;
                    case "multiple-choice":
                    case "single-choice":
                    case "dropdown":
                        answer.IsValid = !string.IsNullOrWhiteSpace(answer.Value) &&
                                        (question.Options?.Contains(answer.Value) ?? false);
                        break;
                    default:
                        answer.IsValid = true;
                        break;
                }
            }
        }
    }
}
