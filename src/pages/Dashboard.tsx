import { Link } from "react-router-dom";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { 
  BarChart3, 
  ArrowRight, 
  Clock, 
  LineChart, 
  CheckCircle2,
  ChevronDown
} from "lucide-react";
import { useQuery } from "@tanstack/react-query";
import Navbar from "@/components/layout/Navbar";
import { Badge } from "@/components/ui/badge";
import { cn } from "@/lib/utils";
import { useState } from "react";
import { toast } from "sonner";
import { useSurveyAnalytics } from "@/application/hooks/useSurveyAnalytics";
import { Suggestion } from "@/types/suggestions";
import { Requirement } from "@/types/requirements";
import { Survey, SurveyResponse } from "@/domain/models/Survey";
import { SupabaseSurveyResponseRepository } from "@/infrastructure/repositories/SupabaseSurveyResponseRepository";
import { SupabaseSurveyRepository } from "@/infrastructure/repositories/SupabaseSurveyRepository";

const surveyRepository = new SupabaseSurveyRepository();
const surveyResponseRepository = new SupabaseSurveyResponseRepository();

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

const translatePriority = (priority: string): string => {
  switch (priority.toLowerCase()) {
    case "high":
      return "Alta";
    case "medium":
      return "Media";
    case "low":
      return "Baja";
    default:
      return priority;
  }
};

const CollapsibleSection = ({ 
  title, 
  children, 
  expanded, 
  onToggle 
}: { 
  title: React.ReactNode, 
  children: React.ReactNode, 
  expanded: boolean, 
  onToggle: () => void 
}) => {
  return (
    <div className="border-b last:border-b-0">
      <div 
        className="w-full px-6 py-4 flex justify-between items-center hover:bg-gray-50 transition-colors cursor-pointer"
        onClick={onToggle}
      >
        <div>{title}</div>
        <ChevronDown className={cn(
          "h-5 w-5 text-gray-400 transition-transform",
          expanded && "transform rotate-180"
        )} />
      </div>
      <div className={cn(
        "bg-gray-50 px-6 py-3 grid transition-all",
        expanded ? "grid-rows-[1fr] opacity-100" : "grid-rows-[0fr] opacity-0"
      )}>
        <div className="overflow-hidden">
          {children}
        </div>
      </div>
    </div>
  );
};

export default function Dashboard() {
  const [expandedSections, setExpandedSections] = useState({
    surveys: false,
    suggestions: false,
    requirements: false
  });

  const toggleSection = (section: keyof typeof expandedSections) => {
    setExpandedSections(prev => ({
      ...prev,
      [section]: !prev[section]
    }));
  };

  const { 
    surveys, 
    isLoadingSurveys: isLoadingSurvey,
    surveysError
  } = useSurveyAnalytics();
  
  const latestSurvey = surveys && surveys.length > 0 
    ? surveys.sort((a, b) => 
        new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
      )[0] 
    : null;

  const { data: recentResponses, isLoading: isLoadingResponses } = useQuery({
    queryKey: ['recentResponses', latestSurvey?.id],
    queryFn: async () => {
      if (!latestSurvey?.id) return [];
      return surveyResponseRepository.getResponsesBySurveyId(latestSurvey.id);
    },
    enabled: expandedSections.surveys && !!latestSurvey?.id
  });

  const { data: suggestions, isLoading: isLoadingSuggestions } = useQuery({
    queryKey: ['suggestions'],
    queryFn: async () => {
      const response = await fetch('/api/suggestions');
      if (!response.ok) {
        throw new Error('Error fetching suggestions');
      }
      return response.json() as Promise<Suggestion[]>;
    },
    enabled: false
  });

  const latestSuggestion = suggestions && suggestions.length > 0 
    ? suggestions.sort((a, b) => 
        new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
      )[0] 
    : null;

  const { data: requirements, isLoading: isLoadingRequirements } = useQuery({
    queryKey: ['requirements'],
    queryFn: async () => {
      const response = await fetch('/api/requirements');
      if (!response.ok) {
        throw new Error('Error fetching requirements');
      }
      return response.json() as Promise<Requirement[]>;
    },
    enabled: false
  });

  const latestRequirement = requirements && requirements.length > 0 
    ? requirements.sort((a, b) => 
        new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
      )[0] 
    : null;

  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    return new Intl.DateTimeFormat('es-ES', { 
      year: 'numeric', 
      month: 'short', 
      day: 'numeric' 
    }).format(date);
  };

  const handleUpdateStatus = async (itemType: string, id: string, status: string) => {
    try {
      console.log(`Actualizando estado de ${itemType} ${id} a ${status}`);
      
      toast.success(`Estado del ${itemType} actualizado correctamente`);
    } catch (error) {
      toast.error(`Error al actualizar el estado: ${(error as Error).message}`);
    }
  };

  const recentSuggestions = !suggestions ? [
    {
      id: "1",
      content: "Agregar modo oscuro al portal del cliente",
      customerName: "Juan Pérez",
      createdAt: "2023-12-10T09:30:00Z",
      status: "pending"
    },
    {
      id: "2",
      content: "Mejorar la velocidad de carga de las páginas",
      customerName: "María López",
      createdAt: "2023-12-09T15:20:00Z",
      status: "in-progress"
    },
    {
      id: "3",
      content: "Añadir más opciones de pago en la checkout",
      customerName: "Carlos Gómez",
      createdAt: "2023-12-08T11:45:00Z",
      status: "closed"
    }
  ] : [];

  const recentRequirements = !requirements ? [
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
      title: "Implementar autenticación con Google",
      description: "Permitir a los usuarios iniciar sesión con sus cuentas de Google",
      priority: "medium",
      createdAt: "2023-12-04T10:15:00Z",
      status: "in-progress"
    },
    {
      id: "3",
      title: "Optimizar rendimiento en navegadores antiguos",
      description: "Mejorar la compatibilidad con IE11 y otros navegadores obsoletos",
      priority: "low",
      createdAt: "2023-12-03T09:30:00Z",
      status: "pending"
    }
  ] : [];

  return (
    <div className="min-h-screen bg-background">
      <Navbar />
      <div className="container mx-auto pt-20 pb-10 px-4 md:px-6">
        <h1 className="text-3xl font-bold tracking-tight mb-6">Panel de Control</h1>
        
        <Card className="shadow-sm">
          <CardHeader className="pb-2">
            <CardTitle className="text-xl">Vista Rápida de Elementos Recientes</CardTitle>
          </CardHeader>
          <CardContent className="p-0">
            <CollapsibleSection
              expanded={expandedSections.surveys}
              onToggle={() => toggleSection('surveys')}
              title={
                <div>
                  <div className="flex items-center gap-2 mb-1">
                    <h3 className="font-medium">Encuestas</h3>
                    {isLoadingSurvey ? (
                      <div className="w-20 h-5 bg-gray-200 animate-pulse rounded-full"></div>
                    ) : latestSurvey ? (
                      <StatusBadge status={latestSurvey.status || "active"} />
                    ) : null}
                  </div>
                  {isLoadingSurvey ? (
                    <div className="space-y-2">
                      <div className="w-3/4 h-5 bg-gray-200 animate-pulse rounded"></div>
                      <div className="w-1/2 h-4 bg-gray-200 animate-pulse rounded"></div>
                    </div>
                  ) : latestSurvey ? (
                    <>
                      <p className="font-semibold">{latestSurvey.title}</p>
                      <p className="text-xs text-muted-foreground">
                        Creada {formatDate(latestSurvey.createdAt)} • {latestSurvey.responseCount || 0} respuestas
                      </p>
                    </>
                  ) : (
                    <p className="text-gray-500">No hay encuestas disponibles</p>
                  )}
                </div>
              }
            >
              <div className="space-y-3">
                <h4 className="text-sm font-medium mb-2">Últimas 5 respuestas</h4>
                {isLoadingResponses ? (
                  <div className="space-y-2">
                    {[...Array(3)].map((_, i) => (
                      <div key={i} className="bg-white p-3 rounded border">
                        <div className="flex justify-between">
                          <div className="w-1/3 h-4 bg-gray-200 animate-pulse rounded"></div>
                          <div className="w-1/4 h-4 bg-gray-200 animate-pulse rounded"></div>
                        </div>
                        <div className="w-2/3 h-4 mt-2 bg-gray-200 animate-pulse rounded"></div>
                      </div>
                    ))}
                  </div>
                ) : recentResponses && recentResponses.length > 0 ? (
                  <div className="space-y-2">
                    {recentResponses.slice(0, 5).map(response => (
                      <div key={response.id} className="bg-white p-3 rounded border text-sm">
                        <div className="flex justify-between">
                          <span className="font-medium">{response.respondentName}</span>
                          <span className="text-gray-500 text-xs">{formatDate(response.submittedAt)}</span>
                        </div>
                        <p className="text-gray-600">{latestSurvey?.title}</p>
                      </div>
                    ))}
                  </div>
                ) : (
                  <p className="text-sm text-gray-500">No hay respuestas recientes</p>
                )}
                <div className="mt-3 text-right">
                  <Link to="/surveys" className="text-sm text-blue-600 hover:underline inline-flex items-center">
                    Ver todas las encuestas <ArrowRight className="ml-1 h-3 w-3" />
                  </Link>
                </div>
              </div>
            </CollapsibleSection>

            <CollapsibleSection
              expanded={expandedSections.suggestions}
              onToggle={() => toggleSection('suggestions')}
              title={
                <div>
                  <div className="flex items-center gap-2 mb-1">
                    <h3 className="font-medium">Sugerencias</h3>
                    {isLoadingSuggestions ? (
                      <div className="w-20 h-5 bg-gray-200 animate-pulse rounded-full"></div>
                    ) : latestSuggestion ? (
                      <StatusBadge status={latestSuggestion.status} />
                    ) : (
                      <StatusBadge status="pending" />
                    )}
                  </div>
                  {isLoadingSuggestions ? (
                    <div className="space-y-2">
                      <div className="w-3/4 h-5 bg-gray-200 animate-pulse rounded"></div>
                      <div className="w-1/2 h-4 bg-gray-200 animate-pulse rounded"></div>
                    </div>
                  ) : latestSuggestion ? (
                    <>
                      <p className="font-semibold">{latestSuggestion.content}</p>
                      <p className="text-xs text-muted-foreground">
                        De {latestSuggestion.customerName} • {formatDate(latestSuggestion.createdAt)}
                      </p>
                    </>
                  ) : (
                    <>
                      <p className="font-semibold">{recentSuggestions[0]?.content}</p>
                      <p className="text-xs text-muted-foreground">
                        De {recentSuggestions[0]?.customerName} • {formatDate(recentSuggestions[0]?.createdAt)}
                      </p>
                    </>
                  )}
                </div>
              }
            >
              <div className="space-y-3">
                <h4 className="text-sm font-medium mb-2">Últimas 5 sugerencias</h4>
                {isLoadingSuggestions ? (
                  <div className="space-y-2">
                    {[...Array(3)].map((_, i) => (
                      <div key={i} className="bg-white p-3 rounded border">
                        <div className="flex justify-between">
                          <div className="w-1/3 h-4 bg-gray-200 animate-pulse rounded"></div>
                          <div className="w-1/4 h-4 bg-gray-200 animate-pulse rounded"></div>
                        </div>
                        <div className="w-2/3 h-4 mt-2 bg-gray-200 animate-pulse rounded"></div>
                      </div>
                    ))}
                  </div>
                ) : suggestions && suggestions.length > 0 ? (
                  <div className="space-y-2">
                    {suggestions.slice(0, 5).map(suggestion => (
                      <div key={suggestion.id} className="bg-white p-3 rounded border text-sm">
                        <div className="flex justify-between">
                          <span className="font-medium">{suggestion.customerName}</span>
                          <span className="text-gray-500 text-xs">{formatDate(suggestion.createdAt)}</span>
                        </div>
                        <p className="text-gray-600">
                          {suggestion.content.length > 60 
                            ? `${suggestion.content.substring(0, 60)}...` 
                            : suggestion.content}
                        </p>
                        <div className="flex justify-end mt-2">
                          <StatusBadge status={suggestion.status} />
                        </div>
                      </div>
                    ))}
                  </div>
                ) : (
                  <div className="space-y-2">
                    {recentSuggestions.map(suggestion => (
                      <div key={suggestion.id} className="bg-white p-3 rounded border text-sm">
                        <div className="flex justify-between">
                          <span className="font-medium">{suggestion.customerName}</span>
                          <span className="text-gray-500 text-xs">{formatDate(suggestion.createdAt)}</span>
                        </div>
                        <p className="text-gray-600">
                          {suggestion.content.length > 60 
                            ? `${suggestion.content.substring(0, 60)}...` 
                            : suggestion.content}
                        </p>
                        <div className="flex justify-end mt-2">
                          <StatusBadge status={suggestion.status} />
                        </div>
                      </div>
                    ))}
                  </div>
                )}
                <div className="mt-3 text-right">
                  <Link to="/suggestions" className="text-sm text-blue-600 hover:underline inline-flex items-center">
                    Ver todas las sugerencias <ArrowRight className="ml-1 h-3 w-3" />
                  </Link>
                </div>
              </div>
            </CollapsibleSection>

            <CollapsibleSection
              expanded={expandedSections.requirements}
              onToggle={() => toggleSection('requirements')}
              title={
                <div>
                  <div className="flex items-center gap-2 mb-1">
                    <h3 className="font-medium">Requerimientos</h3>
                    {isLoadingRequirements ? (
                      <div className="w-20 h-5 bg-gray-200 animate-pulse rounded-full"></div>
                    ) : latestRequirement ? (
                      <StatusBadge status={latestRequirement.status} />
                    ) : (
                      <StatusBadge status="pending" />
                    )}
                  </div>
                  {isLoadingRequirements ? (
                    <div className="space-y-2">
                      <div className="w-3/4 h-5 bg-gray-200 animate-pulse rounded"></div>
                      <div className="w-1/2 h-4 bg-gray-200 animate-pulse rounded"></div>
                    </div>
                  ) : latestRequirement ? (
                    <>
                      <p className="font-semibold">{latestRequirement.title}</p>
                      <p className="text-xs text-muted-foreground">
                        Prioridad: {translatePriority(latestRequirement.priority)} • {formatDate(latestRequirement.createdAt)}
                      </p>
                    </>
                  ) : (
                    <>
                      <p className="font-semibold">{recentRequirements[0]?.title}</p>
                      <p className="text-xs text-muted-foreground">
                        Prioridad: {translatePriority(recentRequirements[0]?.priority)} • {formatDate(recentRequirements[0]?.createdAt)}
                      </p>
                    </>
                  )}
                </div>
              }
            >
              <div className="space-y-3">
                <h4 className="text-sm font-medium mb-2">Últimos 5 requerimientos</h4>
                {isLoadingRequirements ? (
                  <div className="space-y-2">
                    {[...Array(3)].map((_, i) => (
                      <div key={i} className="bg-white p-3 rounded border">
                        <div className="flex justify-between">
                          <div className="w-1/3 h-4 bg-gray-200 animate-pulse rounded"></div>
                          <div className="w-1/4 h-4 bg-gray-200 animate-pulse rounded"></div>
                        </div>
                        <div className="w-2/3 h-4 mt-2 bg-gray-200 animate-pulse rounded"></div>
                      </div>
                    ))}
                  </div>
                ) : requirements && requirements.length > 0 ? (
                  <div className="space-y-2">
                    {requirements.slice(0, 5).map(requirement => (
                      <div key={requirement.id} className="bg-white p-3 rounded border text-sm">
                        <div className="flex justify-between">
                          <span className="font-medium">{requirement.title}</span>
                          <Badge className={cn(
                            "text-xs",
                            requirement.priority === 'high' 
                              ? "bg-red-50 text-red-700 border-red-200" 
                              : requirement.priority === 'medium' 
                                ? "bg-amber-50 text-amber-700 border-amber-200"
                                : "bg-green-50 text-green-700 border-green-200"
                          )}>
                            {translatePriority(requirement.priority)}
                          </Badge>
                        </div>
                        <p className="text-gray-600">
                          {requirement.description && requirement.description.length > 60 
                            ? `${requirement.description.substring(0, 60)}...` 
                            : requirement.description}
                        </p>
                        <div className="flex justify-end mt-2">
                          <StatusBadge status={requirement.status} />
                        </div>
                      </div>
                    ))}
                  </div>
                ) : (
                  <div className="space-y-2">
                    {recentRequirements.map(requirement => (
                      <div key={requirement.id} className="bg-white p-3 rounded border text-sm">
                        <div className="flex justify-between">
                          <span className="font-medium">{requirement.title}</span>
                          <Badge className={cn(
                            "text-xs",
                            requirement.priority === 'high' 
                              ? "bg-red-50 text-red-700 border-red-200" 
                              : requirement.priority === 'medium' 
                                ? "bg-amber-50 text-amber-700 border-amber-200"
                                : "bg-green-50 text-green-700 border-green-200"
                          )}>
                            {translatePriority(requirement.priority)}
                          </Badge>
                        </div>
                        <p className="text-gray-600">
                          {requirement.description && requirement.description.length > 60 
                            ? `${requirement.description.substring(0, 60)}...` 
                            : requirement.description}
                        </p>
                        <div className="flex justify-end mt-2">
                          <StatusBadge status={requirement.status} />
                        </div>
                      </div>
                    ))}
                  </div>
                )}
                <div className="mt-3 text-right">
                  <Link to="/requirements" className="text-sm text-blue-600 hover:underline inline-flex items-center">
                    Ver todos los requerimientos <ArrowRight className="ml-1 h-3 w-3" />
                  </Link>
                </div>
              </div>
            </CollapsibleSection>
          </CardContent>

          <div className="px-6 py-4 bg-gray-50 border-t">
            <div className="flex justify-end space-x-2">
              <Link to="/results">
                <Button size="sm">
                  <BarChart3 className="mr-2 h-4 w-4" />
                  Ver Análisis
                </Button>
              </Link>
              <Link to="/surveys">
                <Button variant="outline" size="sm">
                  Ver Todas las Encuestas
                  <ArrowRight className="ml-2 h-4 w-4" />
                </Button>
              </Link>
            </div>
          </div>
        </Card>
      </div>
    </div>
  );
}
