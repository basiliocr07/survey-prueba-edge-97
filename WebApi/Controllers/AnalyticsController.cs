
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
    }
}
