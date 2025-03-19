
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SurveyApp.Application.DTOs;
using SurveyApp.Application.Services;

namespace SurveyApp.WebApi.Controllers
{
    [Route("api/survey-responses")]
    [ApiController]
    public class SurveyResponsesController : ControllerBase
    {
        private readonly ISurveyService _surveyService;
        private readonly ICustomerService _customerService;
        private readonly ILogger<SurveyResponsesController> _logger;

        public SurveyResponsesController(
            ISurveyService surveyService,
            ICustomerService customerService,
            ILogger<SurveyResponsesController> logger)
        {
            _surveyService = surveyService;
            _customerService = customerService;
            _logger = logger;
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
                _logger.LogWarning(ex, "Encuesta no encontrada al obtener respuestas. SurveyId: {SurveyId}", surveyId);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener respuestas de encuesta. SurveyId: {SurveyId}", surveyId);
                return BadRequest(new { message = ex.Message });
            }
        }

        // POST: api/survey-responses
        [HttpPost]
        public async Task<ActionResult<SurveyResponseDto>> SubmitSurveyResponse(CreateSurveyResponseDto createResponseDto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Modelo inválido al enviar respuesta de encuesta. Errores: {@Errors}", 
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return BadRequest(ModelState);
            }

            try
            {
                // Validar que la encuesta existe
                if (createResponseDto.SurveyId == Guid.Empty)
                {
                    return BadRequest(new { message = "El ID de la encuesta es requerido." });
                }

                // Validar que hay respuestas
                if (createResponseDto.Answers == null || createResponseDto.Answers.Count == 0)
                {
                    return BadRequest(new { message = "No se han proporcionado respuestas." });
                }

                // Validar que el respondente proporcionó su nombre y email
                if (string.IsNullOrWhiteSpace(createResponseDto.RespondentName))
                {
                    return BadRequest(new { message = "El nombre del respondente es requerido." });
                }

                if (string.IsNullOrWhiteSpace(createResponseDto.RespondentEmail))
                {
                    return BadRequest(new { message = "El email del respondente es requerido." });
                }

                // Verificar si el respondente es un cliente existente o crear uno nuevo
                if (createResponseDto.IsExistingClient && createResponseDto.ExistingClientId.HasValue)
                {
                    // Verificar que el cliente existe
                    try
                    {
                        var existingCustomer = await _customerService.GetCustomerByIdAsync(createResponseDto.ExistingClientId.Value);
                        // Actualizar la información del cliente si es necesario
                        if (existingCustomer != null)
                        {
                            _logger.LogInformation("Cliente existente encontrado: {CustomerId}", existingCustomer.Id);
                        }
                    }
                    catch (KeyNotFoundException)
                    {
                        _logger.LogWarning("Cliente no encontrado con ID: {CustomerId}", createResponseDto.ExistingClientId.Value);
                        return BadRequest(new { message = "El cliente seleccionado no existe." });
                    }
                }
                else
                {
                    // Si el cliente no existe y proporcionó datos de empresa, crearlo
                    if (!string.IsNullOrWhiteSpace(createResponseDto.RespondentCompany))
                    {
                        try
                        {
                            var newCustomer = new CreateCustomerDto
                            {
                                BrandName = createResponseDto.RespondentCompany,
                                ContactEmail = createResponseDto.RespondentEmail,
                                ContactPhone = createResponseDto.RespondentPhone,
                                ContactName = createResponseDto.RespondentName,
                                AcquiredServices = new List<string> { "Survey" }
                            };
                            
                            var createdCustomer = await _customerService.CreateCustomerAsync(newCustomer);
                            _logger.LogInformation("Nuevo cliente creado: {CustomerId}", createdCustomer.Id);
                            
                            // Asignar el ID del cliente creado a la respuesta
                            createResponseDto.ExistingClientId = createdCustomer.Id;
                            createResponseDto.IsExistingClient = true;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error al crear un nuevo cliente");
                            // No falla el proceso si no se puede crear el cliente
                        }
                    }
                }

                // Guardar la respuesta en la base de datos
                var response = await _surveyService.SubmitSurveyResponseAsync(createResponseDto);
                
                _logger.LogInformation("Respuesta de encuesta API guardada exitosamente. SurveyId: {SurveyId}, Email: {Email}", 
                    createResponseDto.SurveyId, createResponseDto.RespondentEmail);
                
                return CreatedAtAction(nameof(GetSurveyResponses), new { surveyId = response.SurveyId }, response);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Encuesta no encontrada al enviar respuesta API. SurveyId: {SurveyId}", createResponseDto.SurveyId);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar respuesta de encuesta API. SurveyId: {SurveyId}", createResponseDto.SurveyId);
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
