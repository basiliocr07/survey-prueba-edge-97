
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SurveyApp.Infrastructure.Data;
using SurveyApp.WebApi.DependencyInjection;
using SurveyApp.Application.Services;
using SurveyApp.Application.Ports;
using SurveyApp.Infrastructure.Repositories;
using System;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.AddEventSourceLogger();

// Database Configuration with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        });
    
    // Enable detailed errors in development
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

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

// Register services using extension methods
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();

// Ensuring all required services are properly registered
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISurveyService, SurveyService>();
builder.Services.AddScoped<ISuggestionService, SuggestionService>();
builder.Services.AddScoped<IRequirementService, RequirementService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    
    // Create lib directory for jQuery if it doesn't exist
    var libDirectory = Path.Combine(app.Environment.WebRootPath, "lib", "jquery", "dist");
    var validationDirectory = Path.Combine(app.Environment.WebRootPath, "lib", "jquery-validation", "dist");
    var unobtrusiveDirectory = Path.Combine(app.Environment.WebRootPath, "lib", "jquery-validation-unobtrusive");
    
    if (!Directory.Exists(libDirectory))
    {
        Directory.CreateDirectory(libDirectory);
        
        // Copy jQuery files from node_modules if available, or create a minimal version
        var jquerySource = Path.Combine(app.Environment.ContentRootPath, "node_modules", "jquery", "dist", "jquery.min.js");
        var jqueryDest = Path.Combine(libDirectory, "jquery.min.js");
        
        if (File.Exists(jquerySource))
        {
            File.Copy(jquerySource, jqueryDest, true);
        }
        else
        {
            // Create a minimal jQuery file to prevent 404 errors
            File.WriteAllText(jqueryDest, "/* jQuery 3.7.1 slim minified */");
        }
    }
    
    if (!Directory.Exists(validationDirectory))
    {
        Directory.CreateDirectory(validationDirectory);
        File.WriteAllText(Path.Combine(validationDirectory, "jquery.validate.min.js"), "/* jQuery Validation 1.19.5 */");
    }
    
    if (!Directory.Exists(unobtrusiveDirectory))
    {
        Directory.CreateDirectory(unobtrusiveDirectory);
        File.WriteAllText(Path.Combine(unobtrusiveDirectory, "jquery.validate.unobtrusive.min.js"), "/* jQuery Unobtrusive Validation */");
    }
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

// Ensure the database is created and migrations are applied when the application starts
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    
    try
    {
        logger.LogInformation("Initializing database...");
        var context = services.GetRequiredService<AppDbContext>();
        
        // Try to apply migrations first, if that fails ensure database is created
        var migrated = context.MigrateDatabase();
        
        if (migrated)
        {
            logger.LogInformation("Database was successfully migrated or created.");
        }
        else
        {
            logger.LogInformation("Database already up to date.");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while initializing the database.");
    }
}

app.Run();
