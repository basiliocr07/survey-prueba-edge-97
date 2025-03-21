
using System.Threading.Tasks;
using SurveyApp.Application.DTOs;
using SurveyApp.Application.Ports;
using SurveyApp.Domain.Entities;

namespace SurveyApp.Application.Services
{
    public class AuthenticationService : Application.Ports.IAuthenticationService
    {
        private readonly IUserRepository _userRepository;

        public AuthenticationService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> ValidateUserAsync(string username, string password)
        {
            return await _userRepository.ValidateUserCredentialsAsync(username, password);
        }

        public async Task<UserDto> GetUserByUsernameAsync(string username)
        {
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
            if (await _userRepository.UserExistsAsync(username))
            {
                return false;
            }

            return await _userRepository.CreateUserAsync(username, email, password, role);
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            return await _userRepository.UserExistsAsync(username);
        }
    }
}
