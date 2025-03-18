
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SurveyApp.Application.Ports;
using SurveyApp.Application.Services;
using SurveyApp.Infrastructure.Data;
using SurveyApp.Infrastructure.Repositories;
using SurveyApp.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()
    ));

// Register repositories
builder.Services.AddScoped<ISurveyRepository, SurveyRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IAnalyticsRepository, AnalyticsRepository>();
builder.Services.AddScoped<ISuggestionRepository, SuggestionRepository>();
builder.Services.AddScoped<IKnowledgeBaseRepository, KnowledgeBaseRepository>();

// Register application services
builder.Services.AddScoped<ISurveyService, SurveyService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<ISuggestionService, SuggestionService>();
builder.Services.AddScoped<IKnowledgeBaseService, KnowledgeBaseService>();

// Add API controllers
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

// Register additional routes
app.MapControllerRoute(
    name: "dashboard",
    pattern: "dashboard/{action=Index}/{id?}",
    defaults: new { controller = "Dashboard" });

app.MapControllerRoute(
    name: "customers",
    pattern: "customers/{action=Index}/{id?}",
    defaults: new { controller = "Customers" });

app.MapControllerRoute(
    name: "analytics",
    pattern: "analytics/{action=Index}/{id?}",
    defaults: new { controller = "Analytics" });

app.MapControllerRoute(
    name: "suggestions",
    pattern: "suggestions/{action=Index}/{id?}",
    defaults: new { controller = "Suggestions" });

app.MapControllerRoute(
    name: "requirements",
    pattern: "requirements/{action=Index}/{id?}",
    defaults: new { controller = "Requirements" });

app.MapControllerRoute(
    name: "knowledge-base",
    pattern: "knowledge-base/{action=Index}/{id?}",
    defaults: new { controller = "KnowledgeBase" });

app.MapControllers(); // Map API controllers

// Apply database migrations
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while applying migrations");
    }
}

app.Run();
