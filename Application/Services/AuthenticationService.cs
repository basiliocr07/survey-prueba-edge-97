
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SurveyApp.Application.DTOs;
using SurveyApp.Application.Ports;

namespace SurveyApp.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(IUserRepository userRepository, ILogger<AuthenticationService> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> ValidateUserAsync(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return false;

            try
            {
                return await _userRepository.ValidateUserCredentialsAsync(username, password);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating user credentials");
                return false;
            }
        }

        public async Task<UserDto> GetUserByUsernameAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
                return null;

            try
            {
                var user = await _userRepository.GetUserByUsernameAsync(username);
                if (user == null)
                    return null;

                return new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    Role = user.Role
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by username");
                return null;
            }
        }

        public async Task<bool> RegisterUserAsync(string username, string email, string password, string role)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(email))
                return false;

            try
            {
                if (await _userRepository.UserExistsAsync(username))
                    return false;

                return await _userRepository.CreateUserAsync(username, email, password, role);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering user");
                return false;
            }
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
                return false;

            try
            {
                return await _userRepository.UserExistsAsync(username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if user exists");
                return false;
            }
        }
    }
}
