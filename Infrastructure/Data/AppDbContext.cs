using System;
using System.Collections.Generic;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SurveyApp.Domain.Entities;

namespace SurveyApp.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Survey> Surveys { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<GrowthMetric> GrowthMetrics { get; set; }
        public DbSet<Suggestion> Suggestions { get; set; }
        public DbSet<KnowledgeBaseItem> KnowledgeBaseItems { get; set; }
        public DbSet<AnalyticsData> AnalyticsData { get; set; }
        public DbSet<SurveyResponseTrend> ResponseTrends { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Survey entity
            modelBuilder.Entity<Survey>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.CreatedAt).IsRequired();

                entity.HasMany(e => e.Questions)
                      .WithOne()
                      .HasForeignKey("SurveyId")
                      .OnDelete(DeleteBehavior.Cascade);

                // Configure complex properties
                entity.OwnsOne(e => e.DeliveryConfig, config =>
                {
                    config.Property(c => c.Type).HasConversion<string>();
                    config.Property(c => c.EmailAddresses).HasConversion(
                        v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                        v => JsonSerializer.Deserialize<List<string>>(v, new JsonSerializerOptions())
                    );

                    config.OwnsOne(c => c.Schedule);
                    config.OwnsOne(c => c.Trigger);
                });
            });

            // Configure Question entity
            modelBuilder.Entity<Question>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(500);
                entity.Property(e => e.Description).HasMaxLength(1000);
                entity.Property(e => e.Type).HasConversion<string>();

                // Configure complex properties
                entity.Property(e => e.Options).HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                    v => JsonSerializer.Deserialize<List<string>>(v, new JsonSerializerOptions())
                );
            });

            // Configure Customer entity
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.BrandName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.ContactEmail).IsRequired().HasMaxLength(200);
                entity.Property(e => e.ContactPhone).HasMaxLength(50);
                entity.Property(e => e.CreatedAt).IsRequired();

                entity.Property(e => e.AcquiredServices).HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                    v => JsonSerializer.Deserialize<List<string>>(v, new JsonSerializerOptions())
                );

                entity.HasMany(e => e.GrowthMetrics)
                      .WithOne()
                      .HasForeignKey("CustomerId")
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure GrowthMetric entity
            modelBuilder.Entity<GrowthMetric>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Period).IsRequired().HasMaxLength(20);
            });

            // Configure Suggestion entity
            modelBuilder.Entity<Suggestion>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Content).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.CustomerName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.CustomerEmail).IsRequired().HasMaxLength(200);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.Status).HasConversion<string>();
                entity.Property(e => e.Category).HasMaxLength(100);
                entity.Property(e => e.Priority).HasConversion<string>().IsRequired(false);
                entity.Property(e => e.Response).HasMaxLength(1000);

                entity.Property(e => e.SimilarSuggestions).HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                    v => JsonSerializer.Deserialize<List<string>>(v, new JsonSerializerOptions())
                );
            });

            // Configure KnowledgeBaseItem entity
            modelBuilder.Entity<KnowledgeBaseItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Content).IsRequired();
                entity.Property(e => e.Category).HasMaxLength(100);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt).IsRequired();

                entity.Property(e => e.Tags).HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                    v => JsonSerializer.Deserialize<List<string>>(v, new JsonSerializerOptions())
                );
            });

            // Configure AnalyticsData entity
            modelBuilder.Entity<AnalyticsData>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TotalSurveys);
                entity.Property(e => e.TotalResponses);
                entity.Property(e => e.AverageCompletionRate);
                
                entity.Property(e => e.QuestionTypeDistribution).HasConversion(
                    v => JsonSerializer.Serialize(v, new JsonSerializerOptions()),
                    v => JsonSerializer.Deserialize<Dictionary<string, int>>(v, new JsonSerializerOptions()) ?? new Dictionary<string, int>()
                );
                
                entity.HasMany(e => e.ResponseTrends)
                      .WithOne()
                      .HasForeignKey("AnalyticsDataId")
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure SurveyResponseTrend entity
            modelBuilder.Entity<SurveyResponseTrend>(entity =>
            {
                entity.HasKey(e => e.Date);
                entity.Property(e => e.Date).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Responses);
            });
        }
    }
}
