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

        public async Task<Dictionary<string, object>> GetGlobalAnalyticsDashboardAsync()
        {
            var dashboard = new Dictionary<string, object>();

            // Obtener datos globales necesarios para el dashboard
            var totalSurveys = await _surveyRepository.GetTotalSurveyCountAsync();
            var totalResponses = await _surveyResponseRepository.GetTotalResponseCountAsync();
            var deviceDistribution = await _surveyResponseRepository.GetDeviceDistributionAsync();
            var browserDistribution = await _surveyResponseRepository.GetBrowserDistributionAsync();
            var osDistribution = await _surveyResponseRepository.GetOperatingSystemDistributionAsync();
            var locationDistribution = await _surveyResponseRepository.GetLocationDistributionAsync();
            var sourceDistribution = await _surveyResponseRepository.GetSourceDistributionAsync();

            // Calcular métricas clave
            double avgCompletionTime = await _surveyResponseRepository.GetAverageCompletionTimeAsync(Guid.Empty); // No se usa surveyId para global
            double abandonmentRate = await _surveyResponseRepository.GetAbandonmentRateAsync(Guid.Empty); // No se usa surveyId para global

            // Organizar los datos para el dashboard
            dashboard["totalSurveys"] = totalSurveys;
            dashboard["totalResponses"] = totalResponses;
            dashboard["averageCompletionTime"] = FormatTimeSpan(TimeSpan.FromSeconds(avgCompletionTime));
            dashboard["abandonmentRate"] = abandonmentRate;
            dashboard["deviceDistribution"] = deviceDistribution;
            dashboard["browserDistribution"] = browserDistribution;
            dashboard["osDistribution"] = osDistribution;
            dashboard["locationDistribution"] = locationDistribution;
            dashboard["sourceDistribution"] = sourceDistribution;

            // Datos para gráficos de tendencias (ejemplo simplificado)
            var responseTrends = await _surveyResponseRepository.GetRecentResponsesAsync(30);
            var trendsData = responseTrends
                .GroupBy(r => r.SubmittedAt.Date)
                .OrderBy(g => g.Key)
                .Select(g => new { Date = g.Key.ToString("yyyy-MM-dd"), Count = g.Count() })
                .ToList();

            dashboard["responseTrends"] = trendsData;

            return dashboard;
        }

        public async Task<Dictionary<string, object>> GetUserEngagementMetricsAsync(Guid? surveyId = null)
        {
            var metrics = new Dictionary<string, object>();

            // Obtener datos de engagement
            var averageCompletionTime = await _surveyResponseRepository.GetAverageCompletionTimeAsync(surveyId ?? Guid.Empty);
            var averageTimePerQuestionType = await _surveyResponseRepository.GetAverageTimePerQuestionTypeAsync(surveyId ?? Guid.Empty);
            var abandonmentCount = await _surveyResponseRepository.GetAbandonmentCountAsync(surveyId ?? Guid.Empty);
            var abandonmentRate = await _surveyResponseRepository.GetAbandonmentRateAsync(surveyId ?? Guid.Empty);
            var pageViewsDistribution = await _surveyResponseRepository.GetPageViewsDistributionAsync(surveyId ?? Guid.Empty);
            var sourceDistribution = await _surveyResponseRepository.GetSourceDistributionAsync(surveyId);

            // Organizar los datos
            metrics["averageCompletionTime"] = FormatTimeSpan(TimeSpan.FromSeconds(averageCompletionTime));
            metrics["averageTimePerQuestionType"] = averageTimePerQuestionType;
            metrics["abandonmentCount"] = abandonmentCount;
            metrics["abandonmentRate"] = abandonmentRate;
            metrics["pageViewsDistribution"] = pageViewsDistribution;
            metrics["sourceDistribution"] = sourceDistribution;

            return metrics;
        }

        public async Task<Dictionary<string, object>> GetDeviceAnalyticsAsync(Guid? surveyId = null)
        {
            var deviceAnalytics = new Dictionary<string, object>();

            // Obtener distribuciones de dispositivos
            var deviceDistribution = await _surveyResponseRepository.GetDeviceDistributionAsync();
            var browserDistribution = await _surveyResponseRepository.GetBrowserDistributionAsync(surveyId);
            var osDistribution = await _surveyResponseRepository.GetOperatingSystemDistributionAsync(surveyId);
            var locationDistribution = await _surveyResponseRepository.GetLocationDistributionAsync(surveyId);

            // Organizar los datos
            deviceAnalytics["deviceDistribution"] = deviceDistribution;
            deviceAnalytics["browserDistribution"] = browserDistribution;
            deviceAnalytics["osDistribution"] = osDistribution;
            deviceAnalytics["locationDistribution"] = locationDistribution;

            return deviceAnalytics;
        }

        public async Task<Dictionary<DateTime, int>> GetResponseTrendsAsync(Guid? surveyId = null, string timeRange = "last30days")
        {
            // Calcular fechas de inicio y fin basadas en el rango de tiempo
            DateTime endDate = DateTime.UtcNow.Date;
            DateTime startDate = timeRange.ToLower() switch
            {
                "last7days" => endDate.AddDays(-7),
                "last90days" => endDate.AddDays(-90),
                _ => endDate.AddDays(-30), // Default to last 30 days
            };

            // Obtener tendencias de respuesta
            return await _surveyResponseRepository.GetResponsesOverTimeAsync(surveyId ?? Guid.Empty, startDate, endDate);
        }

        public async Task<Dictionary<string, object>> GetCompletionAnalyticsAsync(Guid surveyId)
        {
            var completionAnalytics = new Dictionary<string, object>();

            // Obtener datos de finalización
            var responses = await _surveyResponseRepository.GetBySurveyIdAsync(surveyId);
            var totalResponses = responses.Count;
            var abandonmentCount = await _surveyResponseRepository.GetAbandonmentCountAsync(surveyId);
            var abandonmentRate = await _surveyResponseRepository.GetAbandonmentRateAsync(surveyId);

            // Calcular métricas de finalización
            double completionRate = totalResponses > 0 ? (double)(totalResponses - abandonmentCount) / totalResponses * 100 : 0;

            // Organizar los datos
            completionAnalytics["totalResponses"] = totalResponses;
            completionAnalytics["completionRate"] = completionRate;
            completionAnalytics["abandonmentCount"] = abandonmentCount;
            completionAnalytics["abandonmentRate"] = abandonmentRate;

            return completionAnalytics;
        }

        public async Task<Dictionary<string, object>> GetQuestionPerformanceAnalyticsAsync(Guid surveyId)
        {
            var questionPerformance = new Dictionary<string, object>();

            // Obtener datos de rendimiento de preguntas
            var survey = await _surveyRepository.GetByIdAsync(surveyId);
            if (survey == null)
            {
                throw new ApplicationException($"Survey with ID {surveyId} not found");
            }

            var responses = await _surveyResponseRepository.GetBySurveyIdAsync(surveyId);
            var totalResponses = responses.Count;

            // Calcular métricas por pregunta
            var questionMetrics = new List<object>();
            foreach (var question in survey.Questions)
            {
                var questionResponses = responses.SelectMany(r => r.Answers).Where(a => a.QuestionId == question.Id).ToList();
                var responseCount = questionResponses.Count;
                var skipCount = questionResponses.Count(qr => string.IsNullOrEmpty(qr.Answer) && (qr.MultipleAnswers == null || !qr.MultipleAnswers.Any()));
                double skipRate = responseCount > 0 ? (double)skipCount / responseCount * 100 : 0;

                questionMetrics.Add(new
                {
                    QuestionId = question.Id,
                    QuestionTitle = question.Title,
                    ResponseCount = responseCount,
                    SkipCount = skipCount,
                    SkipRate = skipRate
                });
            }

            // Organizar los datos
            questionPerformance["totalResponses"] = totalResponses;
            questionPerformance["questionMetrics"] = questionMetrics;

            return questionPerformance;
        }

        public async Task ExportAnalyticsDataAsync(Guid? surveyId, string format, bool includeRawData, string timeRange)
        {
            // Simulación de exportación de datos
            await Task.Delay(1000); // Simular tiempo de procesamiento

            // Aquí iría la lógica real para exportar los datos
            Console.WriteLine($"Exporting analytics data for survey ID: {surveyId}, format: {format}, includeRawData: {includeRawData}, timeRange: {timeRange}");
        }

        public async Task<Dictionary<string, int>> GetBrowserStatisticsAsync(Guid? surveyId = null)
        {
            try
            {
                return await _surveyResponseRepository.GetBrowserDistributionAsync(surveyId);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Error al obtener estadísticas de navegador: {ex.Message}", ex);
            }
        }
    }
}
