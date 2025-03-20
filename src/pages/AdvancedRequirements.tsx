
import { useState, useEffect } from 'react';
import Navbar from '@/components/layout/Navbar';
import Footer from '@/components/layout/Footer';
import RequirementsDashboard from '@/components/requirements/RequirementsDashboard';
import { useToast } from "@/components/ui/use-toast";
import { Requirement } from '@/types/requirements';
import { useQuery } from '@tanstack/react-query';

// Sample mock data for development purposes
const mockRequirements: Requirement[] = [
  {
    id: '1',
    title: 'Implementar inicio de sesión con Google',
    content: 'Añadir opción para que los usuarios puedan iniciar sesión usando su cuenta de Google',
    description: 'Los usuarios necesitan poder iniciar sesión con Google para una experiencia más fluida',
    customerName: 'María González',
    customerEmail: 'maria@example.com',
    createdAt: '2023-09-15T10:30:00',
    status: 'proposed',
    category: 'Authentication',
    priority: 'high',
    projectArea: 'User Management',
    acceptanceCriteria: 'Los usuarios deben poder iniciar sesión con su cuenta de Google existente',
    completionPercentage: 0
  },
  {
    id: '2',
    title: 'Crear panel de estadísticas avanzado',
    content: 'Diseñar e implementar un panel que muestre estadísticas en tiempo real sobre el uso del sistema',
    description: 'Necesitamos análisis detallados del uso de la plataforma',
    customerName: 'Carlos López',
    customerEmail: 'carlos@example.com',
    createdAt: '2023-09-10T14:45:00',
    status: 'in-progress',
    category: 'Analytics',
    priority: 'medium',
    projectArea: 'Dashboard',
    acceptanceCriteria: 'El panel debe mostrar datos de usuarios activos, sesiones y tasa de conversión',
    completionPercentage: 35
  },
  {
    id: '3',
    title: 'Optimización de consultas SQL en listados',
    content: 'Mejorar el rendimiento de las consultas SQL en las páginas de listado de productos',
    description: 'La página de productos tarda más de 3 segundos en cargar con más de 100 productos',
    customerName: 'Ana Martínez',
    customerEmail: 'ana@example.com',
    createdAt: '2023-09-05T09:15:00',
    status: 'implemented',
    category: 'Performance',
    priority: 'critical',
    projectArea: 'Database',
    acceptanceCriteria: 'Las páginas de listado deben cargar en menos de 1 segundo con 500 productos',
    completionPercentage: 100
  },
  {
    id: '4',
    title: 'Integración con sistema de pagos',
    content: 'Implementar la integración con el sistema de pagos Stripe',
    description: 'Necesitamos procesar pagos con tarjetas de crédito y métodos alternativos',
    customerName: 'Roberto Gómez',
    customerEmail: 'roberto@example.com',
    createdAt: '2023-08-28T11:20:00',
    status: 'in-progress',
    category: 'Payments',
    priority: 'high',
    projectArea: 'Finance',
    acceptanceCriteria: 'Los usuarios deben poder pagar con tarjeta, PayPal y transferencia bancaria',
    completionPercentage: 65
  },
  {
    id: '5',
    title: 'Exportación de reportes a PDF',
    content: 'Añadir funcionalidad para exportar reportes en formato PDF',
    description: 'Los usuarios necesitan descargar reportes para compartirlos externamente',
    customerName: 'Laura Sánchez',
    customerEmail: 'laura@example.com',
    createdAt: '2023-08-20T15:30:00',
    status: 'rejected',
    category: 'Reports',
    priority: 'low',
    projectArea: 'Analytics',
    acceptanceCriteria: 'Los reportes PDF deben incluir gráficos, tablas y datos completos del informe',
    completionPercentage: 0
  }
];

export default function AdvancedRequirements() {
  const [userRole, setUserRole] = useState('');
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const { toast } = useToast();
  
  // Simulate loading user data from localStorage
  useEffect(() => {
    const loggedIn = localStorage.getItem('isLoggedIn') === 'true';
    const role = localStorage.getItem('userRole') || '';
    
    setIsLoggedIn(loggedIn);
    setUserRole(role);
  }, []);

  // In a real application, replace this with actual API call
  const { data: requirements, isLoading, isError } = useQuery({
    queryKey: ['requirements'],
    queryFn: async () => {
      // Simulate API call delay
      await new Promise(resolve => setTimeout(resolve, 1000));
      
      // In production, this would be a real API call to your backend
      console.log('Fetching requirements data from API');
      return mockRequirements;
    }
  });

  // For demo purposes, we'll force admin mode
  const isAdmin = true; // userRole.toLowerCase() === 'admin';
  
  // Calculate counts based on requirements data
  const totalCount = requirements?.length || 0;
  const proposedCount = requirements?.filter(req => req.status === 'proposed').length || 0;
  const inProgressCount = requirements?.filter(req => req.status === 'in-progress').length || 0;
  const implementedCount = requirements?.filter(req => req.status === 'implemented').length || 0;
  const rejectedCount = requirements?.filter(req => req.status === 'rejected').length || 0;

  return (
    <div className="min-h-screen flex flex-col bg-background">
      <Navbar />
      
      <main className="flex-1 w-full px-4 sm:px-6 lg:px-8 pt-24 pb-20 flex justify-center">
        <div className="w-[900px] max-w-[900px] h-full bg-white p-6">
          <div className="text-center mb-8">
            <h1 className="text-3xl font-bold tracking-tight sm:text-4xl mb-2">Advanced Requirements</h1>
            <p className="text-muted-foreground max-w-2xl mx-auto">
              Comprehensive management of project requirements with advanced analytics
            </p>
          </div>
          
          {isLoading ? (
            <div className="flex justify-center items-center py-12">
              <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary"></div>
            </div>
          ) : isError ? (
            <div className="text-center py-12">
              <p className="text-red-500 mb-2">Error loading requirements data</p>
              <button 
                className="text-blue-500 underline"
                onClick={() => {
                  toast({
                    title: "Trying to reconnect...",
                    description: "Attempting to load requirements data again.",
                  });
                }}
              >
                Try again
              </button>
            </div>
          ) : (
            <RequirementsDashboard
              isAdmin={isAdmin}
              requirements={requirements || []}
              totalCount={totalCount}
              proposedCount={proposedCount}
              inProgressCount={inProgressCount} 
              implementedCount={implementedCount}
              rejectedCount={rejectedCount}
            />
          )}
        </div>
      </main>
      
      <Footer />
    </div>
  );
}
