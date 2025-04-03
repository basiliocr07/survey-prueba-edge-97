
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
        
        /// <summary>
        /// Tipo de cliente opcional para filtrar (admin o client)
        /// </summary>
        public string? CustomerType { get; set; }
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
        
        /// <summary>
        /// Datos de crecimiento mensual de clientes
        /// </summary>
        public IEnumerable<MonthlyGrowthData> MonthlyGrowthData { get; set; } = new List<MonthlyGrowthData>();
        
        /// <summary>
        /// Datos de crecimiento por marca
        /// </summary>
        public IEnumerable<BrandGrowthData> BrandGrowthData { get; set; } = new List<BrandGrowthData>();
        
        /// <summary>
        /// Tipo de gráfico a mostrar
        /// </summary>
        public string ChartType { get; set; } = "bar";
        
        /// <summary>
        /// Período de tiempo seleccionado para los datos (en meses)
        /// </summary>
        public string TimeRange { get; set; } = "12";
    }
    
    /// <summary>
    /// Datos de crecimiento mensual
    /// </summary>
    public class MonthlyGrowthData
    {
        /// <summary>
        /// Nombre del mes (formato: MMM yyyy)
        /// </summary>
        public string Month { get; set; } = string.Empty;
        
        /// <summary>
        /// Cantidad de nuevos clientes en ese mes
        /// </summary>
        public int NewCustomers { get; set; }
    }
    
    /// <summary>
    /// Datos de crecimiento por marca
    /// </summary>
    public class BrandGrowthData
    {
        /// <summary>
        /// Nombre de la marca
        /// </summary>
        public string BrandName { get; set; } = string.Empty;
        
        /// <summary>
        /// Total de clientes de esta marca
        /// </summary>
        public int TotalCustomers { get; set; }
        
        /// <summary>
        /// Nuevos clientes recientes (según el período de tiempo seleccionado)
        /// </summary>
        public int RecentCustomers { get; set; }
    }
}
