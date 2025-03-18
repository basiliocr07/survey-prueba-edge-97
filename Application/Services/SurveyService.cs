
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SurveyApp.Application.DTOs;
using SurveyApp.Application.Ports;
using SurveyApp.Domain.Entities;

namespace SurveyApp.Application.Services
{
    public partial class SurveyService : ISurveyService
    {
        // Implementa este método para actualizar el estado de una encuesta
        public async Task UpdateSurveyStatusAsync(Guid id, string status)
        {
            var survey = await _surveyRepository.GetByIdAsync(id);
            
            if (survey == null)
                throw new KeyNotFoundException($"Encuesta con ID {id} no encontrada.");
                
            survey.SetStatus(status);
            await _surveyRepository.UpdateAsync(survey);
        }
    }
}
