
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SurveyApp.Application.DTOs;
using SurveyApp.Application.Services;

namespace SurveyApp.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        // GET: api/analytics
        [HttpGet]
        public async Task<ActionResult<AnalyticsDto>> GetAnalyticsData()
        {
            var analyticsData = await _analyticsService.GetAnalyticsDataAsync();
            return Ok(analyticsData);
        }

        // POST: api/analytics/refresh
        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshAnalyticsData()
        {
            await _analyticsService.RefreshAnalyticsDataAsync();
            return NoContent();
        }
        
        // GET: api/analytics/responses/{responseId}
        [HttpGet("responses/{responseId}")]
        public async Task<ActionResult<SurveyResponseAnalyticsDto>> GetResponseAnalytics(Guid responseId)
        {
            try
            {
                var responseAnalytics = await _analyticsService.GetResponseAnalyticsAsync(responseId);
                return Ok(responseAnalytics);
            }
            catch (ApplicationException ex)
            {
                return NotFound(ex.Message);
            }
        }
        
        // GET: api/analytics/responses?surveyId={surveyId}
        [HttpGet("responses")]
        public async Task<ActionResult<List<SurveyResponseAnalyticsDto>>> GetResponsesAnalytics([FromQuery] Guid? surveyId = null)
        {
            var responsesAnalytics = await _analyticsService.GetResponsesAnalyticsAsync(surveyId);
            return Ok(responsesAnalytics);
        }
        
        // GET: api/analytics/surveys/{surveyId}/dashboard
        [HttpGet("surveys/{surveyId}/dashboard")]
        public async Task<ActionResult<Dictionary<string, object>>> GetSurveyAnalyticsDashboard(Guid surveyId)
        {
            try
            {
                var dashboard = await _analyticsService.GetSurveyAnalyticsDashboardAsync(surveyId);
                return Ok(dashboard);
            }
            catch (ApplicationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET: api/analytics/dashboard
        [HttpGet("dashboard")]
        public async Task<ActionResult<Dictionary<string, object>>> GetGlobalAnalyticsDashboard()
        {
            try
            {
                var dashboard = await _analyticsService.GetGlobalAnalyticsDashboardAsync();
                return Ok(dashboard);
            }
            catch (ApplicationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET: api/analytics/engagement
        [HttpGet("engagement")]
        public async Task<ActionResult<Dictionary<string, object>>> GetUserEngagementMetrics([FromQuery] Guid? surveyId = null)
        {
            try
            {
                var metrics = await _analyticsService.GetUserEngagementMetricsAsync(surveyId);
                return Ok(metrics);
            }
            catch (ApplicationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET: api/analytics/devices
        [HttpGet("devices")]
        public async Task<ActionResult<Dictionary<string, object>>> GetDeviceAnalytics([FromQuery] Guid? surveyId = null)
        {
            try
            {
                var deviceAnalytics = await _analyticsService.GetDeviceAnalyticsAsync(surveyId);
                return Ok(deviceAnalytics);
            }
            catch (ApplicationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET: api/analytics/trends?surveyId={surveyId}&timeRange={timeRange}
        [HttpGet("trends")]
        public async Task<ActionResult<Dictionary<DateTime, int>>> GetResponseTrends(
            [FromQuery] Guid? surveyId = null, 
            [FromQuery] string timeRange = "last30days")
        {
            try
            {
                var trends = await _analyticsService.GetResponseTrendsAsync(surveyId, timeRange);
                return Ok(trends);
            }
            catch (ApplicationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET: api/analytics/surveys/{surveyId}/completion
        [HttpGet("surveys/{surveyId}/completion")]
        public async Task<ActionResult<Dictionary<string, object>>> GetCompletionAnalytics(Guid surveyId)
        {
            try
            {
                var completionAnalytics = await _analyticsService.GetCompletionAnalyticsAsync(surveyId);
                return Ok(completionAnalytics);
            }
            catch (ApplicationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // GET: api/analytics/surveys/{surveyId}/questions
        [HttpGet("surveys/{surveyId}/questions")]
        public async Task<ActionResult<Dictionary<string, object>>> GetQuestionPerformanceAnalytics(Guid surveyId)
        {
            try
            {
                var questionAnalytics = await _analyticsService.GetQuestionPerformanceAnalyticsAsync(surveyId);
                return Ok(questionAnalytics);
            }
            catch (ApplicationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // POST: api/analytics/export?surveyId={surveyId}&format={format}&includeRawData={includeRawData}&timeRange={timeRange}
        [HttpPost("export")]
        public async Task<IActionResult> ExportAnalyticsData(
            [FromQuery] Guid? surveyId, 
            [FromQuery] string format = "csv", 
            [FromQuery] bool includeRawData = true, 
            [FromQuery] string timeRange = "all")
        {
            try
            {
                await _analyticsService.ExportAnalyticsDataAsync(surveyId, format, includeRawData, timeRange);
                return Ok(new { message = "Export initiated. File will be emailed when ready." });
            }
            catch (ApplicationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
