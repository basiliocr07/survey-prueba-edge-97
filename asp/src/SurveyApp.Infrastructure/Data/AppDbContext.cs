
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
        public DbSet<SurveyResponse> SurveyResponses { get; set; }
        public DbSet<QuestionResponse> QuestionResponses { get; set; }
        public DbSet<DeliveryConfig> DeliveryConfigs { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Trigger> Triggers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Survey
            modelBuilder.Entity<Survey>()
                .HasMany(s => s.Questions)
                .WithOne()
                .HasForeignKey("SurveyId")
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Survey>()
                .HasOne(s => s.DeliveryConfig)
                .WithOne()
                .HasForeignKey<DeliveryConfig>("SurveyId");

            // SurveyResponse
            modelBuilder.Entity<SurveyResponse>()
                .HasMany(sr => sr.Answers)
                .WithOne()
                .HasForeignKey("SurveyResponseId")
                .OnDelete(DeleteBehavior.Cascade);

            // DeliveryConfig
            modelBuilder.Entity<DeliveryConfig>()
                .HasOne(dc => dc.Schedule)
                .WithOne()
                .HasForeignKey<Schedule>("DeliveryConfigId");

            modelBuilder.Entity<DeliveryConfig>()
                .HasOne(dc => dc.Trigger)
                .WithOne()
                .HasForeignKey<Trigger>("DeliveryConfigId");

            // Handle collection of primitive types
            modelBuilder.Entity<Question>()
                .Property(q => q.Options)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());

            modelBuilder.Entity<DeliveryConfig>()
                .Property(dc => dc.EmailAddresses)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());

            modelBuilder.Entity<QuestionResponse>()
                .Property(qr => qr.MultipleAnswers)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
                    
            base.OnModelCreating(modelBuilder);
        }
    }
}
