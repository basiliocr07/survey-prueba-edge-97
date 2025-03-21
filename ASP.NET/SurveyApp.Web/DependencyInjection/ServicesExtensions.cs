
using Microsoft.Extensions.DependencyInjection;
using SurveyApp.Application.Interfaces;
using SurveyApp.Application.Services;

namespace SurveyApp.Web.DependencyInjection
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ISurveyService, SurveyService>();
            return services;
        }

        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped<SurveyApp.Domain.Repositories.ISurveyRepository, SurveyApp.Infrastructure.Repositories.SurveyRepository>();
            return services;
        }
    }
}
