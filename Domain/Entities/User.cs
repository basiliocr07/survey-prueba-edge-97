
using System;

namespace SurveyApp.Domain.Entities
{
    public class User
    {
        public string Id { get; private set; }
        public string Username { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public string Role { get; private set; } // "Admin" or "Client"
        public DateTime CreatedAt { get; private set; }

        // For EF Core
        protected User() { }

        public User(string username, string email, string passwordHash, string role)
        {
            Id = Guid.NewGuid().ToString();
            Username = username;
            Email = email;
            PasswordHash = passwordHash;
            Role = role;
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateEmail(string email)
        {
            Email = email;
        }

        public void UpdatePassword(string passwordHash)
        {
            PasswordHash = passwordHash;
        }

        public void UpdateRole(string role)
        {
            Role = role;
        }
    }
}
