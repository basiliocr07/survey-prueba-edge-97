
using MediatR;
using SurveyApp.Domain.Models;
using System.Collections.Generic;

namespace SurveyApp.Application.Customers.Queries.GetCustomerGrowthData
{
    /// <summary>
    /// Consulta para obtener datos de crecimiento de clientes y uso de servicios
    /// </summary>
    public class GetCustomerGrowthDataQuery : IRequest<CustomerGrowthDataViewModel>
    {
        /// <summary>
        /// Período de tiempo opcional para filtrar los datos (en meses)
        /// </summary>
        public int? TimeRangeInMonths { get; set; }
        
        /// <summary>
        /// Categoría de servicio opcional para filtrar
        /// </summary>
        public string? ServiceCategory { get; set; }
    }

    /// <summary>
    /// Modelo de vista para la página de crecimiento de clientes
    /// </summary>
    public class CustomerGrowthDataViewModel
    {
        /// <summary>
        /// Lista de todos los clientes
        /// </summary>
        public IEnumerable<Customer> Customers { get; set; } = new List<Customer>();
        
        /// <summary>
        /// Lista de todos los servicios disponibles
        /// </summary>
        public IEnumerable<Service> Services { get; set; } = new List<Service>();
        
        /// <summary>
        /// Datos de uso de servicios por cliente
        /// </summary>
        public IEnumerable<ServiceUsageData> ServiceUsageData { get; set; } = new List<ServiceUsageData>();
    }
}
