
using MediatR;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SurveyApp.Application.Customers.Commands.AddCustomer
{
    /// <summary>
    /// Comando para añadir un nuevo cliente
    /// </summary>
    public class AddCustomerCommand : IRequest<AddCustomerResult>
    {
        [Required(ErrorMessage = "El nombre de la marca es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre de la marca debe tener entre {2} y {1} caracteres", MinimumLength = 2)]
        public string BrandName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre de contacto es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre de contacto debe tener entre {2} y {1} caracteres", MinimumLength = 2)]
        public string ContactName { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email de contacto es obligatorio")]
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido")]
        public string ContactEmail { get; set; } = string.Empty;

        [Phone(ErrorMessage = "El teléfono no tiene un formato válido")]
        public string ContactPhone { get; set; } = string.Empty;

        public List<string> AcquiredServices { get; set; } = new List<string>();
    }

    /// <summary>
    /// Resultado de la operación de añadir cliente
    /// </summary>
    public class AddCustomerResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? CustomerId { get; set; }
    }
}
