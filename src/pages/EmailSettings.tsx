
import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { useToast } from "@/components/ui/use-toast";
import { ArrowLeft, Save } from 'lucide-react';
import Navbar from '@/components/layout/Navbar';
import Footer from '@/components/layout/Footer';
import EmailDeliverySettings from '@/components/survey/EmailDeliverySettings';
import { DeliveryConfig } from '@/types/surveyTypes';

export default function EmailSettings() {
  const navigate = useNavigate();
  const { toast } = useToast();
  const [deliveryConfig, setDeliveryConfig] = useState<DeliveryConfig>({
    type: 'manual',
    emailAddresses: [],
  });
  
  const handleSaveSettings = () => {
    // Aquí puedes implementar la lógica para guardar la configuración
    localStorage.setItem('emailDeliveryConfig', JSON.stringify(deliveryConfig));
    
    toast({
      title: "Configuración guardada",
      description: "La configuración de entrega de emails ha sido guardada exitosamente"
    });
  };
  
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
            >
              <Save className="mr-2 h-4 w-4" />
              Guardar configuración
            </Button>
          </div>
        </div>
        
        <Card className="mb-8">
          <CardHeader>
            <CardTitle>Configuración global de email</CardTitle>
          </CardHeader>
          <CardContent>
            <p className="mb-4">
              Esta configuración se aplicará como predeterminada para todas las nuevas encuestas.
              Puedes sobrescribir esta configuración para encuestas individuales durante su creación.
            </p>
            
            <EmailDeliverySettings 
              deliveryConfig={deliveryConfig}
              onConfigChange={(config) => setDeliveryConfig(config)}
            />
          </CardContent>
        </Card>
      </main>
      
      <Footer />
    </div>
  );
}
