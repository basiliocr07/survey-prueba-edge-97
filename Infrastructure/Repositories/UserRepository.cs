
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SurveyApp.Application.Ports;
using SurveyApp.Domain.Entities;
using SurveyApp.Infrastructure.Data;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;

namespace SurveyApp.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(AppDbContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> ValidateUserCredentialsAsync(string username, string password)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
                if (user == null)
                {
                    _logger.LogWarning($"User validation failed: Username {username} not found");
                    return false;
                }

                var hashedPassword = HashPassword(password);
                var isValid = user.PasswordHash == hashedPassword;
                
                _logger.LogInformation($"User validation for {username}: {(isValid ? "successful" : "failed")}");
                return isValid;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error validating credentials for user {username}");
                return false;
            }
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            try
            {
                return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving user {username}");
                return null;
            }
        }

        public async Task<bool> CreateUserAsync(string username, string email, string password, string role)
        {
            try
            {
                var hashedPassword = HashPassword(password);
                var user = new User(username, email, hashedPassword, role);
                
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                
                _logger.LogInformation($"User {username} created successfully");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating user {username}");
                return false;
            }
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            try
            {
                return await _context.Users.AnyAsync(u => u.Username == username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking if user {username} exists");
                return false;
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }
    }
}
