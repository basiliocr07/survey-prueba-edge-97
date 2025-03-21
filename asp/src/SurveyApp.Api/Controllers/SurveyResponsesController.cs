
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SurveyApp.Application.DTOs;
using SurveyApp.Application.Services;

namespace SurveyApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SurveyResponsesController : ControllerBase
    {
        private readonly ISurveyResponseService _responseService;
        private readonly ILogger<SurveyResponsesController> _logger;

        public SurveyResponsesController(ISurveyResponseService responseService, ILogger<SurveyResponsesController> logger)
        {
            _responseService = responseService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SurveyResponseDto>>> GetAllResponses()
        {
            try
            {
                var responses = await _responseService.GetAllResponsesAsync();
                return Ok(responses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all survey responses");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("survey/{surveyId}")]
        public async Task<ActionResult<IEnumerable<SurveyResponseDto>>> GetResponsesBySurveyId(Guid surveyId)
        {
            try
            {
                var responses = await _responseService.GetResponsesBySurveyIdAsync(surveyId);
                return Ok(responses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while getting responses for survey with ID {surveyId}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SurveyResponseDto>> GetResponseById(Guid id)
        {
            try
            {
                var response = await _responseService.GetResponseByIdAsync(id);
                if (response == null)
                {
                    return NotFound($"Response with ID {id} not found");
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while getting response with ID {id}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<SurveyResponseDto>> CreateResponse([FromBody] CreateSurveyResponseDto responseDto)
        {
            try
            {
                var response = await _responseService.CreateResponseAsync(responseDto);
                return CreatedAtAction(nameof(GetResponseById), new { id = response.Id }, response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a new survey response");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("analytics/{surveyId}")]
        public async Task<ActionResult<SurveyResponseAnalyticsDto>> GetResponseAnalytics(Guid surveyId)
        {
            try
            {
                var analytics = await _responseService.GetResponseAnalyticsAsync(surveyId);
                return Ok(analytics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while getting analytics for survey with ID {surveyId}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
