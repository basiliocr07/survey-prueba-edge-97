
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
            // Register application services
            services.AddScoped<ISurveyService, SurveyService>();
            services.AddScoped<ISuggestionService, SuggestionService>();
            services.AddScoped<IRequirementService, RequirementService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IKnowledgeBaseService, KnowledgeBaseService>();
            services.AddScoped<IAnalyticsService, AnalyticsService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            
            return services;
        }
        
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            // Register repositories
            services.AddScoped<ISurveyRepository, SurveyRepository>();
            services.AddScoped<ISuggestionRepository, SuggestionRepository>();
            services.AddScoped<IRequirementRepository, RequirementRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IKnowledgeBaseRepository, KnowledgeBaseRepository>();
            services.AddScoped<IAnalyticsRepository, AnalyticsRepository>();
            services.AddScoped<ISurveyResponseRepository, SurveyResponseRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            
            // Register infrastructure services
            services.AddScoped<IEmailService, EmailService>();
            
            return services;
        }
    }
}
