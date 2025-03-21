
using System.Threading.Tasks;
using SurveyApp.Application.DTOs;
using SurveyApp.Application.Ports;
using SurveyApp.Domain.Entities;

namespace SurveyApp.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;

        public AuthenticationService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> ValidateUserAsync(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return false;
                
            // Verificación de usuarios predefinidos (hardcoded para desarrollo)
            if ((username == "admin" && password == "adminpass") || 
                (username == "client" && password == "clientpass"))
            {
                return true;
            }
            
            return await _userRepository.ValidateUserCredentialsAsync(username, password);
        }

        public async Task<UserDto> GetUserByUsernameAsync(string username)
        {
            // Para usuarios predefinidos hardcoded
            if (username == "admin")
            {
                return new UserDto
                {
                    Id = "admin-id",
                    Username = "admin",
                    Email = "admin@example.com",
                    Role = "Admin"
                };
            }
            
            if (username == "client")
            {
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
                return null;
            }

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
                
            // No permitir registrar usuarios predefinidos
            if (username == "admin" || username == "client")
                return false;
                
            if (await _userRepository.UserExistsAsync(username))
            {
                return false;
            }

            return await _userRepository.CreateUserAsync(username, email, password, role);
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            if (username == "admin" || username == "client")
                return true;
                
            return await _userRepository.UserExistsAsync(username);
        }
    }
}
