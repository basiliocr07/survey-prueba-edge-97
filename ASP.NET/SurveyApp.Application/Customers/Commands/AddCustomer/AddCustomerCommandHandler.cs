
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
                // Crear el objeto Customer a partir del comando
                var customer = new Customer
                {
                    BrandName = request.BrandName,
                    ContactName = request.ContactName,
                    ContactEmail = request.ContactEmail,
                    ContactPhone = request.ContactPhone,
                    AcquiredServices = request.AcquiredServices,
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

                return new AddCustomerResult
                {
                    Success = true,
                    Message = "Cliente añadido exitosamente",
                    CustomerId = customerId
                };
            }
            catch (Exception ex)
            {
                return new AddCustomerResult
                {
                    Success = false,
                    Message = $"Error: {ex.Message}"
                };
            }
        }
    }
}
