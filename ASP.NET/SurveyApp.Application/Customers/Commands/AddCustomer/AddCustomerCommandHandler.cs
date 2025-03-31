
using MediatR;
using SurveyApp.Domain.Models;
using SurveyApp.Domain.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SurveyApp.Application.Customers.Commands.AddCustomer
{
    public class AddCustomerCommandHandler : IRequestHandler<AddCustomerCommand, AddCustomerResult>
    {
        private readonly ICustomerRepository _customerRepository;

        public AddCustomerCommandHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<AddCustomerResult> Handle(AddCustomerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validación de la solicitud
                if (string.IsNullOrWhiteSpace(request.BrandName) || 
                    string.IsNullOrWhiteSpace(request.ContactName) || 
                    string.IsNullOrWhiteSpace(request.ContactEmail))
                {
                    return new AddCustomerResult
                    {
                        Success = false,
                        Message = "Los campos Nombre de Marca, Nombre de Contacto y Email son obligatorios"
                    };
                }

                // Crear el objeto Customer a partir del comando
                var customer = new Customer
                {
                    BrandName = request.BrandName,
                    ContactName = request.ContactName,
                    ContactEmail = request.ContactEmail,
                    ContactPhone = request.ContactPhone,
                    AcquiredServices = request.AcquiredServices,
                    CustomerType = request.CustomerType, // Asegurar que se establece el CustomerType
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Guardar el cliente
                var customerId = await _customerRepository.AddCustomerAsync(customer);

                // Relacionar con servicios
                if (request.AcquiredServices.Any())
                {
                    foreach (var service in request.AcquiredServices)
                    {
                        var serviceId = await _customerRepository.GetServiceIdByNameAsync(service);
                        if (!string.IsNullOrEmpty(serviceId))
                        {
                            await _customerRepository.AddCustomerServiceAsync(customerId, serviceId);
                        }
                    }
                }

                // Retornar resultado exitoso
                return new AddCustomerResult
                {
                    Success = true,
                    Message = "Cliente añadido exitosamente",
                    CustomerId = customerId.ToString()
                };
            }
            catch (Exception ex)
            {
                // Log the exception (idealmente usar un servicio de logging)
                Console.WriteLine($"Error adding customer: {ex.Message}");
                
                // Retornar resultado con error
                return new AddCustomerResult
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }
    }
}
