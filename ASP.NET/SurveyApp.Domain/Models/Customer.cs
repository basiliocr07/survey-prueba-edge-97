
using System;
using System.Collections.Generic;

namespace SurveyApp.Domain.Models
{
    public class Customer
    {
        public int Id { get; set; } // Cambiado de string a int para coincidir con la infraestructura
        public string BrandName { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string ContactEmail { get; set; } = string.Empty;
        public string? ContactPhone { get; set; }
        public List<string> AcquiredServices { get; set; } = new List<string>();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string CustomerType { get; set; } = "client"; // Puede ser "admin" o "client"
        
        // Propiedad de navegación para encuestas relacionadas
        public List<SurveyResponse>? SurveyResponses { get; set; }
        
        // Método para obtener el nombre completo
        public string GetFullName()
        {
            return $"{ContactName} ({BrandName})";
        }
        
        // Método para verificar si el cliente es administrador
        public bool IsAdmin()
        {
            return CustomerType.Equals("admin", StringComparison.OrdinalIgnoreCase);
        }
    }
}
