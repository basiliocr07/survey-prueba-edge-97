
using System;
using System.Threading.Tasks;
using SurveyApp.Domain.Entities;

namespace SurveyApp.Application.Ports
{
    public interface IUserRepository
    {
        Task<bool> ValidateUserCredentialsAsync(string username, string password);
        Task<User> GetUserByUsernameAsync(string username);
        Task<bool> CreateUserAsync(string username, string email, string password, string role);
        Task<bool> UserExistsAsync(string username);
    }
}
