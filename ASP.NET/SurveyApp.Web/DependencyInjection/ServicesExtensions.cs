
using Microsoft.Extensions.DependencyInjection;
using SurveyApp.Application.Interfaces;
using SurveyApp.Application.Services;
using SurveyApp.Domain.Repositories;
using SurveyApp.Infrastructure.Repositories;
using System.Reflection;

namespace SurveyApp.Web.DependencyInjection
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ISurveyService, SurveyService>();
            services.AddScoped<ISurveyResponseService, SurveyResponseService>();
            
            // Add MediatR
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(SurveyService).Assembly));
            
            return services;
        }

        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<ISurveyRepository, SurveyRepository>();
            services.AddScoped<ISurveyResponseRepository, SurveyResponseRepository>();
            return services;
        }
    }
}
