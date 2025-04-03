
using Microsoft.Extensions.DependencyInjection;
using SurveyApp.Application.Interfaces;
using SurveyApp.Application.Services;
using SurveyApp.Domain.Repositories;
using SurveyApp.Domain.Services;
using SurveyApp.Infrastructure.Repositories;
using SurveyApp.Infrastructure.Services;

namespace SurveyApp.Web.DependencyInjection
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register Repositories
            services.AddScoped<ISurveyRepository, SurveyRepository>();
            services.AddScoped<IResponseRepository, ResponseRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            
            // Register Application Services
            services.AddScoped<ISurveyService, SurveyService>();
            services.AddScoped<IResponseService, ResponseService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            
            // Register Email Services
            services.AddScoped<IEmailService, MailKitEmailService>();
            
            return services;
        }
    }
}
