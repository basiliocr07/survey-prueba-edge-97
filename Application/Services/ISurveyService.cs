
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SurveyApp.Application.DTOs;

namespace SurveyApp.Application.Services
{
    public interface ISurveyService
    {
        /// <summary>
        /// Gets a survey by its identifier
        /// </summary>
        /// <param name="id">The survey identifier</param>
        /// <returns>The survey DTO</returns>
        Task<SurveyDto> GetSurveyByIdAsync(Guid id);
        
        /// <summary>
        /// Gets all surveys
        /// </summary>
        /// <returns>A list of survey DTOs</returns>
        Task<List<SurveyDto>> GetAllSurveysAsync();
        
        /// <summary>
        /// Creates a new survey
        /// </summary>
        /// <param name="createSurveyDto">The survey creation data</param>
        /// <returns>The created survey DTO</returns>
        Task<SurveyDto> CreateSurveyAsync(CreateSurveyDto createSurveyDto);
        
        /// <summary>
        /// Updates an existing survey
        /// </summary>
        /// <param name="id">The survey identifier</param>
        /// <param name="updateSurveyDto">The survey update data</param>
        Task UpdateSurveyAsync(Guid id, CreateSurveyDto updateSurveyDto);
        
        /// <summary>
        /// Deletes a survey
        /// </summary>
        /// <param name="id">The survey identifier</param>
        Task DeleteSurveyAsync(Guid id);
        
        /// <summary>
        /// Sends survey invitation emails to all recipients specified in the delivery configuration
        /// </summary>
        /// <param name="id">The survey identifier</param>
        Task SendSurveyEmailsAsync(Guid id);
        
        /// <summary>
        /// Sends a survey email when a ticket is closed
        /// </summary>
        /// <param name="customerEmail">The customer email address</param>
        /// <param name="specificSurveyId">Optional specific survey ID to send</param>
        /// <returns>True if at least one survey was sent successfully, otherwise false</returns>
        Task<bool> SendSurveyOnTicketClosedAsync(string customerEmail, Guid? specificSurveyId = null);
        
        /// <summary>
        /// Sends a test survey email to a specified address
        /// </summary>
        /// <param name="email">The recipient email address</param>
        /// <param name="surveyId">The survey identifier</param>
        /// <returns>True if the email was sent successfully, otherwise false</returns>
        Task<bool> SendTestSurveyEmailAsync(string email, Guid surveyId);

        /// <summary>
        /// Updates the status of a survey
        /// </summary>
        /// <param name="id">The survey identifier</param>
        /// <param name="status">The new status</param>
        Task UpdateSurveyStatusAsync(Guid id, string status);
        
        /// <summary>
        /// Submits a response to a survey
        /// </summary>
        /// <param name="createResponseDto">The survey response data</param>
        /// <returns>The created survey response DTO</returns>
        Task<SurveyResponseDto> SubmitSurveyResponseAsync(CreateSurveyResponseDto createResponseDto);
        
        /// <summary>
        /// Gets all responses for a specific survey
        /// </summary>
        /// <param name="surveyId">The survey identifier</param>
        /// <returns>A list of survey response DTOs</returns>
        Task<List<SurveyResponseDto>> GetSurveyResponsesAsync(Guid surveyId);
    }
}
