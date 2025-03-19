
import { useState, useEffect } from 'react';
import Navbar from '@/components/layout/Navbar';
import Footer from '@/components/layout/Footer';
import ClientRequirementForm from '@/components/client/ClientRequirementForm';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { FileText, Filter, Search } from 'lucide-react';
import { useQuery } from '@tanstack/react-query';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { 
  Select, 
  SelectContent, 
  SelectItem, 
  SelectTrigger, 
  SelectValue 
} from '@/components/ui/select';

// Tipo para los requerimientos
interface Requirement {
  id: string;
  title: string;
  description: string;
  status: string;
  priority: string;
  projectArea: string;
  createdAt: string;
  customerName?: string;
  customerEmail?: string;
}

export default function Requirements() {
  const [userRole, setUserRole] = useState('');
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [activeTab, setActiveTab] = useState('new');
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState('');
  const [priorityFilter, setPriorityFilter] = useState('');
  
  // Datos de ejemplo para mostrar en el modo administrador
  const sampleRequirements: Requirement[] = [
    {
      id: '1',
      title: 'Implementar inicio de sesión con Google',
      description: 'Añadir opción para que los usuarios puedan iniciar sesión usando su cuenta de Google',
      status: 'Propuesto',
      priority: 'Alta',
      projectArea: 'Autenticación',
      createdAt: '2023-09-15T10:30:00',
      customerName: 'María González',
      customerEmail: 'maria@example.com'
    },
    {
      id: '2',
      title: 'Crear panel de estadísticas',
      description: 'Diseñar e implementar un panel que muestre estadísticas en tiempo real sobre el uso del sistema',
      status: 'En progreso',
      priority: 'Media',
      projectArea: 'Dashboard',
      createdAt: '2023-09-10T14:45:00',
      customerName: 'Carlos López',
      customerEmail: 'carlos@example.com'
    },
    {
      id: '3',
      title: 'Optimización de consultas SQL',
      description: 'Mejorar el rendimiento de las consultas SQL en las páginas de listado de productos',
      status: 'En pruebas',
      priority: 'Crítica',
      projectArea: 'Base de datos',
      createdAt: '2023-09-05T09:15:00',
      customerName: 'Ana Martínez',
      customerEmail: 'ana@example.com'
    },
  ];
  
  // Simular carga de requerimientos con React Query
  const { data: requirements = [], isLoading } = useQuery({
    queryKey: ['requirements', statusFilter, priorityFilter, searchTerm],
    queryFn: async () => {
      // En producción, esta función haría una petición API real
      console.log('Fetching requirements with filters:', { statusFilter, priorityFilter, searchTerm });
      
      // Simular filtrado
      let filtered = [...sampleRequirements];
      
      if (statusFilter) {
        filtered = filtered.filter(req => req.status.toLowerCase() === statusFilter.toLowerCase());
      }
      
      if (priorityFilter) {
        filtered = filtered.filter(req => req.priority.toLowerCase() === priorityFilter.toLowerCase());
      }
      
      if (searchTerm) {
        filtered = filtered.filter(req => 
          req.title.toLowerCase().includes(searchTerm.toLowerCase()) || 
          req.description.toLowerCase().includes(searchTerm.toLowerCase())
        );
      }
      
      return filtered;
    },
    enabled: isLoggedIn && userRole.toLowerCase() === 'admin'
  });
  
  useEffect(() => {
    const loggedIn = localStorage.getItem('isLoggedIn') === 'true';
    const role = localStorage.getItem('userRole') || '';
    
    setIsLoggedIn(loggedIn);
    setUserRole(role);
  }, []);
  
  // Función para mostrar el estado con colores adecuados
  const getStatusBadge = (status: string) => {
    const statusColors: Record<string, string> = {
      'propuesto': 'bg-blue-100 text-blue-800',
      'en progreso': 'bg-yellow-100 text-yellow-800',
      'en pruebas': 'bg-purple-100 text-purple-800',
      'completado': 'bg-green-100 text-green-800',
      'cancelado': 'bg-red-100 text-red-800',
    };
    
    const normalizedStatus = status.toLowerCase();
    const colorClass = statusColors[normalizedStatus] || 'bg-gray-100 text-gray-800';
    
    return (
      <span className={`px-2 py-1 rounded-full text-xs font-medium ${colorClass}`}>
        {status}
      </span>
    );
  };
  
  // Función para mostrar la prioridad con colores adecuados
  const getPriorityBadge = (priority: string) => {
    const priorityColors: Record<string, string> = {
      'crítica': 'bg-red-100 text-red-800',
      'alta': 'bg-orange-100 text-orange-800',
      'media': 'bg-blue-100 text-blue-800',
      'baja': 'bg-green-100 text-green-800',
    };
    
    const normalizedPriority = priority.toLowerCase();
    const colorClass = priorityColors[normalizedPriority] || 'bg-gray-100 text-gray-800';
    
    return (
      <span className={`px-2 py-1 rounded-full text-xs font-medium ${colorClass}`}>
        {priority}
      </span>
    );
  };
  
  return (
    <div className="min-h-screen flex flex-col bg-background">
      <Navbar />
      
      <main className="flex-1 w-full px-4 sm:px-6 lg:px-8 pt-24 pb-20 flex justify-center">
        <div className="w-[900px] max-w-[900px] h-full bg-white p-6">
          <div className="text-center mb-8">
            <h1 className="text-3xl font-bold tracking-tight sm:text-4xl mb-2">Requirements Management</h1>
            <p className="text-muted-foreground max-w-2xl mx-auto">
              Submit your project requirements and help us better understand your needs
            </p>
          </div>
          
          {isLoggedIn && userRole.toLowerCase() === 'admin' ? (
            // Vista de administrador con pestañas
            <Tabs defaultValue={activeTab} onValueChange={setActiveTab} className="space-y-6">
              <TabsList className="w-full grid grid-cols-2 md:grid-cols-4">
                <TabsTrigger value="new">Nuevo requerimiento</TabsTrigger>
                <TabsTrigger value="list">Lista de requerimientos</TabsTrigger>
                <TabsTrigger value="stats">Estadísticas</TabsTrigger>
                <TabsTrigger value="settings">Configuración</TabsTrigger>
              </TabsList>
              
              <TabsContent value="new" className="pt-4">
                <Card>
                  <CardHeader>
                    <CardTitle className="flex items-center gap-2">
                      <FileText className="h-5 w-5" />
                      Nuevo requerimiento
                    </CardTitle>
                    <CardDescription>
                      Crea un nuevo requerimiento de proyecto
                    </CardDescription>
                  </CardHeader>
                  <CardContent>
                    <ClientRequirementForm />
                  </CardContent>
                </Card>
              </TabsContent>
              
              <TabsContent value="list" className="pt-4">
                <Card>
                  <CardHeader>
                    <CardTitle className="flex items-center gap-2">
                      <FileText className="h-5 w-5" />
                      Lista de requerimientos
                    </CardTitle>
                    <CardDescription>
                      Administra los requerimientos existentes
                    </CardDescription>
                    
                    <div className="flex flex-col md:flex-row gap-4 mt-4">
                      <div className="flex-1 relative">
                        <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
                        <Input
                          type="search"
                          placeholder="Buscar requerimientos..."
                          className="pl-8"
                          value={searchTerm}
                          onChange={(e) => setSearchTerm(e.target.value)}
                        />
                      </div>
                      <Select value={statusFilter} onValueChange={setStatusFilter}>
                        <SelectTrigger className="w-[180px]">
                          <SelectValue placeholder="Estado" />
                        </SelectTrigger>
                        <SelectContent>
                          <SelectItem value="">Todos</SelectItem>
                          <SelectItem value="propuesto">Propuesto</SelectItem>
                          <SelectItem value="en progreso">En progreso</SelectItem>
                          <SelectItem value="en pruebas">En pruebas</SelectItem>
                          <SelectItem value="completado">Completado</SelectItem>
                          <SelectItem value="cancelado">Cancelado</SelectItem>
                        </SelectContent>
                      </Select>
                      <Select value={priorityFilter} onValueChange={setPriorityFilter}>
                        <SelectTrigger className="w-[180px]">
                          <SelectValue placeholder="Prioridad" />
                        </SelectTrigger>
                        <SelectContent>
                          <SelectItem value="">Todas</SelectItem>
                          <SelectItem value="crítica">Crítica</SelectItem>
                          <SelectItem value="alta">Alta</SelectItem>
                          <SelectItem value="media">Media</SelectItem>
                          <SelectItem value="baja">Baja</SelectItem>
                        </SelectContent>
                      </Select>
                    </div>
                  </CardHeader>
                  <CardContent>
                    {isLoading ? (
                      <div className="text-center py-8">Cargando requerimientos...</div>
                    ) : requirements.length === 0 ? (
                      <div className="text-center py-8 text-muted-foreground">
                        No se encontraron requerimientos con los filtros aplicados.
                      </div>
                    ) : (
                      <div className="space-y-4">
                        {requirements.map((req) => (
                          <Card key={req.id} className="hover:bg-gray-50">
                            <CardContent className="p-4">
                              <div className="flex justify-between items-start mb-2">
                                <h3 className="font-medium">{req.title}</h3>
                                <div className="flex gap-2">
                                  {getStatusBadge(req.status)}
                                  {getPriorityBadge(req.priority)}
                                </div>
                              </div>
                              <p className="text-sm text-muted-foreground mb-2 line-clamp-2">
                                {req.description}
                              </p>
                              <div className="flex justify-between text-xs text-muted-foreground">
                                <span>Área: {req.projectArea}</span>
                                <span>Creado: {new Date(req.createdAt).toLocaleDateString()}</span>
                                <span>Por: {req.customerName || 'Anónimo'}</span>
                              </div>
                              <div className="mt-3 flex justify-end">
                                <Button variant="outline" size="sm" className="mr-2">
                                  Ver detalles
                                </Button>
                                <Button size="sm">
                                  Editar
                                </Button>
                              </div>
                            </CardContent>
                          </Card>
                        ))}
                      </div>
                    )}
                  </CardContent>
                </Card>
              </TabsContent>
              
              <TabsContent value="stats" className="pt-4">
                <Card>
                  <CardHeader>
                    <CardTitle>Estadísticas de requerimientos</CardTitle>
                    <CardDescription>
                      Visualiza datos y métricas sobre los requerimientos del proyecto
                    </CardDescription>
                  </CardHeader>
                  <CardContent className="flex justify-center">
                    <div className="text-center py-8 text-muted-foreground">
                      Las estadísticas estarán disponibles próximamente
                    </div>
                  </CardContent>
                </Card>
              </TabsContent>
              
              <TabsContent value="settings" className="pt-4">
                <Card>
                  <CardHeader>
                    <CardTitle>Configuración</CardTitle>
                    <CardDescription>
                      Personaliza las opciones de gestión de requerimientos
                    </CardDescription>
                  </CardHeader>
                  <CardContent className="flex justify-center">
                    <div className="text-center py-8 text-muted-foreground">
                      Las opciones de configuración estarán disponibles próximamente
                    </div>
                  </CardContent>
                </Card>
              </TabsContent>
            </Tabs>
          ) : (
            // Vista de cliente con solo el formulario
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <FileText className="h-5 w-5" />
                  Nuevo requerimiento
                </CardTitle>
                <CardDescription>
                  Envía un nuevo requerimiento de proyecto
                </CardDescription>
              </CardHeader>
              <CardContent>
                <ClientRequirementForm />
              </CardContent>
            </Card>
          )}
        </div>
      </main>
      
      <Footer />
    </div>
  );
}
