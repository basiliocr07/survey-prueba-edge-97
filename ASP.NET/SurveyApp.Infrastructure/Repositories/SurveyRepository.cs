
using Microsoft.EntityFrameworkCore;
using SurveyApp.Domain.Models;
using SurveyApp.Domain.Repositories;
using SurveyApp.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SurveyApp.Infrastructure.Repositories
{
    public class SurveyRepository : ISurveyRepository
    {
        private readonly AppDbContext _context;

        public SurveyRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Survey>> GetAllAsync()
        {
            return await _context.Surveys
                .Include(s => s.Questions)
                .ThenInclude(q => q.Settings)
                .Include(s => s.DeliveryConfig)
                .ThenInclude(d => d.Schedule)
                .Include(s => s.DeliveryConfig)
                .ThenInclude(d => d.Trigger)
                .ToListAsync();
        }

        public async Task<Survey?> GetByIdAsync(int id)
        {
            return await _context.Surveys
                .Include(s => s.Questions)
                .ThenInclude(q => q.Settings)
                .Include(s => s.DeliveryConfig)
                .ThenInclude(d => d.Schedule)
                .Include(s => s.DeliveryConfig)
                .ThenInclude(d => d.Trigger)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<bool> AddAsync(Survey survey)
        {
            if (survey.CreatedAt == default)
            {
                survey.CreatedAt = DateTime.Now;
            }
            
            _context.Surveys.Add(survey);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(Survey survey)
        {
            // Para asegurar que Entity Framework rastree los cambios en las colecciones relacionadas
            var existingSurvey = await _context.Surveys
                .Include(s => s.Questions)
                .ThenInclude(q => q.Settings)
                .Include(s => s.DeliveryConfig)
                .ThenInclude(d => d.Schedule)
                .Include(s => s.DeliveryConfig)
                .ThenInclude(d => d.Trigger)
                .FirstOrDefaultAsync(s => s.Id == survey.Id);

            if (existingSurvey == null)
                return false;

            // Actualizar propiedades principales
            _context.Entry(existingSurvey).CurrentValues.SetValues(survey);

            // Actualizar Questions
            UpdateQuestions(existingSurvey, survey);

            // Actualizar DeliveryConfig
            UpdateDeliveryConfig(existingSurvey, survey);

            return await _context.SaveChangesAsync() > 0;
        }

        private void UpdateQuestions(Survey existingSurvey, Survey updatedSurvey)
        {
            // Eliminar preguntas que ya no existen
            existingSurvey.Questions.RemoveAll(q => !updatedSurvey.Questions.Any(uq => uq.Id == q.Id));

            foreach (var updatedQuestion in updatedSurvey.Questions)
            {
                var existingQuestion = existingSurvey.Questions.FirstOrDefault(q => q.Id == updatedQuestion.Id);
                
                if (existingQuestion == null)
                {
                    // Nueva pregunta
                    existingSurvey.Questions.Add(updatedQuestion);
                }
                else
                {
                    // Actualizar pregunta existente
                    _context.Entry(existingQuestion).CurrentValues.SetValues(updatedQuestion);
                    
                    // Actualizar Settings si existe
                    if (updatedQuestion.Settings != null)
                    {
                        if (existingQuestion.Settings == null)
                        {
                            existingQuestion.Settings = updatedQuestion.Settings;
                        }
                        else
                        {
                            _context.Entry(existingQuestion.Settings).CurrentValues.SetValues(updatedQuestion.Settings);
                        }
                    }
                    else
                    {
                        existingQuestion.Settings = null;
                    }

                    // Actualizar Options
                    existingQuestion.Options = updatedQuestion.Options;
                }
            }
        }

        private void UpdateDeliveryConfig(Survey existingSurvey, Survey updatedSurvey)
        {
            if (updatedSurvey.DeliveryConfig == null)
            {
                existingSurvey.DeliveryConfig = null;
                return;
            }

            if (existingSurvey.DeliveryConfig == null)
            {
                existingSurvey.DeliveryConfig = updatedSurvey.DeliveryConfig;
                return;
            }

            // Actualizar propiedades principales de DeliveryConfig
            _context.Entry(existingSurvey.DeliveryConfig).CurrentValues.SetValues(updatedSurvey.DeliveryConfig);

            // Actualizar Schedule
            if (updatedSurvey.DeliveryConfig.Schedule != null)
            {
                if (existingSurvey.DeliveryConfig.Schedule == null)
                {
                    existingSurvey.DeliveryConfig.Schedule = updatedSurvey.DeliveryConfig.Schedule;
                }
                else
                {
                    _context.Entry(existingSurvey.DeliveryConfig.Schedule).CurrentValues.SetValues(updatedSurvey.DeliveryConfig.Schedule);
                }
            }
            else
            {
                existingSurvey.DeliveryConfig.Schedule = null;
            }

            // Actualizar Trigger
            if (updatedSurvey.DeliveryConfig.Trigger != null)
            {
                if (existingSurvey.DeliveryConfig.Trigger == null)
                {
                    existingSurvey.DeliveryConfig.Trigger = updatedSurvey.DeliveryConfig.Trigger;
                }
                else
                {
                    _context.Entry(existingSurvey.DeliveryConfig.Trigger).CurrentValues.SetValues(updatedSurvey.DeliveryConfig.Trigger);
                }
            }
            else
            {
                existingSurvey.DeliveryConfig.Trigger = null;
            }

            // Actualizar EmailAddresses
            existingSurvey.DeliveryConfig.EmailAddresses = updatedSurvey.DeliveryConfig.EmailAddresses;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var survey = await _context.Surveys.FindAsync(id);
            if (survey == null)
                return false;
                
            _context.Surveys.Remove(survey);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Survey>> GetByStatusAsync(string status)
        {
            return await _context.Surveys
                .Include(s => s.Questions)
                .ThenInclude(q => q.Settings)
                .Where(s => s.Status == status)
                .ToListAsync();
        }

        public async Task<SurveyStatistics> GetStatisticsAsync(int surveyId)
        {
            var survey = await _context.Surveys.FindAsync(surveyId);
            if (survey == null)
                return new SurveyStatistics { SurveyId = surveyId };

            var responses = await _context.SurveyResponses
                .Where(r => r.SurveyId == surveyId)
                .Include(r => r.Answers)
                .ToListAsync();

            var stats = new SurveyStatistics
            {
                SurveyId = surveyId,
                TotalResponses = responses.Count,
                CompletionRate = responses.Count > 0 ? 100 : 0, // Simplificado para este ejemplo
                AverageCompletionTime = responses.Count > 0 ? responses.Average(r => r.CompletionTime ?? 0) : 0,
                StartDate = survey.CreatedAt,
                EndDate = null
            };

            // Obtener estadísticas por pregunta
            var fullSurvey = await GetByIdAsync(surveyId);
            if (fullSurvey?.Questions != null)
            {
                foreach (var question in fullSurvey.Questions)
                {
                    var questionStat = new QuestionStatistic
                    {
                        QuestionId = question.Id.ToString(),
                        QuestionTitle = question.Text,
                        Responses = new List<AnswerStatistic>()
                    };

                    var questionResponses = responses
                        .SelectMany(r => r.Answers)
                        .Where(a => a.QuestionId == question.Id.ToString())
                        .ToList();

                    var answerCounts = new Dictionary<string, int>();
                    foreach (var answer in questionResponses)
                    {
                        if (answerCounts.ContainsKey(answer.Value))
                            answerCounts[answer.Value]++;
                        else
                            answerCounts[answer.Value] = 1;
                    }

                    foreach (var answer in answerCounts)
                    {
                        questionStat.Responses.Add(new AnswerStatistic
                        {
                            Answer = answer.Key,
                            Count = answer.Value,
                            Percentage = questionResponses.Count > 0 
                                ? (double)answer.Value / questionResponses.Count * 100 
                                : 0
                        });
                    }

                    stats.QuestionStats.Add(questionStat);
                }
            }

            return stats;
        }

        public async Task<bool> SendEmailsAsync(int surveyId, List<string> emailAddresses)
        {
            // En una implementación real, esto enviaría correos electrónicos a través de un servicio
            // Para este ejemplo, solo verificamos que la encuesta exista
            var survey = await _context.Surveys.FindAsync(surveyId);
            return survey != null && emailAddresses.Count > 0;
        }
    }
}
