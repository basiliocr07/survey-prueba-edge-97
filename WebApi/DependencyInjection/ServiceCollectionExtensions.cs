
using Microsoft.Extensions.DependencyInjection;
using SurveyApp.Application.Ports;
using SurveyApp.Application.Services;
using SurveyApp.Infrastructure.Repositories;
using SurveyApp.Infrastructure.Services;

namespace SurveyApp.WebApi.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Registrar servicios de aplicación
            services.AddScoped<ISurveyService, SurveyService>();
            services.AddScoped<ISuggestionService, SuggestionService>();
            services.AddScoped<IRequirementService, RequirementService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IKnowledgeBaseService, KnowledgeBaseService>();
            services.AddScoped<IAnalyticsService, AnalyticsService>();
            services.AddScoped<Application.Ports.IAuthenticationService, Application.Services.AuthenticationService>();
            
            return services;
        }
        
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            // Registrar repositorios
            services.AddScoped<ISurveyRepository, SurveyRepository>();
            services.AddScoped<ISuggestionRepository, SuggestionRepository>();
            services.AddScoped<IRequirementRepository, RequirementRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IKnowledgeBaseRepository, KnowledgeBaseRepository>();
            services.AddScoped<IAnalyticsRepository, AnalyticsRepository>();
            services.AddScoped<ISurveyResponseRepository, SurveyResponseRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            
            // Registrar servicios de infraestructura
            services.AddScoped<IEmailService, EmailService>();
            
            return services;
        }
    }
}
