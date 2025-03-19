
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
                .Include(r => r.Answers)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<List<SurveyResponse>> GetBySurveyIdAsync(Guid surveyId)
        {
            return await _dbContext.SurveyResponses
                .Include(r => r.Answers)
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
                .Include(r => r.Answers)
                .Where(r => r.SurveyId == surveyId)
                .SelectMany(r => r.Answers)
                .Where(a => a.QuestionId == questionId)
                .ToListAsync();

            foreach (var response in responses)
            {
                if (!string.IsNullOrEmpty(response.Answer))
                {
                    if (results.ContainsKey(response.Answer))
                        results[response.Answer]++;
                    else
                        results[response.Answer] = 1;
                }

                if (response.MultipleAnswers != null && response.MultipleAnswers.Any())
                {
                    foreach (var answer in response.MultipleAnswers)
                    {
                        if (results.ContainsKey(answer))
                            results[answer]++;
                        else
                            results[answer] = 1;
                    }
                }
            }

            return results;
        }
        
        public async Task<List<SurveyResponse>> GetRecentResponsesAsync(int count)
        {
            return await _dbContext.SurveyResponses
                .Include(r => r.Answers)
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
