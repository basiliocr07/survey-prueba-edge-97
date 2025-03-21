
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SurveyApp.Application.DTOs;
using SurveyApp.Application.Services;

namespace SurveyApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SurveyResponsesController : ControllerBase
    {
        private readonly ISurveyResponseService _responseService;

        public SurveyResponsesController(ISurveyResponseService responseService)
        {
            _responseService = responseService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SurveyResponseDto>>> GetAllResponses()
        {
            var responses = await _responseService.GetAllResponsesAsync();
            return Ok(responses);
        }

        [HttpGet("survey/{surveyId}")]
        public async Task<ActionResult<IEnumerable<SurveyResponseDto>>> GetResponsesBySurveyId(Guid surveyId)
        {
            var responses = await _responseService.GetResponsesBySurveyIdAsync(surveyId);
            return Ok(responses);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SurveyResponseDto>> GetResponseById(Guid id)
        {
            var response = await _responseService.GetResponseByIdAsync(id);
            if (response == null)
            {
                return NotFound();
            }
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<SurveyResponseDto>> CreateResponse(CreateSurveyResponseDto responseDto)
        {
            try
            {
                var createdResponse = await _responseService.CreateResponseAsync(responseDto);
                return CreatedAtAction(nameof(GetResponseById), new { id = createdResponse.Id }, createdResponse);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
