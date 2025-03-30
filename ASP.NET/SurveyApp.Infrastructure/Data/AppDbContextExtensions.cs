
using Microsoft.EntityFrameworkCore;
using SurveyApp.Infrastructure.Data.Entities;

namespace SurveyApp.Infrastructure.Data
{
    public static class AppDbContextExtensions
    {
        public static void ConfigureCustomerEntities(this ModelBuilder modelBuilder)
        {
            // Configuración de Customer
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.BrandName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ContactName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ContactEmail).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
                entity.Property(e => e.UpdatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Configuración de Service
            modelBuilder.Entity<Service>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            // Configuración de CustomerService (tabla de unión)
            modelBuilder.Entity<CustomerService>(entity =>
            {
                entity.HasKey(e => new { e.CustomerId, e.ServiceId });
                
                entity.HasOne(e => e.Customer)
                      .WithMany(c => c.CustomerServices)
                      .HasForeignKey(e => e.CustomerId);
                      
                entity.HasOne(e => e.Service)
                      .WithMany(s => s.CustomerServices)
                      .HasForeignKey(e => e.ServiceId);
            });
        }
    }
}
