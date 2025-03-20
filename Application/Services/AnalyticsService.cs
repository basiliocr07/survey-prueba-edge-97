using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SurveyApp.Application.DTOs;
using SurveyApp.Application.Ports;
using SurveyApp.Domain.Entities;

namespace SurveyApp.Application.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IAnalyticsRepository _analyticsRepository;
        private readonly ISurveyRepository _surveyRepository;
        private readonly ISurveyResponseRepository _surveyResponseRepository;

        public AnalyticsService(
            IAnalyticsRepository analyticsRepository, 
            ISurveyRepository surveyRepository,
            ISurveyResponseRepository surveyResponseRepository)
        {
            _analyticsRepository = analyticsRepository;
            _surveyRepository = surveyRepository;
            _surveyResponseRepository = surveyResponseRepository;
        }

        public async Task<AnalyticsDto> GetAnalyticsDataAsync()
        {
            var analyticsData = await _analyticsRepository.GetAnalyticsDataAsync();
            var random = new Random();
            
            var dto = new AnalyticsDto
            {
                TotalSurveys = analyticsData.TotalSurveys,
                TotalResponses = analyticsData.TotalResponses,
                AverageCompletionRate = analyticsData.AverageCompletionRate,
                QuestionTypeDistribution = analyticsData.QuestionTypeDistribution,
                ResponseTrends = analyticsData.ResponseTrends.Select(rt => new SurveyResponseTrendDto
                {
                    Date = rt.Date,
                    Responses = rt.Responses
                }).ToList(),
                
                // Additional properties with sample data
                SurveyGrowthRate = random.Next(5, 25),
                ResponseGrowthRate = random.Next(5, 30),
                AvgCompletionRate = Math.Round(analyticsData.AverageCompletionRate),
                CompletionRateChange = random.Next(-5, 15),
                AvgResponseTime = random.Next(2, 8),
                ResponseTimeChange = random.Next(-10, 10)
            };
            
            // Add sample survey performance data
            dto.SurveyPerformance = Enumerable.Range(1, 5).Select(i => new SurveyPerformanceDto
            {
                Title = $"Survey {i}",
                ResponseCount = random.Next(50, 200),
                CompletionRate = random.Next(60, 95),
                AverageTimeMinutes = random.Next(2, 10)
            }).ToList();
            
            // Add sample demographic data
            dto.Demographics = new List<DemographicDto>
            {
                new DemographicDto { Category = "18-24", Percentage = random.Next(10, 25) },
                new DemographicDto { Category = "25-34", Percentage = random.Next(20, 35) },
                new DemographicDto { Category = "35-44", Percentage = random.Next(15, 30) },
                new DemographicDto { Category = "45-54", Percentage = random.Next(10, 20) },
                new DemographicDto { Category = "55+", Percentage = random.Next(5, 15) }
            };
            
            // Add sample device distribution data
            dto.DeviceDistribution = new List<DeviceDistributionDto>
            {
                new DeviceDistributionDto { DeviceType = "Desktop", Percentage = random.Next(30, 50) },
                new DeviceDistributionDto { DeviceType = "Mobile", Percentage = random.Next(30, 45) },
                new DeviceDistributionDto { DeviceType = "Tablet", Percentage = random.Next(10, 20) },
                new DeviceDistributionDto { DeviceType = "Other", Percentage = random.Next(2, 8) }
            };
            
            return dto;
        }

        public async Task RefreshAnalyticsDataAsync()
        {
            var surveys = await _surveyRepository.GetAllAsync();
            
            var analyticsData = new AnalyticsData();
            
            // Calculate metrics
            int totalSurveys = surveys.Count;
            int totalResponses = surveys.Sum(s => s.Responses);
            double averageCompletionRate = surveys.Count > 0 
                ? surveys.Average(s => s.CompletionRate) 
                : 0;
            
            analyticsData.UpdateMetrics(totalSurveys, totalResponses, averageCompletionRate);
            
            // Calculate question type distribution
            var questionTypeDistribution = new Dictionary<string, int>();
            foreach (var survey in surveys)
            {
                foreach (var question in survey.Questions)
                {
                    string type = question.Type.ToString();
                    if (questionTypeDistribution.ContainsKey(type))
                    {
                        questionTypeDistribution[type]++;
                    }
                    else
                    {
                        questionTypeDistribution[type] = 1;
                    }
                }
            }
            analyticsData.SetQuestionTypeDistribution(questionTypeDistribution);
            
            // Calculate response trends (simplified)
            var lastSixMonths = Enumerable.Range(0, 6)
                .Select(i => DateTime.UtcNow.AddMonths(-i).ToString("MMM yyyy"))
                .ToList();
            
            var random = new Random();
            foreach (var month in lastSixMonths)
            {
                analyticsData.AddResponseTrend(new SurveyResponseTrend
                {
                    Date = month,
                    Responses = random.Next(50, 200) // Placeholder data
                });
            }
            
            await _analyticsRepository.UpdateAnalyticsDataAsync(analyticsData);
        }

        public async Task<SurveyResponseAnalyticsDto> GetResponseAnalyticsAsync(Guid responseId)
        {
            var response = await _surveyResponseRepository.GetByIdAsync(responseId);
            if (response == null)
            {
                throw new ApplicationException($"Response with ID {responseId} not found");
            }
            
            var survey = await _surveyRepository.GetByIdAsync(response.SurveyId);
            
            // Mapear la respuesta a DTO
            var dto = MapResponseToAnalyticsDto(response, survey?.Title);
            
            // Calcular métricas adicionales
            CalculateResponseMetrics(dto);
            
            return dto;
        }
        
        public async Task<List<SurveyResponseAnalyticsDto>> GetResponsesAnalyticsAsync(Guid? surveyId = null)
        {
            var responses = await _surveyResponseRepository.GetResponsesWithFullDataAsync(surveyId);
            var result = new List<SurveyResponseAnalyticsDto>();
            
            var surveys = await _surveyRepository.GetAllAsync();
            var surveyDict = surveys.ToDictionary(s => s.Id, s => s.Title);
            
            foreach (var response in responses)
            {
                string surveyTitle = surveyDict.ContainsKey(response.SurveyId) ? 
                                  surveyDict[response.SurveyId] : 
                                  "Unknown Survey";
                                  
                var dto = MapResponseToAnalyticsDto(response, surveyTitle);
                CalculateResponseMetrics(dto);
                result.Add(dto);
            }
            
            return result;
        }
        
        public async Task<Dictionary<string, object>> GetSurveyAnalyticsDashboardAsync(Guid surveyId)
        {
            var dashboard = new Dictionary<string, object>();
            
            // Obtener toda la información necesaria para un dashboard completo
            var survey = await _surveyRepository.GetByIdAsync(surveyId);
            if (survey == null)
            {
                throw new ApplicationException($"Survey with ID {surveyId} not found");
            }
            
            var responses = await _surveyResponseRepository.GetBySurveyIdAsync(surveyId);
            var questionTypeDistribution = await _surveyResponseRepository.GetQuestionTypeDistributionAsync(surveyId);
            var completionRateByType = await _surveyResponseRepository.GetCompletionRateByQuestionTypeAsync(surveyId);
            var averageScoreByCategory = await _surveyResponseRepository.GetAverageScoreByCategoryAsync(surveyId);
            var npsDistribution = await _surveyResponseRepository.GetNPSDistributionAsync(surveyId);
            var ratingDistribution = await _surveyResponseRepository.GetRatingDistributionAsync(surveyId);
            
            // Calcular métricas clave
            double avgCompletionTime = responses.Any() ? 
                                   responses.Average(r => r.CompletionTime) : 0;
                                   
            int totalAnswers = responses.Sum(r => r.Answers.Count);
            int validAnswers = responses.Sum(r => r.Answers.Count(a => a.IsValid));
            double validationRate = totalAnswers > 0 ? 
                                (double)validAnswers / totalAnswers * 100 : 0;
                                
            // Organizar los datos para el dashboard
            dashboard["surveyTitle"] = survey.Title;
            dashboard["responseCount"] = responses.Count;
            dashboard["averageCompletionTime"] = avgCompletionTime;
            dashboard["validationRate"] = validationRate;
            dashboard["questionTypeDistribution"] = questionTypeDistribution;
            dashboard["completionRateByQuestionType"] = completionRateByType;
            dashboard["averageScoreByCategory"] = averageScoreByCategory;
            dashboard["npsDistribution"] = npsDistribution;
            dashboard["ratingDistribution"] = ratingDistribution;
            
            // Datos para gráficos de tendencias
            var responseTrends = responses
                .GroupBy(r => r.SubmittedAt.Date)
                .OrderBy(g => g.Key)
                .Select(g => new { Date = g.Key.ToString("yyyy-MM-dd"), Count = g.Count() })
                .ToList();
                
            dashboard["responseTrends"] = responseTrends;
            
            return dashboard;
        }
        
        private SurveyResponseAnalyticsDto MapResponseToAnalyticsDto(SurveyResponse response, string surveyTitle)
        {
            var dto = new SurveyResponseAnalyticsDto
            {
                Id = response.Id,
                SurveyId = response.SurveyId,
                SurveyTitle = surveyTitle ?? "Unknown Survey",
                RespondentName = response.RespondentName,
                RespondentEmail = response.RespondentEmail,
                RespondentPhone = response.RespondentPhone,
                RespondentCompany = response.RespondentCompany,
                SubmittedAt = response.SubmittedAt,
                CompletionTime = response.CompletionTime,
                DeviceType = response.DeviceType,
                Browser = response.Browser,
                OperatingSystem = response.OperatingSystem,
                Location = response.Location,
                IpAddress = response.IpAddress,
                Source = response.Source,
                UserAgent = response.UserAgent,
                WasAbandoned = response.WasAbandoned,
                PageViews = response.PageViews,
                Answers = response.Answers.Select(a => new QuestionAnswerDto
                {
                    QuestionId = a.QuestionId,
                    QuestionTitle = a.QuestionTitle,
                    QuestionType = a.QuestionType,
                    Answer = a.Answer,
                    MultipleAnswers = a.MultipleAnswers,
                    IsValid = a.IsValid,
                    ScoreValue = a.ScoreValue,
                    CompletionTimeSeconds = a.CompletionTimeSeconds,
                    Category = a.Category,
                    IsSkipped = string.IsNullOrEmpty(a.Answer) && (a.MultipleAnswers == null || !a.MultipleAnswers.Any()),
                    CharacterCount = a.Answer?.Length ?? 0,
                    FormattedCompletionTime = FormatTimeSpan(TimeSpan.FromSeconds(a.CompletionTimeSeconds))
                }).ToList()
            };
            
            return dto;
        }
        
        private void CalculateResponseMetrics(SurveyResponseAnalyticsDto dto)
        {
            // Calcular métricas básicas
            dto.QuestionCount = dto.Answers.Count;
            dto.ValidAnswersCount = dto.Answers.Count(a => a.IsValid);
            dto.ValidationRate = dto.QuestionCount > 0 ? 
                            (double)dto.ValidAnswersCount / dto.QuestionCount * 100 : 0;
            dto.FormattedCompletionTime = FormatTimeSpan(TimeSpan.FromSeconds(dto.CompletionTime));
            dto.IsValidated = dto.ValidationRate > 95; // Consideramos validado si más del 95% de respuestas son válidas
            dto.AverageTimePerQuestion = dto.Answers.Any() ? dto.CompletionTime / dto.Answers.Count : 0;
            
            // Calcular distribuciones
            dto.QuestionTypeDistribution = dto.Answers
                .GroupBy(a => a.QuestionType)
                .ToDictionary(g => g.Key, g => g.Count());
                
            dto.CompletionRateByQuestionType = dto.Answers
                .GroupBy(a => a.QuestionType)
                .ToDictionary(g => g.Key, g => g.Count(a => !a.IsSkipped) / (double)g.Count() * 100);
                
            dto.AverageScoreByCategory = dto.Answers
                .Where(a => !string.IsNullOrEmpty(a.Category) && a.ScoreValue > 0)
                .GroupBy(a => a.Category)
                .ToDictionary(g => g.Key, g => g.Average(a => a.ScoreValue));
        }
        
        private string FormatTimeSpan(TimeSpan timeSpan)
        {
            if (timeSpan.TotalMinutes < 1)
                return $"{timeSpan.Seconds}s";
            if (timeSpan.TotalHours < 1)
                return $"{timeSpan.Minutes}m {timeSpan.Seconds}s";
            
            return $"{(int)timeSpan.TotalHours}h {timeSpan.Minutes}m";
        }
    }
}
