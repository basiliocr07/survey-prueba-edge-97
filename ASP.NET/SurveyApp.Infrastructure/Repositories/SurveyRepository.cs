using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SurveyApp.Domain.Models;
using SurveyApp.Domain.Repositories;
using SurveyApp.Domain.Services;
using SurveyApp.Infrastructure.Data;
using SurveyApp.Infrastructure.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace SurveyApp.Infrastructure.Repositories
{
    public class SurveyRepository : ISurveyRepository
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public SurveyRepository(AppDbContext context, IConfiguration configuration, IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
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

            _context.Entry(existingSurvey).CurrentValues.SetValues(survey);

            UpdateQuestions(existingSurvey, survey);

            UpdateDeliveryConfig(existingSurvey, survey);

            return await _context.SaveChangesAsync() > 0;
        }

        private void UpdateQuestions(Survey existingSurvey, Survey updatedSurvey)
        {
            existingSurvey.Questions.RemoveAll(q => !updatedSurvey.Questions.Any(uq => uq.Id == q.Id));

            foreach (var updatedQuestion in updatedSurvey.Questions)
            {
                var existingQuestion = existingSurvey.Questions.FirstOrDefault(q => q.Id == updatedQuestion.Id);
                
                if (existingQuestion == null)
                {
                    existingSurvey.Questions.Add(updatedQuestion);
                }
                else
                {
                    _context.Entry(existingQuestion).CurrentValues.SetValues(updatedQuestion);
                    
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

            _context.Entry(existingSurvey.DeliveryConfig).CurrentValues.SetValues(updatedSurvey.DeliveryConfig);

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
                CompletionRate = responses.Count > 0 ? 100 : 0,
                AverageCompletionTime = responses.Count > 0 ? responses.Average(r => r.CompletionTime ?? 0) : 0,
                StartDate = survey.CreatedAt,
                EndDate = null
            };

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
            try
            {
                var survey = await GetByIdAsync(surveyId);
                if (survey == null)
                {
                    return false;
                }

                var frontendUrl = _configuration["FrontendUrl"] ?? "http://localhost:5173";
                var surveyUrl = $"{frontendUrl}/take-survey/{surveyId}";

                string htmlContent = $@"
<!DOCTYPE html>
<html>
<head>
  <style>
    body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
    .header {{ background-color: #4a56e2; color: white; padding: 15px; border-radius: 5px; margin-bottom: 20px; }}
    .button {{ display: inline-block; background-color: #4a56e2; color: white; padding: 10px 20px; text-decoration: none; border-radius: 5px; }}
    .footer {{ margin-top: 30px; font-size: 12px; color: #666; }}
  </style>
</head>
<body>
  <div class=""container"">
    <div class=""header"">
      <h2>Te han invitado a responder una encuesta</h2>
    </div>
    
    <p>Hola,</p>
    <p>Has sido invitado a participar en la encuesta: <strong>{survey.Title}</strong></p>
    
    {(string.IsNullOrEmpty(survey.Description) ? "" : $"<p>{survey.Description}</p>")}
    
    <p>Tu opinión es muy importante para nosotros. Por favor haz clic en el botón de abajo para comenzar la encuesta:</p>
    
    <p><a href=""{surveyUrl}"" class=""button"">Responder Encuesta</a></p>
    
    <p>O copia y pega este enlace en tu navegador: {surveyUrl}</p>
    
    <div class=""footer"">
      <p>Si recibiste este correo por error, por favor ignóralo.</p>
    </div>
  </div>
</body>
</html>";

                var success = await _emailService.SendBulkEmailAsync(
                    emailAddresses, 
                    $"Encuesta: {survey.Title}", 
                    htmlContent
                );

                var emailLog = new
                {
                    SurveyId = surveyId,
                    Recipients = JsonSerializer.Serialize(emailAddresses),
                    Status = success ? "sent" : "failed",
                    ErrorMessage = success ? null : "Fallo en el envío de correos",
                    CreatedAt = DateTime.UtcNow
                };

                var sql = @"
                INSERT INTO SurveyEmailLogs (SurveyId, Recipients, Status, ErrorMessage, CreatedAt)
                VALUES (@SurveyId, @Recipients, @Status, @ErrorMessage, @CreatedAt)";

                using var connection = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));
                await connection.ExecuteAsync(sql, emailLog);

                return success;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending survey emails: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateDeliveryConfigAsync(int surveyId, DeliveryConfiguration config)
        {
            var survey = await GetByIdAsync(surveyId);
            if (survey == null)
                return false;
                
            survey.DeliveryConfig = config;
            return await UpdateAsync(survey);
        }
    }
}
