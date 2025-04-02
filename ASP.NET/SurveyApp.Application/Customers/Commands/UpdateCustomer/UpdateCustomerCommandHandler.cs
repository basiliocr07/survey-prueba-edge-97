
using MediatR;
using SurveyApp.Domain.Models;
using SurveyApp.Domain.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

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
                var customer = await _customerRepository.GetCustomerByIdAsync(request.Id);
                if (customer == null)
                {
                    return new UpdateCustomerResult
                    {
                        Success = false,
                        Message = "Cliente no encontrado."
                    };
                }

                customer.BrandName = request.BrandName;
                customer.ContactName = request.ContactName;
                customer.ContactEmail = request.ContactEmail;
                customer.ContactPhone = request.ContactPhone;
                customer.CustomerType = request.CustomerType;
                customer.UpdatedAt = DateTime.UtcNow;

                await _customerRepository.UpdateCustomerAsync(customer);

                // TODO: Actualizar servicios del cliente (eliminar existentes y agregar nuevos)
                // Esto requeriría métodos adicionales en el repositorio

                return new UpdateCustomerResult
                {
                    Success = true,
                    Message = "Cliente actualizado exitosamente."
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
