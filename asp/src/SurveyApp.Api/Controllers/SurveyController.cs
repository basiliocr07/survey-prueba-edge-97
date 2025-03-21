
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SurveyApp.Application.Services;
using SurveyApp.Domain.Entities;

namespace SurveyApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SurveyController : ControllerBase
    {
        private readonly ISurveyService _surveyService;
        private readonly ILogger<SurveyController> _logger;

        public SurveyController(ISurveyService surveyService, ILogger<SurveyController> logger)
        {
            _surveyService = surveyService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Survey>>> GetAllSurveys()
        {
            try
            {
                var surveys = await _surveyService.GetAllSurveysAsync();
                return Ok(surveys);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all surveys");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Survey>> GetSurveyById(Guid id)
        {
            try
            {
                var survey = await _surveyService.GetSurveyByIdAsync(id);
                return Ok(survey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while getting survey with ID {id}");
                return NotFound($"Survey with ID {id} not found");
            }
        }

        // El método CreateSurvey está duplicado en SurveysController.cs
        /*
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Survey>> CreateSurvey([FromBody] CreateSurveyDto surveyDto)
        {
            try
            {
                var survey = await _surveyService.CreateSurveyAsync(
                    surveyDto.Title,
                    surveyDto.Description,
                    surveyDto.Category);

                return CreatedAtAction(nameof(GetSurveyById), new { id = survey.Id }, survey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a new survey");
                return StatusCode(500, "Internal server error");
            }
        }
        */

        // El método UpdateSurvey está duplicado en SurveysController.cs
        /*
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateSurvey(Guid id, [FromBody] UpdateSurveyDto surveyDto)
        {
            try
            {
                await _surveyService.UpdateSurveyAsync(
                    id,
                    surveyDto.Title,
                    surveyDto.Description,
                    surveyDto.Category);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while updating survey with ID {id}");
                return NotFound($"Survey with ID {id} not found");
            }
        }
        */

        // El método DeleteSurvey está duplicado en SurveysController.cs
        /*
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteSurvey(Guid id)
        {
            try
            {
                await _surveyService.DeleteSurveyAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while deleting survey with ID {id}");
                return NotFound($"Survey with ID {id} not found");
            }
        }
        */

        [HttpPost("{surveyId}/questions")]
        [Authorize]
        public async Task<IActionResult> AddQuestionToSurvey(Guid surveyId, [FromBody] CreateQuestionDto questionDto)
        {
            try
            {
                var question = new Question(
                    questionDto.Title,
                    questionDto.Type,
                    questionDto.Description,
                    questionDto.IsRequired);

                if (questionDto.Options != null && questionDto.Options.Count > 0)
                {
                    question.SetOptions(questionDto.Options);
                }

                await _surveyService.AddQuestionToSurveyAsync(surveyId, question);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while adding question to survey with ID {surveyId}");
                return NotFound($"Survey with ID {surveyId} not found");
            }
        }

        [HttpPost("{id}/publish")]
        [Authorize]
        public async Task<IActionResult> PublishSurvey(Guid id)
        {
            try
            {
                await _surveyService.PublishSurveyAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error occurred while publishing survey with ID {id}");
                return NotFound($"Survey with ID {id} not found");
            }
        }
    }

    public class CreateSurveyDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = "General";
    }

    public class UpdateSurveyDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = "General";
    }

    public class CreateQuestionDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public QuestionType Type { get; set; }
        public bool IsRequired { get; set; }
        public List<string> Options { get; set; } = new List<string>();
    }
}
