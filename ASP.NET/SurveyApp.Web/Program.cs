
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using SurveyApp.Infrastructure.Data;
using SurveyApp.Web.DependencyInjection;
using System;
using Microsoft.Extensions.Logging;
using SurveyApp.Application.Interfaces;
using SurveyApp.Application.Services;
using SurveyApp.Domain.Repositories;
using SurveyApp.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllersWithViews();

// Configure DbContext with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseSqlServer(connectionString);
});

// Register application and infrastructure services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();

// Additional service registrations from extensions file
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IAnalyticsRepository, AnalyticsRepository>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<ISurveyResponseRepository, SurveyResponseRepository>();
builder.Services.AddScoped<ISurveyResponseService, SurveyResponseService>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Additional routes from extensions file
app.MapControllerRoute(
    name: "customers",
    pattern: "customers/{action=Index}/{id?}",
    defaults: new { controller = "Customers" });

app.MapControllerRoute(
    name: "analytics",
    pattern: "analytics/{action=Index}/{id?}",
    defaults: new { controller = "Analytics" });

app.MapControllerRoute(
    name: "surveyResponses",
    pattern: "survey-responses/{action=Index}/{id?}",
    defaults: new { controller = "SurveyResponses" });

// Ensure the database is created when the application starts
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while creating the database.");
    }
}

app.Run();
