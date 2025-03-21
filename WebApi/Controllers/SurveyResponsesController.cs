
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
            var logReference = Guid.NewGuid().ToString("N").Substring(0, 8);
            _logger.LogInformation("[Ref:{LogRef}] API: Obteniendo respuestas para encuesta {SurveyId}", 
                logReference, surveyId);
            
            try
            {
                var responses = await _surveyService.GetSurveyResponsesAsync(surveyId);
                _logger.LogDebug("[Ref:{LogRef}] API: Obtenidas {Count} respuestas para encuesta {SurveyId}", 
                    logReference, responses.Count, surveyId);
                return Ok(responses);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "[Ref:{LogRef}] API: Encuesta no encontrada al obtener respuestas. SurveyId: {SurveyId}", 
                    logReference, surveyId);
                return NotFound(new { message = ex.Message, logReference = logReference });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Ref:{LogRef}] API: Error al obtener respuestas de encuesta. SurveyId: {SurveyId}", 
                    logReference, surveyId);
                
                // Log additional details for debugging
                _logger.LogDebug("[Ref:{LogRef}] API: Stack trace: {StackTrace}", logReference, ex.StackTrace);
                if (ex.InnerException != null)
                {
                    _logger.LogDebug("[Ref:{LogRef}] API: Inner exception: {InnerExceptionMessage}", 
                        logReference, ex.InnerException.Message);
                }
                
                return BadRequest(new { message = ex.Message, logReference = logReference });
            }
        }

        // POST: api/survey-responses
        [HttpPost]
        public async Task<ActionResult<SurveyResponseDto>> SubmitSurveyResponse(CreateSurveyResponseDto createResponseDto)
        {
            var logReference = Guid.NewGuid().ToString("N").Substring(0, 8);
            _logger.LogInformation("[Ref:{LogRef}] API: Recibida solicitud para enviar respuesta de encuesta. SurveyId: {SurveyId}", 
                logReference, createResponseDto?.SurveyId);
            
            if (!ModelState.IsValid)
            {
                var modelErrors = string.Join("; ", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                
                _logger.LogWarning("[Ref:{LogRef}] API: Modelo inválido al enviar respuesta de encuesta. Errores: {Errors}", 
                    logReference, modelErrors);
                return BadRequest(ModelState);
            }

            try
            {
                // Validar que la encuesta existe
                if (createResponseDto.SurveyId == Guid.Empty)
                {
                    _logger.LogWarning("[Ref:{LogRef}] API: ID de encuesta vacío en la solicitud", logReference);
                    return BadRequest(new { message = "El ID de la encuesta es requerido.", logReference = logReference });
                }

                // Validar que hay respuestas
                if (createResponseDto.Answers == null || createResponseDto.Answers.Count == 0)
                {
                    _logger.LogWarning("[Ref:{LogRef}] API: No se proporcionaron respuestas para la encuesta {SurveyId}", 
                        logReference, createResponseDto.SurveyId);
                    return BadRequest(new { message = "No se han proporcionado respuestas.", logReference = logReference });
                }

                // Validar que el respondente proporcionó su nombre y email
                if (string.IsNullOrWhiteSpace(createResponseDto.RespondentName))
                {
                    _logger.LogWarning("[Ref:{LogRef}] API: Nombre de respondente no proporcionado para encuesta {SurveyId}", 
                        logReference, createResponseDto.SurveyId);
                    return BadRequest(new { message = "El nombre del respondente es requerido.", logReference = logReference });
                }

                if (string.IsNullOrWhiteSpace(createResponseDto.RespondentEmail))
                {
                    _logger.LogWarning("[Ref:{LogRef}] API: Email de respondente no proporcionado para encuesta {SurveyId}", 
                        logReference, createResponseDto.SurveyId);
                    return BadRequest(new { message = "El email del respondente es requerido.", logReference = logReference });
                }

                _logger.LogDebug("[Ref:{LogRef}] API: Validando cliente para respuesta de encuesta. IsExistingClient: {IsExistingClient}, ClientId: {ClientId}", 
                    logReference, createResponseDto.IsExistingClient, createResponseDto.ExistingClientId);
                
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
                            _logger.LogInformation("[Ref:{LogRef}] API: Cliente existente encontrado: {CustomerId}, {CustomerName}", 
                                logReference, existingCustomer.Id, existingCustomer.ContactName);
                        }
                    }
                    catch (KeyNotFoundException)
                    {
                        _logger.LogWarning("[Ref:{LogRef}] API: Cliente no encontrado con ID: {CustomerId}", 
                            logReference, createResponseDto.ExistingClientId.Value);
                        return BadRequest(new { message = "El cliente seleccionado no existe.", logReference = logReference });
                    }
                }
                else
                {
                    // Si el cliente no existe y proporcionó datos de empresa, crearlo
                    if (!string.IsNullOrWhiteSpace(createResponseDto.RespondentCompany))
                    {
                        _logger.LogDebug("[Ref:{LogRef}] API: Intentando crear nuevo cliente con nombre: {CompanyName}, Email: {Email}", 
                            logReference, createResponseDto.RespondentCompany, createResponseDto.RespondentEmail);
                        
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
                            _logger.LogInformation("[Ref:{LogRef}] API: Nuevo cliente creado: {CustomerId} para encuesta {SurveyId}", 
                                logReference, createdCustomer.Id, createResponseDto.SurveyId);
                            
                            // Asignar el ID del cliente creado a la respuesta
                            createResponseDto.ExistingClientId = createdCustomer.Id;
                            createResponseDto.IsExistingClient = true;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "[Ref:{LogRef}] API: Error al crear un nuevo cliente para respuesta de encuesta {SurveyId}", 
                                logReference, createResponseDto.SurveyId);
                            // No falla el proceso si no se puede crear el cliente
                        }
                    }
                }

                _logger.LogDebug("[Ref:{LogRef}] API: Enviando respuesta a la base de datos para encuesta {SurveyId}", 
                    logReference, createResponseDto.SurveyId);
                
                // Guardar la respuesta en la base de datos
                var response = await _surveyService.SubmitSurveyResponseAsync(createResponseDto);
                
                _logger.LogInformation("[Ref:{LogRef}] API: Respuesta de encuesta guardada exitosamente. SurveyId: {SurveyId}, Email: {Email}, ResponseId: {ResponseId}", 
                    logReference, createResponseDto.SurveyId, createResponseDto.RespondentEmail, response.Id);
                
                return CreatedAtAction(nameof(GetSurveyResponses), new { surveyId = response.SurveyId }, response);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "[Ref:{LogRef}] API: Encuesta no encontrada al enviar respuesta. SurveyId: {SurveyId}", 
                    logReference, createResponseDto.SurveyId);
                return NotFound(new { message = ex.Message, logReference = logReference });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[Ref:{LogRef}] API: Error al enviar respuesta de encuesta. SurveyId: {SurveyId}", 
                    logReference, createResponseDto.SurveyId);
                
                // Log additional context and details
                _logger.LogDebug("[Ref:{LogRef}] API: Stack trace: {StackTrace}", logReference, ex.StackTrace);
                if (ex.InnerException != null)
                {
                    _logger.LogDebug("[Ref:{LogRef}] API: Inner exception: {InnerExceptionMessage}", 
                        logReference, ex.InnerException.Message);
                }
                
                return BadRequest(new { message = ex.Message, logReference = logReference });
            }
        }
    }
}
