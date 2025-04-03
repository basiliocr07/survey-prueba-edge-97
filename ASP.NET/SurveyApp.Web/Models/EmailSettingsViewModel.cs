
using System;
using System.Collections.Generic;

namespace SurveyApp.Web.Models
{
    /// <summary>
    /// ViewModel para la configuración de envío de correos electrónicos de encuestas
    /// </summary>
    public class EmailSettingsViewModel
    {
        /// <summary>
        /// Lista de encuestas disponibles para enviar
        /// </summary>
        public List<SurveyListItemViewModel> Surveys { get; set; } = new List<SurveyListItemViewModel>();
        
        /// <summary>
        /// Configuración de envío de correos
        /// </summary>
        public DeliveryConfigViewModel DeliveryConfig { get; set; } = new DeliveryConfigViewModel();
        
        /// <summary>
        /// ID de la encuesta seleccionada actualmente
        /// </summary>
        public int? SelectedSurveyId { get; set; }
        
        /// <summary>
        /// Lista de clientes disponibles para enviar encuestas
        /// </summary>
        public List<CustomerViewModel> Customers { get; set; } = new List<CustomerViewModel>();
        
        /// <summary>
        /// Lista de tipos de clientes para el filtrado
        /// </summary>
        public List<string> CustomerTypes { get; set; } = new List<string>();
    }

    /// <summary>
    /// Elemento de lista para mostrar encuestas en la configuración de emails
    /// </summary>
    public class SurveyListItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool HasCustomDeliveryConfig { get; set; }
    }

    /// <summary>
    /// Modelo simplificado de cliente para selección en pantalla de envío de emails
    /// </summary>
    public class CustomerViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string CustomerType { get; set; } = string.Empty;
    }
}
