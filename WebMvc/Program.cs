using Microsoft.EntityFrameworkCore;
using SurveyApp.Application.Ports;
using SurveyApp.Application.Services;
using SurveyApp.Infrastructure.Data;
using SurveyApp.Infrastructure.Repositories;
using SurveyApp.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configurar la autenticación con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
    });

// Configurar las políticas de autorización
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("ClientAccess", policy =>
        policy.RequireRole("Admin", "Client"));
});

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
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Configure Email Settings
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Register application services
builder.Services.AddScoped<ISurveyService, SurveyService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<ISuggestionService, SuggestionService>();
builder.Services.AddScoped<IKnowledgeBaseService, KnowledgeBaseService>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

// Add API controllers
builder.Services.AddControllers();

// Add Tailwind CSS configuration services
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
}

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

// Agregar autenticación y autorización al pipeline
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Register additional routes
app.MapControllerRoute(
    name: "surveys",
    pattern: "surveys/{action=Index}/{id?}",
    defaults: new { controller = "Survey" });

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
    name: "knowledgebase",
    pattern: "kb/{action=Index}/{id?}",
    defaults: new { controller = "KnowledgeBase" });

app.MapControllerRoute(
    name: "customers",
    pattern: "customers/{action=Index}/{id?}",
    defaults: new { controller = "Customers" });

app.MapControllerRoute(
    name: "account",
    pattern: "account/{action=Login}/{id?}",
    defaults: new { controller = "Account" });

app.MapControllers(); // Map API controllers

// Create default users on application startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var dbContext = services.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();

        // Seed default users if no users exist
        if (!dbContext.Users.Any())
        {
            // Add default admin user
            dbContext.Users.Add(new SurveyApp.Domain.Entities.User(
                "admin", 
                "admin@example.com",
                "adminpass", // In a real app, this would be hashed
                "Admin"
            ));
            
            // Add default client user
            dbContext.Users.Add(new SurveyApp.Domain.Entities.User(
                "client", 
                "client@example.com",
                "clientpass", // In a real app, this would be hashed
                "Client"
            ));
            
            dbContext.SaveChanges();
            Console.WriteLine("Default users created.");
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while applying migrations or seeding the database.");
        Console.WriteLine($"Error initializing database: {ex.Message}");
    }
}

app.Run();
