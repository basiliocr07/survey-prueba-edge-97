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
    <meta charset=""UTF-8"" />
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
    <title>Invitación a Encuesta: {survey.Title}</title>
    <style>
        * {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }}
        body {{
            background-color: #f9fafb;
            line-height: 1.5;
        }}
        .container {{
            max-width: 600px;
            margin: 20px auto;
            background-color: #ffffff;
            border-radius: 8px;
            overflow: hidden;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
        }}
        .header {{
            background-color: #4f46e5;
            color: white;
            padding: 20px;
            text-align: center;
        }}
        .header h1 {{
            font-size: 24px;
            margin-bottom: 5px;
        }}
        .content {{
            padding: 30px 20px;
            color: #374151;
        }}
        .survey-info {{
            background-color: #f3f4f6;
            border-radius: 6px;
            padding: 15px;
            margin: 20px 0;
        }}
        .survey-title {{
            font-size: 18px;
            font-weight: 600;
            color: #111827;
            margin-bottom: 10px;
        }}
        .button-container {{
            text-align: center;
            margin: 30px 0;
        }}
        .button {{
            display: inline-block;
            background-color: #4f46e5;
            color: white;
            text-decoration: none;
            padding: 12px 24px;
            border-radius: 6px;
            font-weight: 600;
            transition: background-color 0.3s;
        }}
        .button:hover {{
            background-color: #4338ca;
        }}
        .survey-link {{
            background-color: #f3f4f6;
            border-radius: 6px;
            padding: 12px;
            margin-top: 20px;
            word-break: break-all;
            font-size: 14px;
            text-align: center;
        }}
        .footer {{
            border-top: 1px solid #e5e7eb;
            padding: 20px;
            text-align: center;
            font-size: 14px;
            color: #6b7280;
        }}
        .logo {{
            max-width: 120px;
            margin-bottom: 10px;
        }}
        @media only screen and (max-width: 550px) {{
            .container {{
                width: 100%;
                margin: 0;
                border-radius: 0;
            }}
            .header h1 {{
                font-size: 20px;
            }}
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>Invitación a Encuesta</h1>
            <p>Tu opinión es importante para nosotros</p>
        </div>
        
        <div class=""content"">
            <p>Hola,</p>
            <p>Te invitamos a participar en nuestra encuesta:</p>
            
            <div class=""survey-info"">
                <div class=""survey-title"">{survey.Title}</div>
                {(string.IsNullOrEmpty(survey.Description) ? "" : $"<p>{survey.Description}</p>")}
            </div>
            
            <p>Tus respuestas nos ayudarán a mejorar nuestros servicios. La encuesta solo tomará unos minutos de tu tiempo.</p>
            
            <div class=""button-container"">
                <a href=""{surveyUrl}"" class=""button"">Comenzar Encuesta</a>
            </div>
            
            <p>O si prefieres, puedes copiar y pegar este enlace en tu navegador:</p>
            <div class=""survey-link"">
                {surveyUrl}
            </div>
        </div>
        
        <div class=""footer"">
            <p>Si recibiste este correo por error, puedes ignorarlo.</p>
            <p>&copy; {DateTime.Now.Year} Sistema de Encuestas</p>
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
