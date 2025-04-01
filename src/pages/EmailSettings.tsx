
import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { useToast } from "@/components/ui/use-toast";
import { ArrowLeft, Save, List } from 'lucide-react';
import Navbar from '@/components/layout/Navbar';
import Footer from '@/components/layout/Footer';
import EmailDeliverySettings from '@/components/survey/EmailDeliverySettings';
import { DeliveryConfig, Survey } from '@/types/surveyTypes';
import { Table, TableHeader, TableBody, TableHead, TableRow, TableCell } from "@/components/ui/table";
import { useSurveyAnalytics } from '@/application/hooks/useSurveyAnalytics';
import { Badge } from '@/components/ui/badge';

export default function EmailSettings() {
  const navigate = useNavigate();
  const { toast } = useToast();
  const [deliveryConfig, setDeliveryConfig] = useState<DeliveryConfig>({
    type: 'manual',
    emailAddresses: [],
  });
  
  const [selectedSurveyId, setSelectedSurveyId] = useState<string | null>(null);
  const { surveys, isLoadingSurveys, updateDeliveryConfig, isUpdatingDeliveryConfig } = useSurveyAnalytics();
  
  // Cargar configuración guardada al iniciar
  useEffect(() => {
    const savedConfig = localStorage.getItem('emailDeliveryConfig');
    if (savedConfig) {
      try {
        setDeliveryConfig(JSON.parse(savedConfig));
      } catch (error) {
        console.error('Error parsing saved config:', error);
      }
    }
  }, []);

  // Cuando se selecciona una encuesta, cargar su configuración
  useEffect(() => {
    if (selectedSurveyId && surveys) {
      const selectedSurvey = surveys.find(s => s.id === selectedSurveyId);
      if (selectedSurvey && selectedSurvey.deliveryConfig) {
        setDeliveryConfig(selectedSurvey.deliveryConfig);
      } else {
        // Si la encuesta no tiene configuración, usar la global
        const savedConfig = localStorage.getItem('emailDeliveryConfig');
        if (savedConfig) {
          try {
            setDeliveryConfig(JSON.parse(savedConfig));
          } catch (error) {
            console.error('Error parsing saved config:', error);
          }
        }
      }
    }
  }, [selectedSurveyId, surveys]);

  const handleSaveSettings = async () => {
    if (selectedSurveyId) {
      // Guardar configuración para una encuesta específica
      try {
        await updateDeliveryConfig(selectedSurveyId, deliveryConfig);
        toast({
          title: "Configuración guardada",
          description: "La configuración de entrega de emails ha sido guardada para esta encuesta"
        });
      } catch (error) {
        toast({
          title: "Error",
          description: "No se pudo guardar la configuración",
          variant: "destructive"
        });
        console.error("Error saving survey delivery config:", error);
      }
    } else {
      // Guardar configuración global
      localStorage.setItem('emailDeliveryConfig', JSON.stringify(deliveryConfig));
      
      toast({
        title: "Configuración global guardada",
        description: "La configuración de entrega de emails ha sido guardada exitosamente"
      });
    }
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('es-ES', {
      year: 'numeric',
      month: 'short',
      day: 'numeric'
    });
  };

  const handleSelectSurvey = (surveyId: string) => {
    setSelectedSurveyId(surveyId);
  };

  const handleResetToGlobal = () => {
    setSelectedSurveyId(null);
    const savedConfig = localStorage.getItem('emailDeliveryConfig');
    if (savedConfig) {
      try {
        setDeliveryConfig(JSON.parse(savedConfig));
      } catch (error) {
        console.error('Error parsing saved config:', error);
      }
    }
  };

  return (
    <div className="min-h-screen flex flex-col bg-background">
      <Navbar />
      
      <main className="flex-1 w-full max-w-7xl mx-auto pt-24 px-6 pb-16">
        <div className="flex justify-between items-center mb-8">
          <div>
            <h1 className="text-3xl font-bold">Configuración de Email</h1>
            <p className="text-muted-foreground">
              Administra la configuración de entrega de emails para tus encuestas
            </p>
          </div>
          
          <div className="flex space-x-3">
            <Button 
              variant="outline" 
              onClick={() => navigate(-1)}
              className="flex items-center"
            >
              <ArrowLeft className="mr-2 h-4 w-4" />
              Volver
            </Button>
            <Button 
              onClick={handleSaveSettings}
              className="flex items-center"
              disabled={isUpdatingDeliveryConfig}
            >
              <Save className="mr-2 h-4 w-4" />
              {isUpdatingDeliveryConfig ? 'Guardando...' : 'Guardar configuración'}
            </Button>
          </div>
        </div>
        
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
          <Card className="lg:col-span-1">
            <CardHeader>
              <CardTitle className="flex items-center">
                <List className="mr-2 h-5 w-5" />
                Encuestas
              </CardTitle>
            </CardHeader>
            <CardContent>
              {isLoadingSurveys ? (
                <div className="flex justify-center py-8">
                  <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary"></div>
                </div>
              ) : surveys && surveys.length > 0 ? (
                <div className="max-h-[500px] overflow-auto">
                  <Table>
                    <TableHeader>
                      <TableRow>
                        <TableHead>Título</TableHead>
                        <TableHead>Fecha</TableHead>
                        <TableHead className="text-right">Acción</TableHead>
                      </TableRow>
                    </TableHeader>
                    <TableBody>
                      <TableRow>
                        <TableCell className="font-medium">Configuración global</TableCell>
                        <TableCell>-</TableCell>
                        <TableCell className="text-right">
                          <Button 
                            variant={selectedSurveyId === null ? "default" : "outline"} 
                            size="sm"
                            onClick={handleResetToGlobal}
                          >
                            Seleccionar
                          </Button>
                        </TableCell>
                      </TableRow>
                      {surveys.map((survey) => (
                        <TableRow key={survey.id} className={selectedSurveyId === survey.id ? "bg-accent/30" : ""}>
                          <TableCell className="font-medium">
                            <div className="flex flex-col">
                              <span>{survey.title}</span>
                              {survey.deliveryConfig && (
                                <Badge variant="outline" className="mt-1 w-fit">
                                  Configuración personalizada
                                </Badge>
                              )}
                            </div>
                          </TableCell>
                          <TableCell>{formatDate(survey.createdAt)}</TableCell>
                          <TableCell className="text-right">
                            <Button 
                              variant={selectedSurveyId === survey.id ? "default" : "outline"} 
                              size="sm"
                              onClick={() => handleSelectSurvey(survey.id)}
                            >
                              Seleccionar
                            </Button>
                          </TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                </div>
              ) : (
                <div className="text-center py-8">
                  <p className="text-muted-foreground">No hay encuestas disponibles</p>
                  <Button 
                    variant="outline" 
                    className="mt-4"
                    onClick={() => navigate('/create')}
                  >
                    Crear una encuesta
                  </Button>
                </div>
              )}
            </CardContent>
          </Card>
          
          <Card className="lg:col-span-2">
            <CardHeader>
              <CardTitle>
                {selectedSurveyId ? 'Configuración específica de encuesta' : 'Configuración global de email'}
              </CardTitle>
              {selectedSurveyId && (
                <p className="text-sm text-muted-foreground">
                  Configurando encuesta: {surveys?.find(s => s.id === selectedSurveyId)?.title}
                </p>
              )}
            </CardHeader>
            <CardContent>
              <p className="mb-4">
                {selectedSurveyId 
                  ? 'Esta configuración se aplicará solo a la encuesta seleccionada.'
                  : 'Esta configuración se aplicará como predeterminada para todas las nuevas encuestas. Puedes sobrescribir esta configuración para encuestas individuales.'}
              </p>
              
              <EmailDeliverySettings 
                deliveryConfig={deliveryConfig}
                onConfigChange={(config) => setDeliveryConfig(config)}
              />
            </CardContent>
          </Card>
        </div>
      </main>
      
      <Footer />
    </div>
  );
}
