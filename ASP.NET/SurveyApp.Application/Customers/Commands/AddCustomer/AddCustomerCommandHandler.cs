
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SurveyApp.Domain.Models;
using SurveyApp.Domain.Repositories;

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
                // Validar entrada
                if (string.IsNullOrWhiteSpace(request.BrandName) || 
                    string.IsNullOrWhiteSpace(request.ContactName) || 
                    string.IsNullOrWhiteSpace(request.ContactEmail))
                {
                    return new AddCustomerResult
                    {
                        Success = false,
                        Message = "Los campos Marca, Nombre de Contacto y Email son obligatorios."
                    };
                }

                // Crear entidad de cliente
                var customer = new Customer
                {
                    BrandName = request.BrandName,
                    ContactName = request.ContactName,
                    ContactEmail = request.ContactEmail,
                    ContactPhone = request.ContactPhone,
                    CustomerType = request.CustomerType,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Agregar cliente a la base de datos
                var customerId = await _customerRepository.AddCustomerAsync(customer);

                // Agregar relaciones con servicios
                if (request.AcquiredServices != null && request.AcquiredServices.Count > 0)
                {
                    foreach (var serviceName in request.AcquiredServices)
                    {
                        var serviceId = await _customerRepository.GetServiceIdByNameAsync(serviceName);
                        if (!string.IsNullOrEmpty(serviceId))
                        {
                            await _customerRepository.AddCustomerServiceAsync(customerId, serviceId);
                        }
                    }
                }

                return new AddCustomerResult
                {
                    Success = true,
                    Message = "Cliente agregado con éxito.",
                    CustomerId = customerId
                };
            }
            catch (Exception ex)
            {
                return new AddCustomerResult
                {
                    Success = false,
                    Message = $"Error al agregar cliente: {ex.Message}"
                };
            }
        }
    }
}
