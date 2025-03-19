
using System;
using System.Threading.Tasks;

namespace SurveyApp.Application.Services
{
    public partial class SurveyService
    {
        public async Task UpdateSurveyStatusAsync(Guid id, string status)
        {
            var survey = await _surveyRepository.GetByIdAsync(id);
            if (survey == null)
                throw new KeyNotFoundException($"Survey with ID {id} not found");

            survey.Status = status;
            await _surveyRepository.UpdateAsync(survey);
        }
    }
}
