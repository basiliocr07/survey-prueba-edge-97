
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SurveyApp.Application.DTOs;
using SurveyApp.Application.Services;

namespace SurveyApp.WebApi.Controllers
{
    [Route("api/survey-responses")]
    [ApiController]
    public class SurveyResponsesController : ControllerBase
    {
        private readonly ISurveyService _surveyService;

        public SurveyResponsesController(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        // GET: api/survey-responses/{surveyId}
        [HttpGet("{surveyId}")]
        public async Task<ActionResult<IEnumerable<SurveyResponseDto>>> GetSurveyResponses(Guid surveyId)
        {
            try
            {
                var responses = await _surveyService.GetSurveyResponsesAsync(surveyId);
                return Ok(responses);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/survey-responses
        [HttpPost]
        public async Task<ActionResult<SurveyResponseDto>> SubmitSurveyResponse(CreateSurveyResponseDto createResponseDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var response = await _surveyService.SubmitSurveyResponseAsync(createResponseDto);
                return CreatedAtAction(nameof(GetSurveyResponses), new { surveyId = response.SurveyId }, response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
