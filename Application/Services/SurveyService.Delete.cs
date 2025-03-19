using System;
using System.Threading.Tasks;

namespace SurveyApp.Application.Services
{
    public partial class SurveyService
    {
        public async Task DeleteSurveyAsync(Guid id)
        {
            var survey = await _surveyRepository.GetByIdAsync(id);
            if (survey == null)
                throw new KeyNotFoundException($"Survey with ID {id} not found");
                
            await _surveyRepository.DeleteAsync(id);
        }
    }
}
