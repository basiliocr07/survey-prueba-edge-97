
using System;
using System.Linq;
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
                // Validar datos
                if (string.IsNullOrWhiteSpace(request.BrandName) || 
                    string.IsNullOrWhiteSpace(request.ContactName) || 
                    string.IsNullOrWhiteSpace(request.ContactEmail))
                {
                    return new AddCustomerResult
                    {
                        Success = false,
                        Message = "Los campos BrandName, ContactName y ContactEmail son obligatorios."
                    };
                }

                // Crear nuevo cliente
                var customer = new Customer
                {
                    BrandName = request.BrandName,
                    ContactName = request.ContactName,
                    ContactEmail = request.ContactEmail,
                    ContactPhone = request.ContactPhone,
                    CustomerType = request.CustomerType ?? "client",
                    AcquiredServices = request.AcquiredServices ?? new List<string>(),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Guardar el cliente
                var customerId = await _customerRepository.AddCustomerAsync(customer);

                // Relacionar el cliente con los servicios seleccionados
                if (request.AcquiredServices != null && request.AcquiredServices.Any())
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
                    Message = "Cliente añadido correctamente.",
                    CustomerId = customerId
                };
            }
            catch (Exception ex)
            {
                return new AddCustomerResult
                {
                    Success = false,
                    Message = $"Error al añadir cliente: {ex.Message}"
                };
            }
        }
    }
}
