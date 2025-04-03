
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SurveyApp.Domain.Repositories;

namespace SurveyApp.Application.Customers.Commands.DeleteCustomer
{
    public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand, bool>
    {
        private readonly ICustomerRepository _customerRepository;

        public DeleteCustomerCommandHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<bool> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _customerRepository.DeleteCustomerAsync(request.Id);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
