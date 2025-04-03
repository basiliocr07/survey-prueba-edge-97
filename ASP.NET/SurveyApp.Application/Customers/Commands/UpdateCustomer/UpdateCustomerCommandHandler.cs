
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SurveyApp.Domain.Models;
using SurveyApp.Domain.Repositories;

namespace SurveyApp.Application.Customers.Commands.UpdateCustomer
{
    public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, UpdateCustomerResult>
    {
        private readonly ICustomerRepository _customerRepository;

        public UpdateCustomerCommandHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<UpdateCustomerResult> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Validar datos
                if (string.IsNullOrWhiteSpace(request.BrandName) || 
                    string.IsNullOrWhiteSpace(request.ContactName) || 
                    string.IsNullOrWhiteSpace(request.ContactEmail))
                {
                    return new UpdateCustomerResult
                    {
                        Success = false,
                        Message = "Los campos BrandName, ContactName y ContactEmail son obligatorios."
                    };
                }

                // Obtener el cliente actual
                var existingCustomer = await _customerRepository.GetCustomerByIdAsync(request.Id);
                if (existingCustomer == null)
                {
                    return new UpdateCustomerResult
                    {
                        Success = false,
                        Message = "Cliente no encontrado."
                    };
                }

                // Actualizar datos del cliente
                existingCustomer.BrandName = request.BrandName;
                existingCustomer.ContactName = request.ContactName;
                existingCustomer.ContactEmail = request.ContactEmail;
                existingCustomer.ContactPhone = request.ContactPhone;
                existingCustomer.CustomerType = request.CustomerType ?? "client";
                existingCustomer.UpdatedAt = DateTime.UtcNow;

                // Actualizar el cliente
                await _customerRepository.UpdateCustomerAsync(existingCustomer);

                // Aquí podría implementarse la actualización de los servicios asociados al cliente
                // Se necesitaría un método adicional en el repositorio para manejar esto

                return new UpdateCustomerResult
                {
                    Success = true,
                    Message = "Cliente actualizado correctamente."
                };
            }
            catch (Exception ex)
            {
                return new UpdateCustomerResult
                {
                    Success = false,
                    Message = $"Error al actualizar cliente: {ex.Message}"
                };
            }
        }
    }
}
