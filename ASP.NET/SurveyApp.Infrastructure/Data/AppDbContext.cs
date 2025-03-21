
using Microsoft.EntityFrameworkCore;
using SurveyApp.Domain.Models;
using System.Text.Json;

namespace SurveyApp.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Survey> Surveys { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Survey>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                
                // Configurar la propiedad Questions como una columna JSON
                entity.Property(e => e.Questions)
                      .HasConversion(
                          v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                          v => JsonSerializer.Deserialize<List<Question>>(v, (JsonSerializerOptions)null) ?? new List<Question>());
            });
        }
    }
}
