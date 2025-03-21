
using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SurveyApp.Application.Ports;
using SurveyApp.Application.Services;
using SurveyApp.Infrastructure.Data;
using SurveyApp.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

// Configure database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repositories
builder.Services.AddScoped<ISurveyRepository, SurveyRepository>();
builder.Services.AddScoped<ISurveyResponseRepository, SurveyResponseRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Repositorios que aún no se utilizan
// builder.Services.AddScoped<ISuggestionRepository, SuggestionRepository>();
// builder.Services.AddScoped<IKnowledgeBaseRepository, KnowledgeBaseRepository>();
// builder.Services.AddScoped<IRequirementRepository, RequirementRepository>();
// builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
// builder.Services.AddScoped<IAnalyticsRepository, AnalyticsRepository>();

// Register services
builder.Services.AddScoped<ISurveyService, SurveyService>();

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Configure JWT authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var securityKey = jwtSettings["SecurityKey"];

if (!string.IsNullOrEmpty(securityKey))
{
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey))
        };
    });
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Initialize/migrate the database on application startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.EnsureDatabaseCreated();
}

app.Run();
