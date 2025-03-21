
using System;
using System.Threading.Tasks;
using SurveyApp.Application.DTOs;
using SurveyApp.Application.Ports;
using SurveyApp.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace SurveyApp.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthenticationService> _logger;

        public AuthenticationService(
            IUserRepository userRepository,
            ILogger<AuthenticationService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<bool> ValidateUserAsync(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return false;
                
            // Check predefined users (hardcoded for development)
            _logger.LogInformation($"Validating predefined user: {username}");
            
            // Ensure case-insensitive comparison for predefined users
            if ((username.Equals("admin", StringComparison.OrdinalIgnoreCase) && password == "adminpass") || 
                (username.Equals("client", StringComparison.OrdinalIgnoreCase) && password == "clientpass"))
            {
                _logger.LogInformation($"Predefined user {username} validated successfully");
                return true;
            }
            
            _logger.LogInformation($"User {username} not a predefined user, checking database");
            return await _userRepository.ValidateUserCredentialsAsync(username, password);
        }

        public async Task<UserDto> GetUserByUsernameAsync(string username)
        {
            // For predefined hardcoded users
            if (username.Equals("admin", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("Returning predefined admin user");
                return new UserDto
                {
                    Id = "admin-id",
                    Username = "admin",
                    Email = "admin@example.com",
                    Role = "Admin"
                };
            }
            
            if (username.Equals("client", StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogInformation("Returning predefined client user");
                return new UserDto
                {
                    Id = "client-id",
                    Username = "client",
                    Email = "client@example.com",
                    Role = "Client"
                };
            }
            
            var user = await _userRepository.GetUserByUsernameAsync(username);
            if (user == null)
            {
                _logger.LogWarning($"User {username} not found in database");
                return null;
            }

            _logger.LogInformation($"User {username} found in database");
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role
            };
        }

        public async Task<bool> RegisterUserAsync(string username, string email, string password, string role)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(email) || 
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(role))
                return false;
                
            // Don't allow registering predefined users
            if (username.Equals("admin", StringComparison.OrdinalIgnoreCase) || 
                username.Equals("client", StringComparison.OrdinalIgnoreCase))
                return false;
                
            if (await _userRepository.UserExistsAsync(username))
            {
                return false;
            }

            return await _userRepository.CreateUserAsync(username, email, password, role);
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            if (username.Equals("admin", StringComparison.OrdinalIgnoreCase) || 
                username.Equals("client", StringComparison.OrdinalIgnoreCase))
                return true;
                
            return await _userRepository.UserExistsAsync(username);
        }
    }
}
