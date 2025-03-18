
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
    public class SurveysController : ControllerBase
    {
        private readonly ISurveyService _surveyService;

        public SurveysController(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        // GET: api/surveys
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SurveyDto>>> GetSurveys()
        {
            var surveys = await _surveyService.GetAllSurveysAsync();
            return Ok(surveys);
        }

        // GET: api/surveys/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<SurveyDto>> GetSurvey(Guid id)
        {
            try
            {
                var survey = await _surveyService.GetSurveyByIdAsync(id);
                return Ok(survey);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // POST: api/surveys
        [HttpPost]
        public async Task<ActionResult<SurveyDto>> CreateSurvey(CreateSurveyDto createSurveyDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdSurvey = await _surveyService.CreateSurveyAsync(createSurveyDto);
                return CreatedAtAction(nameof(GetSurvey), new { id = createdSurvey.Id }, createdSurvey);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/surveys/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSurvey(Guid id, CreateSurveyDto updateSurveyDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _surveyService.UpdateSurveyAsync(id, updateSurveyDto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // DELETE: api/surveys/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSurvey(Guid id)
        {
            try
            {
                await _surveyService.DeleteSurveyAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/surveys/{id}/send
        [HttpPost("{id}/send")]
        public async Task<IActionResult> SendSurveyEmails(Guid id)
        {
            try
            {
                await _surveyService.SendSurveyEmailsAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // PUT: api/surveys/{id}/status
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] string status)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return BadRequest("El estado no puede estar vacío");
            }

            try
            {
                await _surveyService.UpdateSurveyStatusAsync(id, status);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
