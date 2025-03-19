
using System.Threading.Tasks;
using SurveyApp.Application.DTOs;

namespace SurveyApp.Application.Ports
{
    public interface IAuthenticationService
    {
        Task<bool> ValidateUserAsync(string username, string password);
        Task<UserDto> GetUserByUsernameAsync(string username);
        Task<bool> RegisterUserAsync(string username, string email, string password, string role);
        Task<bool> UserExistsAsync(string username);
    }
}
