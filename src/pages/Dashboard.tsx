
import { useState, useEffect, useCallback } from 'react';
import { Link, useLocation } from 'react-router-dom';
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { 
  BarChart3, 
  ArrowRight, 
  Eye, 
  Clock, 
  LineChart, 
  CheckCircle2,
  ChevronDown,
  ListChecks,
  FileText,
  MessageSquare
} from "lucide-react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import Navbar from "@/components/layout/Navbar";
import { Badge } from "@/components/ui/badge";
import { toast } from "sonner";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { 
  Dialog, 
  DialogContent, 
  DialogHeader, 
  DialogTitle, 
  DialogFooter, 
  DialogDescription 
} from "@/components/ui/dialog";
import {
  Drawer,
  DrawerContent,
  DrawerDescription,
  DrawerHeader,
  DrawerTitle,
  DrawerTrigger,
  DrawerFooter,
  DrawerClose,
} from "@/components/ui/drawer";

interface SurveyItem {
  id: string;
  title: string;
  description: string;
  createdAt: string;
  responses: number;
  status: string;
}

interface SuggestionItem {
  id: string;
  content: string;
  customerName: string;
  createdAt: string;
  status: string;
}

interface RequirementItem {
  id: string;
  title: string;
  description: string;
  priority: string;
  createdAt: string;
  status: string;
}

interface UpdateStatusParams {
  id: string;
  type: string;
  newStatus: string;
}

interface ConfirmDialogState {
  open: boolean;
  id: string;
  type: string;
  newStatus: string;
  currentStatus: string;
}

const fetchLatestSurveys = async (): Promise<SurveyItem[]> => {
  // In a real application, this would fetch from an API
  return [
    {
      id: "1",
      title: "Encuesta de Satisfacción del Cliente",
      description: "Recopilar opiniones sobre la calidad de nuestro servicio al cliente",
      createdAt: "2023-12-15T12:00:00Z",
      responses: 8,
      status: "in-progress"
    },
    {
      id: "2",
      title: "Feedback del Nuevo Producto",
      description: "Obtener comentarios sobre el lanzamiento del nuevo producto",
      createdAt: "2023-12-10T10:00:00Z",
      responses: 12,
      status: "closed"
    },
    {
      id: "3",
      title: "Evaluación de la Interfaz de Usuario",
      description: "Evaluar la usabilidad de la nueva interfaz",
      createdAt: "2023-12-05T09:00:00Z",
      responses: 15,
      status: "pending"
    },
    {
      id: "4",
      title: "Preferencias de Características",
      description: "Conocer qué características son más importantes para los usuarios",
      createdAt: "2023-11-28T14:00:00Z",
      responses: 20,
      status: "in-progress"
    },
    {
      id: "5",
      title: "Encuesta de Experiencia de Usuario",
      description: "Evaluar la experiencia general del usuario con nuestra plataforma",
      createdAt: "2023-11-20T15:30:00Z",
      responses: 18,
      status: "closed"
    }
  ];
};

const fetchLatestSuggestions = async (): Promise<SuggestionItem[]> => {
  // In a real application, this would fetch from an API
  return [
    {
      id: "1",
      content: "Agregar modo oscuro al portal del cliente",
      customerName: "Juan Pérez",
      createdAt: "2023-12-10T09:30:00Z",
      status: "pending"
    },
    {
      id: "2",
      content: "Mejorar el rendimiento en dispositivos móviles",
      customerName: "María González",
      createdAt: "2023-12-08T14:15:00Z",
      status: "in-progress"
    },
    {
      id: "3",
      content: "Añadir opciones de filtrado en la búsqueda",
      customerName: "Carlos Rodríguez",
      createdAt: "2023-12-05T11:45:00Z",
      status: "closed"
    },
    {
      id: "4",
      content: "Implementar un sistema de notificaciones por email",
      customerName: "Ana Martínez",
      createdAt: "2023-12-01T16:20:00Z",
      status: "pending"
    },
    {
      id: "5",
      content: "Incluir tutoriales interactivos para nuevos usuarios",
      customerName: "Roberto Sánchez",
      createdAt: "2023-11-28T10:10:00Z",
      status: "in-progress"
    }
  ];
};

const fetchLatestRequirements = async (): Promise<RequirementItem[]> => {
  // In a real application, this would fetch from an API
  return [
    {
      id: "1",
      title: "Diseño responsivo para móviles",
      description: "La aplicación debe ser completamente responsiva en todos los dispositivos móviles",
      priority: "high",
      createdAt: "2023-12-05T14:20:00Z",
      status: "closed"
    },
    {
      id: "2",
      title: "Integración con redes sociales",
      description: "Permitir a los usuarios compartir encuestas en redes sociales",
      priority: "medium",
      createdAt: "2023-12-03T09:45:00Z",
      status: "in-progress"
    },
    {
      id: "3",
      title: "Exportación de datos a Excel",
      description: "Añadir opción para exportar resultados de encuestas a formato Excel",
      priority: "high",
      createdAt: "2023-11-29T16:30:00Z",
      status: "pending"
    },
    {
      id: "4",
      title: "Sistema de notificaciones en tiempo real",
      description: "Implementar notificaciones instantáneas cuando se complete una encuesta",
      priority: "low",
      createdAt: "2023-11-25T11:15:00Z",
      status: "pending"
    },
    {
      id: "5",
      title: "Optimización de rendimiento de la base de datos",
      description: "Mejorar los tiempos de respuesta para consultas complejas",
      priority: "high",
      createdAt: "2023-11-20T13:40:00Z",
      status: "in-progress"
    }
  ];
};

const updateStatus = async ({ id, type, newStatus }: UpdateStatusParams) => {
  console.log(`Actualizando ${type} con id ${id} a estado: ${newStatus}`);
  await new Promise(resolve => setTimeout(resolve, 500));
  return { id, type, status: newStatus };
};

const StatusBadge = ({ status }: { status: string }) => {
  switch (status) {
    case "pending":
      return (
        <Badge variant="outline" className="bg-amber-50 text-amber-700 border-amber-200">
          <Clock className="h-3 w-3 mr-1" /> Pendiente
        </Badge>
      );
    case "in-progress":
      return (
        <Badge variant="outline" className="bg-blue-50 text-blue-700 border-blue-200">
          <LineChart className="h-3 w-3 mr-1" /> En curso
        </Badge>
      );
    case "closed":
      return (
        <Badge variant="outline" className="bg-green-50 text-green-700 border-green-200">
          <CheckCircle2 className="h-3 w-3 mr-1" /> Cerrada
        </Badge>
      );
    default:
      return null;
  }
};

const PriorityBadge = ({ priority }: { priority: string }) => {
  switch (priority) {
    case "high":
      return (
        <Badge variant="outline" className="bg-red-50 text-red-700 border-red-200">
          Alta
        </Badge>
      );
    case "medium":
      return (
        <Badge variant="outline" className="bg-amber-50 text-amber-700 border-amber-200">
          Media
        </Badge>
      );
    case "low":
      return (
        <Badge variant="outline" className="bg-green-50 text-green-700 border-green-200">
          Baja
        </Badge>
      );
    default:
      return null;
  }
};

export default function Dashboard() {
  const queryClient = useQueryClient();
  const [dialogState, setDialogState] = useState<ConfirmDialogState>({
    open: false,
    id: "",
    type: "",
    newStatus: "",
    currentStatus: ""
  });
  
  const { data: latestSurveys, isLoading: loadingSurveys } = useQuery({
    queryKey: ['latestSurveys'],
    queryFn: fetchLatestSurveys
  });

  const { data: latestSuggestions, isLoading: loadingSuggestions } = useQuery({
    queryKey: ['latestSuggestions'],
    queryFn: fetchLatestSuggestions
  });

  const { data: latestRequirements, isLoading: loadingRequirements } = useQuery({
    queryKey: ['latestRequirements'],
    queryFn: fetchLatestRequirements
  });

  const updateStatusMutation = useMutation({
    mutationFn: updateStatus,
    onSuccess: (data) => {
      let queryKey;
      
      if (data.type === 'Survey') {
        queryKey = 'latestSurveys';
        queryClient.setQueryData([queryKey], (oldData: SurveyItem[] | undefined) => {
          if (!oldData) return oldData;
          return oldData.map(item => 
            item.id === data.id ? { ...item, status: data.status } : item
          );
        });
      } else if (data.type === 'Suggestion') {
        queryKey = 'latestSuggestions';
        queryClient.setQueryData([queryKey], (oldData: SuggestionItem[] | undefined) => {
          if (!oldData) return oldData;
          return oldData.map(item => 
            item.id === data.id ? { ...item, status: data.status } : item
          );
        });
      } else {
        queryKey = 'latestRequirements';
        queryClient.setQueryData([queryKey], (oldData: RequirementItem[] | undefined) => {
          if (!oldData) return oldData;
          return oldData.map(item => 
            item.id === data.id ? { ...item, status: data.status } : item
          );
        });
      }
      
      toast.success(`Estado actualizado a ${getStatusLabel(data.status)}`);
      
      // Close dialog by creating a new state object with open: false
      setDialogState({
        open: false,
        id: "",
        type: "",
        newStatus: "",
        currentStatus: ""
      });
    },
    onError: (error) => {
      console.error("Error al actualizar el estado:", error);
      toast.error("Error al actualizar el estado");
      
      // Close dialog by creating a new state object with open: false
      setDialogState({
        open: false,
        id: "",
        type: "",
        newStatus: "",
        currentStatus: ""
      });
    }
  });

  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    return new Intl.DateTimeFormat('es-ES', { 
      year: 'numeric', 
      month: 'short', 
      day: 'numeric' 
    }).format(date);
  };

  const getStatusLabel = (status: string) => {
    switch (status) {
      case "pending": return "Pendiente";
      case "in-progress": return "En curso";
      case "closed": return "Cerrada";
      default: return status;
    }
  };

  // Memoize this function to prevent unnecessary re-renders
  const handleStatusChange = useCallback((id: string, type: string, newStatus: string, currentStatus: string) => {
    if (newStatus === currentStatus) {
      return;
    }
    
    // Create a completely new state object to avoid partial updates
    setDialogState({
      open: true,
      id,
      type,
      newStatus,
      currentStatus
    });
  }, []);

  const confirmStatusChange = useCallback(() => {
    // Only proceed if we have valid data
    const { id, type, newStatus } = dialogState;
    if (id && type && newStatus) {
      updateStatusMutation.mutate({
        id,
        type,
        newStatus
      });
    }
  }, [dialogState, updateStatusMutation]);

  const closeDialog = useCallback(() => {
    // Create a completely new state object with open: false
    setDialogState({
      open: false,
      id: "",
      type: "",
      newStatus: "",
      currentStatus: ""
    });
  }, []);

  // Handle dialog open state changes
  const handleDialogOpenChange = useCallback((open: boolean) => {
    if (!open) {
      closeDialog();
    }
  }, [closeDialog]);

  return (
    <div className="min-h-screen bg-background">
      <Navbar />
      <div className="container mx-auto pt-20 pb-10 px-4 md:px-6">
        <h1 className="text-3xl font-bold tracking-tight mb-6">Panel de Control</h1>
        
        <Card className="shadow-sm">
          <CardHeader className="pb-2">
            <CardTitle className="text-xl">Vista Rápida de Elementos Recientes</CardTitle>
          </CardHeader>
          <CardContent>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4 mb-6">
              {/* Dropdown for Surveys */}
              <Drawer>
                <DrawerTrigger asChild>
                  <Button 
                    variant="outline" 
                    className="h-24 w-full justify-start flex-col items-start p-4 bg-blue-50 hover:bg-blue-100 border-blue-200"
                    type="button"
                  >
                    <div className="flex items-center mb-2">
                      <FileText className="h-5 w-5 mr-2 text-blue-700" />
                      <span className="font-medium text-blue-700">Últimas Encuestas</span>
                    </div>
                    <p className="text-sm text-blue-600 text-left">
                      Ver las {latestSurveys?.length || 0} encuestas más recientes
                    </p>
                  </Button>
                </DrawerTrigger>
                <DrawerContent>
                  <DrawerHeader className="text-left">
                    <DrawerTitle>Últimas Encuestas</DrawerTitle>
                    <DrawerDescription>
                      Las 5 encuestas más recientes en el sistema
                    </DrawerDescription>
                  </DrawerHeader>
                  <div className="px-4 py-2 space-y-4">
                    {loadingSurveys ? (
                      <p>Cargando encuestas...</p>
                    ) : (
                      latestSurveys?.map((survey) => (
                        <div key={survey.id} className="border rounded-md p-4">
                          <div className="flex justify-between items-start mb-2">
                            <div className="flex items-center gap-2">
                              <h3 className="font-medium">{survey.title}</h3>
                              <StatusBadge status={survey.status} />
                            </div>
                          </div>
                          <p className="text-sm text-muted-foreground mb-2">{survey.description}</p>
                          <div className="flex justify-between items-center">
                            <p className="text-xs text-muted-foreground">
                              Creada el {formatDate(survey.createdAt)} • {survey.responses} respuestas
                            </p>
                            <div className="flex space-x-2">
                              <DropdownMenu>
                                <DropdownMenuTrigger asChild>
                                  <Button 
                                    variant="outline" 
                                    size="sm" 
                                    className="h-8"
                                    type="button"
                                  >
                                    <span>Estado</span>
                                    <ChevronDown className="ml-1 h-4 w-4" />
                                  </Button>
                                </DropdownMenuTrigger>
                                <DropdownMenuContent align="end">
                                  <DropdownMenuItem 
                                    onClick={() => handleStatusChange(survey.id, "Survey", "pending", survey.status)}
                                    disabled={survey.status === "pending"}
                                  >
                                    <Clock className="mr-2 h-4 w-4" />
                                    <span>Pendiente</span>
                                  </DropdownMenuItem>
                                  <DropdownMenuItem 
                                    onClick={() => handleStatusChange(survey.id, "Survey", "in-progress", survey.status)}
                                    disabled={survey.status === "in-progress"}
                                  >
                                    <LineChart className="mr-2 h-4 w-4" />
                                    <span>En curso</span>
                                  </DropdownMenuItem>
                                  <DropdownMenuItem 
                                    onClick={() => handleStatusChange(survey.id, "Survey", "closed", survey.status)}
                                    disabled={survey.status === "closed"}
                                  >
                                    <CheckCircle2 className="mr-2 h-4 w-4" />
                                    <span>Cerrada</span>
                                  </DropdownMenuItem>
                                </DropdownMenuContent>
                              </DropdownMenu>
                              <Button 
                                variant="ghost" 
                                size="sm" 
                                className="h-8 w-8 p-0"
                                asChild
                              >
                                <Link to={`/survey/${survey.id}`}>
                                  <Eye className="h-4 w-4" />
                                </Link>
                              </Button>
                            </div>
                          </div>
                        </div>
                      ))
                    )}
                  </div>
                  <DrawerFooter className="flex-row justify-end space-x-2">
                    <DrawerClose asChild>
                      <Button variant="outline" type="button">Cerrar</Button>
                    </DrawerClose>
                    <Link to="/surveys">
                      <Button type="button">
                        Ver Todas
                        <ArrowRight className="ml-2 h-4 w-4" />
                      </Button>
                    </Link>
                  </DrawerFooter>
                </DrawerContent>
              </Drawer>

              {/* Dropdown for Suggestions */}
              <Drawer>
                <DrawerTrigger asChild>
                  <Button 
                    variant="outline" 
                    className="h-24 w-full justify-start flex-col items-start p-4 bg-amber-50 hover:bg-amber-100 border-amber-200"
                    type="button"
                  >
                    <div className="flex items-center mb-2">
                      <MessageSquare className="h-5 w-5 mr-2 text-amber-700" />
                      <span className="font-medium text-amber-700">Últimas Sugerencias</span>
                    </div>
                    <p className="text-sm text-amber-600 text-left">
                      Ver las {latestSuggestions?.length || 0} sugerencias más recientes
                    </p>
                  </Button>
                </DrawerTrigger>
                <DrawerContent>
                  <DrawerHeader className="text-left">
                    <DrawerTitle>Últimas Sugerencias</DrawerTitle>
                    <DrawerDescription>
                      Las 5 sugerencias más recientes en el sistema
                    </DrawerDescription>
                  </DrawerHeader>
                  <div className="px-4 py-2 space-y-4">
                    {loadingSuggestions ? (
                      <p>Cargando sugerencias...</p>
                    ) : (
                      latestSuggestions?.map((suggestion) => (
                        <div key={suggestion.id} className="border rounded-md p-4">
                          <div className="flex justify-between items-start mb-2">
                            <div className="flex items-center gap-2">
                              <h3 className="font-medium">{suggestion.content}</h3>
                              <StatusBadge status={suggestion.status} />
                            </div>
                          </div>
                          <div className="flex justify-between items-center">
                            <p className="text-xs text-muted-foreground">
                              De {suggestion.customerName} • {formatDate(suggestion.createdAt)}
                            </p>
                            <div className="flex space-x-2">
                              <DropdownMenu>
                                <DropdownMenuTrigger asChild>
                                  <Button 
                                    variant="outline" 
                                    size="sm" 
                                    className="h-8"
                                    type="button"
                                  >
                                    <span>Estado</span>
                                    <ChevronDown className="ml-1 h-4 w-4" />
                                  </Button>
                                </DropdownMenuTrigger>
                                <DropdownMenuContent align="end">
                                  <DropdownMenuItem 
                                    onClick={() => handleStatusChange(suggestion.id, "Suggestion", "pending", suggestion.status)}
                                    disabled={suggestion.status === "pending"}
                                  >
                                    <Clock className="mr-2 h-4 w-4" />
                                    <span>Pendiente</span>
                                  </DropdownMenuItem>
                                  <DropdownMenuItem 
                                    onClick={() => handleStatusChange(suggestion.id, "Suggestion", "in-progress", suggestion.status)}
                                    disabled={suggestion.status === "in-progress"}
                                  >
                                    <LineChart className="mr-2 h-4 w-4" />
                                    <span>En curso</span>
                                  </DropdownMenuItem>
                                  <DropdownMenuItem 
                                    onClick={() => handleStatusChange(suggestion.id, "Suggestion", "closed", suggestion.status)}
                                    disabled={suggestion.status === "closed"}
                                  >
                                    <CheckCircle2 className="mr-2 h-4 w-4" />
                                    <span>Cerrada</span>
                                  </DropdownMenuItem>
                                </DropdownMenuContent>
                              </DropdownMenu>
                              <Button 
                                variant="ghost" 
                                size="sm" 
                                className="h-8 w-8 p-0"
                                asChild
                                type="button"
                              >
                                <Link to="/suggestions">
                                  <Eye className="h-4 w-4" />
                                </Link>
                              </Button>
                            </div>
                          </div>
                        </div>
                      ))
                    )}
                  </div>
                  <DrawerFooter className="flex-row justify-end space-x-2">
                    <DrawerClose asChild>
                      <Button variant="outline" type="button">Cerrar</Button>
                    </DrawerClose>
                    <Link to="/suggestions">
                      <Button type="button">
                        Ver Todas
                        <ArrowRight className="ml-2 h-4 w-4" />
                      </Button>
                    </Link>
                  </DrawerFooter>
                </DrawerContent>
              </Drawer>

              {/* Dropdown for Requirements */}
              <Drawer>
                <DrawerTrigger asChild>
                  <Button 
                    variant="outline" 
                    className="h-24 w-full justify-start flex-col items-start p-4 bg-green-50 hover:bg-green-100 border-green-200"
                    type="button"
                  >
                    <div className="flex items-center mb-2">
                      <ListChecks className="h-5 w-5 mr-2 text-green-700" />
                      <span className="font-medium text-green-700">Últimos Requerimientos</span>
                    </div>
                    <p className="text-sm text-green-600 text-left">
                      Ver los {latestRequirements?.length || 0} requerimientos más recientes
                    </p>
                  </Button>
                </DrawerTrigger>
                <DrawerContent>
                  <DrawerHeader className="text-left">
                    <DrawerTitle>Últimos Requerimientos</DrawerTitle>
                    <DrawerDescription>
                      Los 5 requerimientos más recientes en el sistema
                    </DrawerDescription>
                  </DrawerHeader>
                  <div className="px-4 py-2 space-y-4">
                    {loadingRequirements ? (
                      <p>Cargando requerimientos...</p>
                    ) : (
                      latestRequirements?.map((requirement) => (
                        <div key={requirement.id} className="border rounded-md p-4">
                          <div className="flex justify-between items-start mb-2">
                            <div className="flex items-center gap-2">
                              <h3 className="font-medium">{requirement.title}</h3>
                              <StatusBadge status={requirement.status} />
                            </div>
                          </div>
                          <p className="text-sm text-muted-foreground mb-2">{requirement.description}</p>
                          <div className="flex justify-between items-center">
                            <div className="flex items-center gap-2">
                              <p className="text-xs text-muted-foreground">
                                {formatDate(requirement.createdAt)}
                              </p>
                              <PriorityBadge priority={requirement.priority} />
                            </div>
                            <div className="flex space-x-2">
                              <DropdownMenu>
                                <DropdownMenuTrigger asChild>
                                  <Button 
                                    variant="outline" 
                                    size="sm" 
                                    className="h-8"
                                    type="button"
                                  >
                                    <span>Estado</span>
                                    <ChevronDown className="ml-1 h-4 w-4" />
                                  </Button>
                                </DropdownMenuTrigger>
                                <DropdownMenuContent align="end">
                                  <DropdownMenuItem 
                                    onClick={() => handleStatusChange(requirement.id, "Requirement", "pending", requirement.status)}
                                    disabled={requirement.status === "pending"}
                                  >
                                    <Clock className="mr-2 h-4 w-4" />
                                    <span>Pendiente</span>
                                  </DropdownMenuItem>
                                  <DropdownMenuItem 
                                    onClick={() => handleStatusChange(requirement.id, "Requirement", "in-progress", requirement.status)}
                                    disabled={requirement.status === "in-progress"}
                                  >
                                    <LineChart className="mr-2 h-4 w-4" />
                                    <span>En curso</span>
                                  </DropdownMenuItem>
                                  <DropdownMenuItem 
                                    onClick={() => handleStatusChange(requirement.id, "Requirement", "closed", requirement.status)}
                                    disabled={requirement.status === "closed"}
                                  >
                                    <CheckCircle2 className="mr-2 h-4 w-4" />
                                    <span>Cerrada</span>
                                  </DropdownMenuItem>
                                </DropdownMenuContent>
                              </DropdownMenu>
                              <Button 
                                variant="ghost" 
                                size="sm" 
                                className="h-8 w-8 p-0"
                                asChild
                                type="button"
                              >
                                <Link to="/requirements">
                                  <Eye className="h-4 w-4" />
                                </Link>
                              </Button>
                            </div>
                          </div>
                        </div>
                      ))
                    )}
                  </div>
                  <DrawerFooter className="flex-row justify-end space-x-2">
                    <DrawerClose asChild>
                      <Button variant="outline" type="button">Cerrar</Button>
                    </DrawerClose>
                    <Link to="/requirements">
                      <Button type="button">
                        Ver Todos
                        <ArrowRight className="ml-2 h-4 w-4" />
                      </Button>
                    </Link>
                  </DrawerFooter>
                </DrawerContent>
              </Drawer>
            </div>
            
            <div className="flex justify-end space-x-2 mt-6">
              <Link to="/results">
                <Button size="sm" type="button">
                  <BarChart3 className="mr-2 h-4 w-4" />
                  Ver Análisis
                </Button>
              </Link>
              <Link to="/surveys">
                <Button variant="outline" size="sm" type="button">
                  Ver Todas las Encuestas
                  <ArrowRight className="ml-2 h-4 w-4" />
                </Button>
              </Link>
            </div>
          </CardContent>
        </Card>
      </div>

      <Dialog 
        open={dialogState.open} 
        onOpenChange={handleDialogOpenChange}
      >
        <DialogContent className="sm:max-w-md">
          <DialogHeader>
            <DialogTitle>Confirmar Cambio de Estado</DialogTitle>
            <DialogDescription>
              ¿Estás seguro de que deseas cambiar el estado?
            </DialogDescription>
          </DialogHeader>
          <div className="py-4">
            <p>¿Estás seguro de que deseas cambiar el estado de
              {dialogState.type === 'Survey' && ' esta encuesta '}
              {dialogState.type === 'Suggestion' && ' esta sugerencia '}
              {dialogState.type === 'Requirement' && ' este requerimiento '}
              de <strong>{getStatusLabel(dialogState.currentStatus)}</strong> a <strong>{getStatusLabel(dialogState.newStatus)}</strong>?</p>
          </div>
          <DialogFooter>
            <Button 
              variant="outline" 
              onClick={closeDialog}
              type="button"
            >
              Cancelar
            </Button>
            <Button 
              onClick={confirmStatusChange}
              disabled={updateStatusMutation.isPending}
              type="button"
            >
              {updateStatusMutation.isPending ? "Actualizando..." : "Confirmar"}
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
    </div>
  );
}
