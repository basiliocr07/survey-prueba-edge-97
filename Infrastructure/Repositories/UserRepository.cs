
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SurveyApp.Application.Ports;
using SurveyApp.Domain.Entities;
using SurveyApp.Infrastructure.Data;

namespace SurveyApp.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(AppDbContext context, ILogger<UserRepository> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> ValidateUserCredentialsAsync(string username, string password)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == username);

                if (user == null)
                    return false;

                // In a real app, use password hashing
                return user.PasswordHash == password;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating user credentials");
                return false;
            }
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            try
            {
                return await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by username");
                return null;
            }
        }

        public async Task<bool> CreateUserAsync(string username, string email, string password, string role)
        {
            try
            {
                var user = new User(username, email, password, role);
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return false;
            }
        }

        public async Task<bool> UserExistsAsync(string username)
        {
            try
            {
                return await _context.Users
                    .AnyAsync(u => u.Username == username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if user exists");
                return false;
            }
        }
    }
}
