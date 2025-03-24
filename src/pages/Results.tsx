import { useState, useEffect } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import DataVisualizations from "@/components/results/DataVisualizations";
import { BarChart, PieChart, Download, Filter, Calendar, Eye, Loader2 } from "lucide-react";
import { Button } from "@/components/ui/button";
import Navbar from '@/components/layout/Navbar';
import Footer from '@/components/layout/Footer';
import { cn } from '@/lib/utils';
import { useQuery } from '@tanstack/react-query';
import { useSurveyAnalytics } from '../application/hooks/useSurveyAnalytics';
import { Survey, SurveyStatistics } from '../domain/models/Survey';
import { toast } from 'sonner';

export default function Results() {
  const { surveys, isLoadingSurveys, fetchSurveyStatistics, fetchSurveyResponses } = useSurveyAnalytics();
  const [selectedSurveyId, setSelectedSurveyId] = useState<string>('');
  
  useEffect(() => {
    if (surveys && surveys.length > 0 && !selectedSurveyId) {
      setSelectedSurveyId(surveys[0].id);
    }
  }, [surveys, selectedSurveyId]);

  const { 
    data: statistics, 
    isLoading: isLoadingStats 
  } = useQuery({
    queryKey: ['surveyStatistics', selectedSurveyId],
    queryFn: () => fetchSurveyStatistics(selectedSurveyId),
    enabled: !!selectedSurveyId,
  });

  const { 
    data: responses, 
    isLoading: isLoadingResponses 
  } = useQuery({
    queryKey: ['surveyResponses', selectedSurveyId],
    queryFn: () => fetchSurveyResponses(selectedSurveyId),
    enabled: !!selectedSurveyId,
  });

  const selectedSurvey = surveys?.find(survey => survey.id === selectedSurveyId);
  
  const formatTime = (seconds: number) => {
    const minutes = Math.floor(seconds / 60);
    const remainingSeconds = seconds % 60;
    return `${minutes}m ${remainingSeconds}s`;
  };

  const handleExport = () => {
    toast.info("Export feature coming soon");
  };

  const handleFilter = () => {
    toast.info("Filter feature coming soon");
  };

  const handleViewSurvey = () => {
    if (selectedSurvey) {
      window.open(`/take-survey/${selectedSurvey.id}`, '_blank');
    }
  };

  if (isLoadingSurveys) {
    return (
      <div className="min-h-screen flex flex-col items-center justify-center bg-background">
        <Loader2 className="w-10 h-10 animate-spin text-primary" />
        <p className="mt-4 text-muted-foreground">Loading surveys...</p>
      </div>
    );
  }

  if (!surveys || surveys.length === 0) {
    return (
      <div className="min-h-screen flex flex-col bg-background">
        <Navbar />
        <main className="flex-1 w-full max-w-7xl mx-auto pt-24 px-6 pb-16">
          <div className="mb-8">
            <h1 className="text-3xl font-bold mb-2">Análisis de Resultados</h1>
            <p className="text-muted-foreground">
              Ver respuestas, analizar datos y exportar información de tus encuestas
            </p>
          </div>
          <Card className="p-8">
            <div className="text-center py-12">
              <h2 className="text-xl font-medium mb-2">No hay encuestas disponibles</h2>
              <p className="text-muted-foreground mb-6">Crea una encuesta para comenzar a recibir respuestas</p>
              <Button asChild>
                <a href="/create-survey">Crear Encuesta</a>
              </Button>
            </div>
          </Card>
        </main>
        <Footer />
      </div>
    );
  }

  return (
    <div className="min-h-screen flex flex-col bg-background">
      <Navbar />
      
      <main className="flex-1 w-full max-w-7xl mx-auto pt-24 px-6 pb-16">
        <div className="mb-8">
          <h1 className="text-3xl font-bold mb-2">Análisis de Resultados</h1>
          <p className="text-muted-foreground">
            Ver respuestas, analizar datos y exportar información de tus encuestas
          </p>
        </div>
        
        <div className="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
          <div className="md:col-span-1">
            <div className="sticky top-24">
              <Card>
                <CardHeader>
                  <CardTitle className="text-lg">Mis Encuestas</CardTitle>
                </CardHeader>
                <CardContent className="p-0">
                  <div className="divide-y">
                    {surveys.map((survey) => (
                      <button
                        key={survey.id}
                        className={cn(
                          "w-full text-left px-4 py-3 hover:bg-accent transition-colors",
                          selectedSurveyId === survey.id ? "bg-accent" : ""
                        )}
                        onClick={() => setSelectedSurveyId(survey.id)}
                      >
                        <div className="font-medium truncate">{survey.title}</div>
                        <div className="text-sm text-muted-foreground mt-1">
                          {statistics?.totalResponses || 0} respuestas
                        </div>
                      </button>
                    ))}
                  </div>
                </CardContent>
              </Card>
            </div>
          </div>
          
          <div className="md:col-span-3 space-y-6">
            {selectedSurvey && (
              <>
                <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 border-b pb-4">
                  <div>
                    <h2 className="text-2xl font-bold">{selectedSurvey.title}</h2>
                    <p className="text-muted-foreground">{selectedSurvey.description}</p>
                  </div>
                  <div className="flex gap-2">
                    <Button variant="outline" size="sm" onClick={handleFilter}>
                      <Filter className="mr-2 h-4 w-4" /> Filtrar
                    </Button>
                    <Button variant="outline" size="sm" onClick={handleExport}>
                      <Download className="mr-2 h-4 w-4" /> Exportar
                    </Button>
                    <Button size="sm" onClick={handleViewSurvey}>
                      <Eye className="mr-2 h-4 w-4" /> Ver Encuesta
                    </Button>
                  </div>
                </div>
                
                {isLoadingStats ? (
                  <div className="h-40 flex items-center justify-center">
                    <Loader2 className="w-8 h-8 animate-spin text-primary" />
                    <span className="ml-2">Cargando estadísticas...</span>
                  </div>
                ) : (
                  <>
                    <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
                      <Card>
                        <CardContent className="pt-6">
                          <div className="flex items-start">
                            <div className="p-2 rounded-md bg-primary/10 text-primary mr-4">
                              <BarChart className="h-6 w-6" />
                            </div>
                            <div>
                              <div className="text-sm text-muted-foreground">Total Respuestas</div>
                              <div className="text-2xl font-bold">{statistics?.totalResponses || 0}</div>
                            </div>
                          </div>
                        </CardContent>
                      </Card>
                      
                      <Card>
                        <CardContent className="pt-6">
                          <div className="flex items-start">
                            <div className="p-2 rounded-md bg-primary/10 text-primary mr-4">
                              <PieChart className="h-6 w-6" />
                            </div>
                            <div>
                              <div className="text-sm text-muted-foreground">Tasa de Finalización</div>
                              <div className="text-2xl font-bold">{Math.round(statistics?.completionRate || 0)}%</div>
                            </div>
                          </div>
                        </CardContent>
                      </Card>
                      
                      <Card>
                        <CardContent className="pt-6">
                          <div className="flex items-start">
                            <div className="p-2 rounded-md bg-primary/10 text-primary mr-4">
                              <Calendar className="h-6 w-6" />
                            </div>
                            <div>
                              <div className="text-sm text-muted-foreground">Tiempo Promedio</div>
                              <div className="text-2xl font-bold">{formatTime(Math.round(statistics?.averageCompletionTime || 0))}</div>
                            </div>
                          </div>
                        </CardContent>
                      </Card>
                    </div>
                    
                    <Tabs defaultValue="charts" className="w-full">
                      <TabsList className="grid grid-cols-3 w-full max-w-md mb-6">
                        <TabsTrigger value="charts">Gráficos</TabsTrigger>
                        <TabsTrigger value="responses">Respuestas</TabsTrigger>
                        <TabsTrigger value="insights">Estadísticas</TabsTrigger>
                      </TabsList>
                      
                      <TabsContent value="charts" className="space-y-6 animate-fade-in">
                        {responses && selectedSurvey && (
                          <DataVisualizations survey={selectedSurvey} responses={responses} />
                        )}
                      </TabsContent>
                      
                      <TabsContent value="responses" className="animate-fade-in">
                        <Card>
                          <CardHeader>
                            <CardTitle>Respuestas Individuales</CardTitle>
                          </CardHeader>
                          <CardContent>
                            {isLoadingResponses ? (
                              <div className="flex justify-center py-12">
                                <Loader2 className="h-8 w-8 animate-spin text-primary" />
                              </div>
                            ) : responses && responses.length > 0 ? (
                              <div className="border rounded-md overflow-hidden">
                                <table className="w-full">
                                  <thead>
                                    <tr className="bg-muted/50">
                                      <th className="px-4 py-3 text-left font-medium text-muted-foreground">ID</th>
                                      <th className="px-4 py-3 text-left font-medium text-muted-foreground">Cliente</th>
                                      <th className="px-4 py-3 text-left font-medium text-muted-foreground">Enviado</th>
                                      <th className="px-4 py-3 text-left font-medium text-muted-foreground">Duración</th>
                                      <th className="px-4 py-3 text-left font-medium text-muted-foreground"></th>
                                    </tr>
                                  </thead>
                                  <tbody className="divide-y">
                                    {responses.map((response) => (
                                      <tr key={response.id} className="hover:bg-muted/30 transition-colors">
                                        <td className="px-4 py-3">{response.id}</td>
                                        <td className="px-4 py-3">
                                          <div className="font-medium">{response.respondentName}</div>
                                          <div className="text-xs text-muted-foreground">{response.respondentEmail}</div>
                                        </td>
                                        <td className="px-4 py-3">
                                          {new Date(response.submittedAt).toLocaleDateString()}
                                        </td>
                                        <td className="px-4 py-3">{formatTime(response.completionTime || 0)}</td>
                                        <td className="px-4 py-3 text-right">
                                          <Button variant="ghost" size="sm">Ver Detalles</Button>
                                        </td>
                                      </tr>
                                    ))}
                                  </tbody>
                                </table>
                              </div>
                            ) : (
                              <p className="text-center py-6 text-muted-foreground">
                                No hay respuestas para esta encuesta todavía
                              </p>
                            )}
                          </CardContent>
                        </Card>
                      </TabsContent>
                      
                      <TabsContent value="insights" className="animate-fade-in">
                        <Card>
                          <CardHeader>
                            <CardTitle>Estadísticas de la Encuesta</CardTitle>
                          </CardHeader>
                          <CardContent>
                            <div className="space-y-6">
                              <div>
                                <h3 className="text-lg font-medium mb-2">Tendencias de Respuestas</h3>
                                <div className="bg-muted/30 p-4 rounded-md">
                                  <p className="text-sm text-muted-foreground">
                                    La visualización de tendencias aparecerá aquí cuando haya suficientes respuestas para analizar patrones a lo largo del tiempo.
                                  </p>
                                </div>
                              </div>
                              
                              <div>
                                <h3 className="text-lg font-medium mb-2">Hallazgos Clave</h3>
                                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                                  {statistics && statistics.questionStats.length > 0 ? (
                                    <>
                                      <div className="border rounded-md p-4">
                                        <div className="text-sm font-medium mb-1">Respuesta más seleccionada</div>
                                        <div className="text-muted-foreground">
                                          {getTopResponse(statistics).answer || "Sin datos suficientes"}
                                        </div>
                                      </div>
                                      
                                      <div className="border rounded-md p-4">
                                        <div className="text-sm font-medium mb-1">Respuesta menos seleccionada</div>
                                        <div className="text-muted-foreground">
                                          {getLeastResponse(statistics).answer || "Sin datos suficientes"}
                                        </div>
                                      </div>
                                      
                                      <div className="border rounded-md p-4">
                                        <div className="text-sm font-medium mb-1">Tasa de respuesta</div>
                                        <div className="text-muted-foreground">
                                          {Math.round(statistics.completionRate)}%
                                        </div>
                                      </div>
                                      
                                      <div className="border rounded-md p-4">
                                        <div className="text-sm font-medium mb-1">Tiempo promedio</div>
                                        <div className="text-muted-foreground">
                                          {formatTime(Math.round(statistics.averageCompletionTime))}
                                        </div>
                                      </div>
                                    </>
                                  ) : (
                                    <p className="col-span-2 text-center py-4 text-muted-foreground">
                                      No hay suficientes datos para mostrar estadísticas
                                    </p>
                                  )}
                                </div>
                              </div>
                            </div>
                          </CardContent>
                        </Card>
                      </TabsContent>
                    </Tabs>
                  </>
                )}
              </>
            )}
          </div>
        </div>
      </main>
      
      <Footer />
    </div>
  );
}

function getTopResponse(statistics: SurveyStatistics) {
  let topResponse = { answer: "", count: 0, percentage: 0 };
  
  for (const question of statistics.questionStats) {
    if (question.responseDistribution) {
      Object.entries(question.responseDistribution).forEach(([answer, data]) => {
        if (data.count > topResponse.count) {
          topResponse = { answer, count: data.count, percentage: data.percentage };
        }
      });
    }
  }
  
  return topResponse;
}

function getLeastResponse(statistics: SurveyStatistics) {
  if (statistics.questionStats.length === 0) {
    return { answer: "", count: 0, percentage: 0 };
  }
  
  let leastResponse = { answer: "", count: Infinity, percentage: 0 };
  
  for (const question of statistics.questionStats) {
    if (question.responseDistribution) {
      Object.entries(question.responseDistribution).forEach(([answer, data]) => {
        if (data.count < leastResponse.count) {
          leastResponse = { answer, count: data.count, percentage: data.percentage };
        }
      });
    }
  }
  
  return leastResponse.count === Infinity 
    ? { answer: "", count: 0, percentage: 0 }
    : leastResponse;
}
