
using Microsoft.Extensions.DependencyInjection;
using SurveyApp.Application.Interfaces;
using SurveyApp.Application.Services;
using SurveyApp.Domain.Repositories;
using SurveyApp.Domain.Services;
using SurveyApp.Infrastructure.Repositories;
using SurveyApp.Infrastructure.Services;
using System.Reflection;

namespace SurveyApp.Web.DependencyInjection
{
    public static class ServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Servicios de aplicación
            services.AddScoped<ISurveyService, SurveyService>();
            services.AddScoped<ISurveyResponseService, SurveyResponseService>();
            
            // CQRS - Registrar todos los manejadores de comandos y consultas desde el assembly de Application
            services.AddMediatR(cfg => 
            {
                cfg.RegisterServicesFromAssembly(typeof(SurveyService).Assembly);
                // Registrar comportamientos de pipeline si es necesario (validación, logging, etc.)
                // cfg.AddBehavior<IPipelineBehavior<,>, ValidationBehavior<,>>();
            });
            
            return services;
        }

        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            // Repositorios
            services.AddScoped<ISurveyRepository, SurveyRepository>();
            services.AddScoped<ISurveyResponseRepository, SurveyResponseRepository>();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            
            // Servicio de email
            services.AddScoped<IEmailService, SmtpEmailService>();
            
            // Otros servicios de infraestructura
            // services.AddScoped<IFileStorage, FileStorage>();
            
            return services;
        }
    }
}
