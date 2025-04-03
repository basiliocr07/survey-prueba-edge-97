
import React from "react";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Collapsible, CollapsibleContent, CollapsibleTrigger } from "@/components/ui/collapsible";
import { ChevronDown, ChevronUp, MessageSquare } from "lucide-react";
import { useSurveyResponse } from "@/application/hooks/useSurveyResponse";
import Navbar from "@/components/layout/Navbar";
import Footer from "@/components/layout/Footer";

const Dashboard = () => {
  const [isOpen, setIsOpen] = React.useState(false);
  const { responses, isLoading } = useSurveyResponse();
  
  const latestResponses = responses?.slice(0, 5) || [];
  
  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    return new Intl.DateTimeFormat('es-ES', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    }).format(date);
  };

  return (
    <div className="flex flex-col min-h-screen">
      <Navbar />
      
      <div className="container py-24 flex-grow">
        <div className="mb-6">
          <Card className="shadow-md">
            <Collapsible open={isOpen} onOpenChange={setIsOpen}>
              <CardHeader className="py-3">
                <div className="flex justify-between items-center">
                  <CardTitle className="text-lg flex items-center">
                    <MessageSquare className="h-5 w-5 mr-2" />
                    Últimas Respuestas
                  </CardTitle>
                  <CollapsibleTrigger asChild>
                    <Button variant="ghost" size="sm" className="h-8 w-8 p-0">
                      {isOpen ? <ChevronUp className="h-4 w-4" /> : <ChevronDown className="h-4 w-4" />}
                    </Button>
                  </CollapsibleTrigger>
                </div>
                <CardDescription>
                  {latestResponses.length > 0 
                    ? `Mostrando las últimas ${latestResponses.length} respuestas recibidas` 
                    : "No hay respuestas recientes"}
                </CardDescription>
              </CardHeader>
              
              <CollapsibleContent>
                <CardContent>
                  {isLoading ? (
                    <div className="text-center py-4">
                      <p className="text-sm text-muted-foreground">Cargando respuestas...</p>
                    </div>
                  ) : latestResponses.length === 0 ? (
                    <div className="text-center py-4">
                      <p className="text-sm text-muted-foreground">No hay respuestas recientes</p>
                    </div>
                  ) : (
                    <div className="space-y-3">
                      {latestResponses.map((response) => (
                        <div key={response.id} className="border rounded-md p-3">
                          <div className="flex justify-between items-start">
                            <div>
                              <div className="font-medium">{response.respondentName}</div>
                              <div className="text-sm text-muted-foreground">{response.respondentEmail}</div>
                              {response.respondentCompany && (
                                <div className="text-xs text-muted-foreground mt-1">
                                  {response.respondentCompany}
                                </div>
                              )}
                            </div>
                            <div className="text-xs text-muted-foreground">
                              {formatDate(response.submittedAt)}
                            </div>
                          </div>
                          <div className="mt-2">
                            <div className="text-xs text-muted-foreground">
                              {response.answers.length} respuestas enviadas
                            </div>
                          </div>
                        </div>
                      ))}
                    </div>
                  )}
                </CardContent>
              </CollapsibleContent>
            </Collapsible>
          </Card>
        </div>
      </div>
      
      <Footer />
    </div>
  );
};

export default Dashboard;
