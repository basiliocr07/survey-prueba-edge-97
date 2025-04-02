
using MediatR;
using SurveyApp.Domain.Models;
using SurveyApp.Domain.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace SurveyApp.Application.Customers.Queries.GetCustomerById
{
    // NOTA: Este handler devuelve Customer pero debería devolver CustomerDetail
    // según la definición de GetCustomerByIdQuery
    public class GetCustomerByIdQueryHandler : IRequestHandler<GetCustomerByIdQuery, CustomerDetail>
    {
        private readonly ICustomerRepository _customerRepository;

        public GetCustomerByIdQueryHandler(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public async Task<CustomerDetail> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
        {
            var customer = await _customerRepository.GetCustomerByIdAsync(request.Id);
            // Crear y devolver un CustomerDetail en lugar de solo el Customer
            return new CustomerDetail 
            { 
                Customer = customer,
                // Si se necesitan las respuestas de encuestas, deberían cargarse aquí
                SurveyResponses = new List<SurveyResponse>() 
            };
        }
    }
}
