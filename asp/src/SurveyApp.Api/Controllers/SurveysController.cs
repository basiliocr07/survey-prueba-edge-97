
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
    public class SurveysController : ControllerBase
    {
        private readonly ISurveyService _surveyService;

        public SurveysController(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SurveyDto>>> GetAllSurveys()
        {
            var surveys = await _surveyService.GetAllSurveysAsync();
            return Ok(surveys);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SurveyDto>> GetSurveyById(Guid id)
        {
            var survey = await _surveyService.GetSurveyByIdAsync(id);
            if (survey == null)
            {
                return NotFound();
            }
            return Ok(survey);
        }

        [HttpPost]
        public async Task<ActionResult<SurveyDto>> CreateSurvey(CreateSurveyDto surveyDto)
        {
            var createdSurvey = await _surveyService.CreateSurveyAsync(surveyDto);
            return CreatedAtAction(nameof(GetSurveyById), new { id = createdSurvey.Id }, createdSurvey);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSurvey(Guid id, CreateSurveyDto surveyDto)
        {
            try
            {
                await _surveyService.UpdateSurveyAsync(id, surveyDto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSurvey(Guid id)
        {
            await _surveyService.DeleteSurveyAsync(id);
            return NoContent();
        }
    }
}
