
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SurveyApp.Application.Ports;
using SurveyApp.Domain.Entities;
using SurveyApp.Infrastructure.Data;

namespace SurveyApp.Infrastructure.Repositories
{
    public class SurveyResponseRepository : ISurveyResponseRepository
    {
        private readonly AppDbContext _dbContext;

        public SurveyResponseRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<SurveyResponse> GetByIdAsync(Guid id)
        {
            return await _dbContext.SurveyResponses
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<List<SurveyResponse>> GetBySurveyIdAsync(Guid surveyId)
        {
            return await _dbContext.SurveyResponses
                .Where(r => r.SurveyId == surveyId)
                .ToListAsync();
        }

        public async Task<SurveyResponse> CreateAsync(SurveyResponse response)
        {
            // Validar las respuestas antes de guardar
            ValidateResponses(response);
            
            await _dbContext.SurveyResponses.AddAsync(response);
            await _dbContext.SaveChangesAsync();
            return response;
        }

        public async Task<int> GetResponseCountForSurveyAsync(Guid surveyId)
        {
            return await _dbContext.SurveyResponses
                .CountAsync(r => r.SurveyId == surveyId);
        }

        public async Task<Dictionary<string, int>> GetQuestionResponseStatisticsAsync(Guid surveyId, Guid questionId)
        {
            var results = new Dictionary<string, int>();
            
            // Obtener todas las respuestas para la pregunta específica
            var responses = await _dbContext.SurveyResponses
                .Where(r => r.SurveyId == surveyId)
                .ToListAsync();

            // Procesar las respuestas para obtener estadísticas
            foreach (var response in responses)
            {
                var questionResponses = response.Answers.Where(a => a.QuestionId == questionId).ToList();
                
                foreach (var qResponse in questionResponses)
                {
                    if (!string.IsNullOrEmpty(qResponse.Answer))
                    {
                        if (results.ContainsKey(qResponse.Answer))
                            results[qResponse.Answer]++;
                        else
                            results[qResponse.Answer] = 1;
                    }

                    if (qResponse.MultipleAnswers != null && qResponse.MultipleAnswers.Any())
                    {
                        foreach (var answer in qResponse.MultipleAnswers)
                        {
                            if (results.ContainsKey(answer))
                                results[answer]++;
                            else
                                results[answer] = 1;
                        }
                    }
                }
            }

            return results;
        }
        
        public async Task<List<SurveyResponse>> GetRecentResponsesAsync(int count)
        {
            return await _dbContext.SurveyResponses
                .OrderByDescending(r => r.SubmittedAt)
                .Take(count)
                .ToListAsync();
        }
        
        // Método privado para validar las respuestas antes de guardar
        private void ValidateResponses(SurveyResponse response)
        {
            foreach (var answer in response.Answers)
            {
                bool isValid = true;
                
                // Validación básica según el tipo de pregunta
                switch (answer.QuestionType)
                {
                    case "text":
                    case "textarea":
                        // Validar que respuestas de texto no estén vacías
                        isValid = !string.IsNullOrWhiteSpace(answer.Answer);
                        break;
                        
                    case "email":
                        // Validar formato de email básico
                        isValid = !string.IsNullOrWhiteSpace(answer.Answer) && 
                                  answer.Answer.Contains("@") && 
                                  answer.Answer.Contains(".");
                        break;
                        
                    case "number":
                        // Validar que sea un número
                        isValid = !string.IsNullOrWhiteSpace(answer.Answer) && 
                                  double.TryParse(answer.Answer, out _);
                        break;
                        
                    case "single-choice":
                    case "dropdown":
                        // Validar que haya una respuesta seleccionada
                        isValid = !string.IsNullOrWhiteSpace(answer.Answer);
                        break;
                        
                    case "multiple-choice":
                        // Validar que haya al menos una opción seleccionada
                        isValid = answer.MultipleAnswers != null && answer.MultipleAnswers.Any();
                        break;
                        
                    case "rating":
                    case "nps":
                        // Validar que sea un número dentro de un rango razonable
                        isValid = !string.IsNullOrWhiteSpace(answer.Answer) && 
                                  int.TryParse(answer.Answer, out int rating) &&
                                  rating >= 0 && rating <= 10;
                        break;
                }
                
                // Establecer el estado de validación
                answer.SetValidationStatus(isValid);
            }
        }
    }
}
