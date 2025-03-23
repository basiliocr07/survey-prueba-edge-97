
import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import Navbar from '@/components/layout/Navbar';
import Footer from '@/components/layout/Footer';
import { useSurvey } from '@/application/hooks/useSurvey';
import { useToast } from "@/hooks/use-toast";
import { v4 as uuidv4 } from 'uuid';

export default function SurveyExample() {
  const navigate = useNavigate();
  const { toast } = useToast();
  const { createSurvey, isCreating } = useSurvey();
  
  const [surveyTitle, setSurveyTitle] = useState<string>('');
  const [surveyDescription, setSurveyDescription] = useState<string>('');
  
  const handleCreateSurvey = async () => {
    if (!surveyTitle.trim()) {
      toast({
        title: "Título requerido",
        description: "Por favor proporciona un título para tu encuesta",
        variant: "destructive"
      });
      return;
    }
    
    // Create a basic survey with one question
    const newSurvey = {
      title: surveyTitle,
      description: surveyDescription,
      questions: [
        {
          id: uuidv4(),
          title: '¿Cómo calificarías nuestro servicio?',
          type: 'rating',
          required: true,
          options: []
        }
      ]
    };
    
    try {
      const createdSurvey = await createSurvey(newSurvey);
      toast({
        title: "Encuesta creada",
        description: "Tu encuesta ha sido creada exitosamente"
      });
      
      // Navigate to the created survey
      if (createdSurvey && createdSurvey.id) {
        navigate(`/survey/${createdSurvey.id}`);
      }
    } catch (error) {
      toast({
        title: "Error",
        description: `No se pudo crear la encuesta: ${error instanceof Error ? error.message : 'Error desconocido'}`,
        variant: "destructive"
      });
    }
  };
  
  return (
    <div className="min-h-screen flex flex-col bg-background">
      <Navbar />
      
      <main className="flex-1 w-full max-w-7xl mx-auto pt-24 px-6 pb-16">
        <div className="mb-8">
          <h1 className="text-3xl font-bold mb-2">Arquitectura Hexagonal - Ejemplo</h1>
          <p className="text-muted-foreground">
            Este es un ejemplo simplificado usando la arquitectura hexagonal para crear encuestas
          </p>
        </div>
        
        <Card>
          <CardHeader>
            <CardTitle>Crear Nueva Encuesta</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <div>
              <label htmlFor="survey-title" className="block text-sm font-medium mb-1">
                Título de la Encuesta
              </label>
              <Input
                id="survey-title"
                value={surveyTitle}
                onChange={(e) => setSurveyTitle(e.target.value)}
                placeholder="Ingresa el título de la encuesta"
                className="w-full"
              />
            </div>
            <div>
              <label htmlFor="survey-description" className="block text-sm font-medium mb-1">
                Descripción (opcional)
              </label>
              <Textarea
                id="survey-description"
                value={surveyDescription}
                onChange={(e) => setSurveyDescription(e.target.value)}
                placeholder="Ingresa una descripción para tu encuesta"
                className="min-h-[100px]"
              />
            </div>
            
            <Button 
              onClick={handleCreateSurvey}
              disabled={isCreating}
            >
              {isCreating ? 'Creando...' : 'Crear Encuesta Básica'}
            </Button>
          </CardContent>
        </Card>
      </main>
      
      <Footer />
    </div>
  );
}
