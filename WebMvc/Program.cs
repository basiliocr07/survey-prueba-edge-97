
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SurveyApp.Infrastructure.Data;
using SurveyApp.Infrastructure.Repositories;
using SurveyApp.Infrastructure.Services;
using SurveyApp.Application.Services;
using SurveyApp.Application.Ports;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Database Configuration with SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(
        builder.Configuration.GetConnectionString("SqliteConnection") ?? "Data Source=surveyapp.db"));

// Configure Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(14);
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.SlidingExpiration = true;
    });

// Configure Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));
        
    options.AddPolicy("ClientAccess", policy =>
        policy.RequireRole("Admin", "Client"));
});

// Register Application Services
builder.Services.AddScoped<IKnowledgeBaseService, KnowledgeBaseService>();
builder.Services.AddScoped<ISuggestionService, SuggestionService>();
builder.Services.AddScoped<ISurveyService, SurveyService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();

// Registro explícito del servicio de autenticación usando nombre completo para evitar ambigüedades
builder.Services.AddScoped<SurveyApp.Application.Ports.IAuthenticationService, SurveyApp.Application.Services.AuthenticationService>();

// Register Infrastructure Services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IKnowledgeBaseRepository, KnowledgeBaseRepository>();
builder.Services.AddScoped<ISuggestionRepository, SuggestionRepository>();
builder.Services.AddScoped<ISurveyRepository, SurveyRepository>();
builder.Services.AddScoped<ISurveyResponseRepository, SurveyResponseRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IAnalyticsRepository, AnalyticsRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

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
