
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using MediatR;
using SurveyApp.Domain.Repositories;
using SurveyApp.Domain.Services;
using SurveyApp.Infrastructure.Repositories;
using SurveyApp.Infrastructure.EmailService;
using SurveyApp.Application.Customers.Queries.GetAllCustomers;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios al contenedor
builder.Services.AddControllersWithViews();

// Registrar MediatR
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(GetAllCustomersQuery).Assembly);
});

// Registrar servicios de infraestructura
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IEmailSender, EmailSender>();

// Agregar las conexiones a la base de datos
builder.Services.AddDbContext<SurveyApp.Infrastructure.Data.ApplicationDbContext>();

var app = builder.Build();

// Configurar pipeline de solicitudes HTTP
if (!app.Environment.IsDevelopment())
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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
