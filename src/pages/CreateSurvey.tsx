
import { useState, useEffect } from 'react';
import { v4 as uuidv4 } from 'uuid';
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import QuestionBuilder from "@/components/survey/QuestionBuilder";
import { Plus, Send, Share2, Save, Copy, Mail } from "lucide-react";
import { useToast } from "@/hooks/use-toast";
import Navbar from '@/components/layout/Navbar';
import Footer from '@/components/layout/Footer';
import EmailDeliverySettings from '@/components/survey/EmailDeliverySettings';
import { 
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle
} from "@/components/ui/dialog";
import { useNavigate, useParams } from 'react-router-dom';
import StarRating from '@/components/survey/StarRating';
import NPSRating from '@/components/survey/NPSRating';
import { useSurvey } from "@/application/hooks/useSurvey";
import { DeliveryConfig, SurveyQuestion } from "@/domain/models/Survey";
import { Loader2 } from "lucide-react";

export default function CreateSurvey() {
  const { toast } = useToast();
  const navigate = useNavigate();
  const { id: urlSurveyId } = useParams<{ id: string }>();
  
  const { 
    survey, 
    isLoading, 
    createSurvey, 
    isCreating, 
    updateSurvey, 
    isUpdating,
    sendSurveyEmails,
    isSendingEmails
  } = useSurvey(urlSurveyId);
  
  const [surveyTitle, setSurveyTitle] = useState<string>('');
  const [surveyDescription, setSurveyDescription] = useState<string>('');
  const [questions, setQuestions] = useState<SurveyQuestion[]>([
    {
      id: uuidv4(),
      type: 'single-choice',
      title: '',
      required: true,
      options: ['Option 1', 'Option 2', 'Option 3']
    }
  ]);
  const [activeTab, setActiveTab] = useState<string>('design');
  const [surveyId, setSurveyId] = useState<string>(urlSurveyId || '');
  const [dialogOpen, setDialogOpen] = useState<boolean>(false);
  const [deliveryConfig, setDeliveryConfig] = useState<DeliveryConfig>({
    type: 'manual',
    emailAddresses: [],
    schedule: {
      frequency: 'monthly',
      dayOfMonth: 1,
      time: '09:00'
    },
    trigger: {
      type: 'ticket-closed',
      delayHours: 24,
      sendAutomatically: false
    }
  });

  // Load survey data if editing
  useEffect(() => {
    if (survey) {
      setSurveyTitle(survey.title);
      setSurveyDescription(survey.description || '');
      setQuestions(survey.questions);
      if (survey.deliveryConfig) {
        setDeliveryConfig(survey.deliveryConfig);
      }
    }
  }, [survey]);

  const addQuestion = () => {
    const newQuestion: SurveyQuestion = {
      id: uuidv4(),
      type: 'single-choice',
      title: '',
      required: true,
      options: ['Option 1', 'Option 2', 'Option 3']
    };
    
    setQuestions([...questions, newQuestion]);
  };

  const updateQuestion = (index: number, updatedQuestion: SurveyQuestion) => {
    const newQuestions = [...questions];
    newQuestions[index] = updatedQuestion;
    setQuestions(newQuestions);
  };

  const deleteQuestion = (index: number) => {
    if (questions.length <= 1) {
      toast({
        title: "Cannot delete",
        description: "Your survey must have at least one question",
        variant: "destructive"
      });
      return;
    }
    
    const newQuestions = questions.filter((_, i) => i !== index);
    setQuestions(newQuestions);
  };

  const moveQuestionUp = (index: number) => {
    if (index === 0) return;
    const newQuestions = [...questions];
    [newQuestions[index - 1], newQuestions[index]] = [newQuestions[index], newQuestions[index - 1]];
    setQuestions(newQuestions);
  };

  const moveQuestionDown = (index: number) => {
    if (index === questions.length - 1) return;
    const newQuestions = [...questions];
    [newQuestions[index], newQuestions[index + 1]] = [newQuestions[index + 1], newQuestions[index]];
    setQuestions(newQuestions);
  };

  const saveSurvey = async () => {
    if (!surveyTitle.trim()) {
      toast({
        title: "Título requerido",
        description: "Por favor proporciona un título para tu encuesta",
        variant: "destructive"
      });
      return;
    }
    
    const hasEmptyQuestionTitles = questions.some(q => !q.title.trim());
    if (hasEmptyQuestionTitles) {
      toast({
        title: "Preguntas incompletas",
        description: "Por favor proporciona un título para todas las preguntas",
        variant: "destructive"
      });
      return;
    }
    
    const surveyData = {
      title: surveyTitle,
      description: surveyDescription,
      questions: questions,
      deliveryConfig: deliveryConfig
    };
    
    try {
      if (surveyId) {
        // Update existing survey
        await updateSurvey({
          ...surveyData,
          id: surveyId,
          createdAt: survey?.createdAt || new Date().toISOString()
        });
        toast({
          title: "Encuesta actualizada",
          description: "Tu encuesta ha sido actualizada exitosamente"
        });
      } else {
        // Create new survey
        const result = await createSurvey(surveyData);
        if (result && result.id) {
          setSurveyId(result.id);
          setDialogOpen(true);
          toast({
            title: "Encuesta guardada",
            description: "Tu encuesta ha sido guardada exitosamente"
          });
        }
      }
    } catch (error) {
      toast({
        title: "Error",
        description: `No se pudo guardar la encuesta: ${error instanceof Error ? error.message : 'Error desconocido'}`,
        variant: "destructive"
      });
    }
  };

  const shareSurvey = () => {
    setDialogOpen(true);
  };

  const previewSurvey = () => {
    setActiveTab('preview');
  };
  
  const copySurveyLink = () => {
    const surveyLink = `${window.location.origin}/survey/${surveyId}`;
    navigator.clipboard.writeText(surveyLink);
    
    toast({
      title: "Link copied!",
      description: "Survey link has been copied to clipboard"
    });
  };
  
  const navigateToSurvey = () => {
    navigate(`/survey/${surveyId}`);
    setDialogOpen(false);
  };

  const sendSurveyEmailsHandler = async () => {
    if (!surveyId) {
      toast({
        title: "Save first",
        description: "Please save your survey before sending emails",
        variant: "destructive"
      });
      return;
    }
    
    if (deliveryConfig.emailAddresses.length === 0) {
      toast({
        title: "No recipients",
        description: "Please add at least one email address",
        variant: "destructive"
      });
      return;
    }

    try {
      const success = await sendSurveyEmails(surveyId, deliveryConfig.emailAddresses);
      
      if (success) {
        toast({
          title: "Emails sent",
          description: `Survey has been sent to ${deliveryConfig.emailAddresses.length} recipient(s)`,
        });
      } else {
        toast({
          title: "Error sending emails",
          description: "There was a problem sending your emails. Please try again.",
          variant: "destructive"
        });
      }
    } catch (error) {
      toast({
        title: "Error",
        description: `Failed to send emails: ${error instanceof Error ? error.message : 'Unknown error'}`,
        variant: "destructive"
      });
    }
  };

  if (isLoading && urlSurveyId) {
    return (
      <div className="min-h-screen flex flex-col items-center justify-center bg-background">
        <Loader2 className="h-8 w-8 animate-spin text-primary" />
        <p className="mt-4 text-lg">Cargando encuesta...</p>
      </div>
    );
  }

  return (
    <div className="min-h-screen flex flex-col bg-background">
      <Navbar />
      
      <main className="flex-1 w-full max-w-7xl mx-auto pt-24 px-6 pb-16">
        <div className="mb-8">
          <h1 className="text-3xl font-bold mb-2">{surveyId ? 'Editar Encuesta' : 'Crear una Nueva Encuesta'}</h1>
          <p className="text-muted-foreground">
            Diseña tu encuesta, añade preguntas y personaliza la configuración
          </p>
        </div>
        
        <Tabs value={activeTab} onValueChange={setActiveTab} className="w-full">
          <TabsList className="grid w-full grid-cols-4 mb-8">
            <TabsTrigger value="design">Diseñar Encuesta</TabsTrigger>
            <TabsTrigger value="delivery">Envío por Email</TabsTrigger>
            <TabsTrigger value="settings">Configuración</TabsTrigger>
            <TabsTrigger value="preview">Vista Previa</TabsTrigger>
          </TabsList>
          
          <TabsContent value="design" className="animate-fade-in">
            <div className="grid gap-8">
              <Card>
                <CardHeader>
                  <CardTitle>Detalles de la Encuesta</CardTitle>
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
                </CardContent>
              </Card>
              
              <div>
                <div className="flex items-center justify-between mb-4">
                  <h2 className="text-xl font-semibold">Preguntas</h2>
                  <Button onClick={addQuestion}>
                    <Plus className="mr-2 h-4 w-4" /> Añadir Pregunta
                  </Button>
                </div>
                
                <div className="space-y-4">
                  {questions.map((question, index) => (
                    <QuestionBuilder
                      key={question.id}
                      question={question}
                      onChange={(updatedQuestion) => updateQuestion(index, updatedQuestion)}
                      onDelete={() => deleteQuestion(index)}
                      onMoveUp={() => moveQuestionUp(index)}
                      onMoveDown={() => moveQuestionDown(index)}
                      isFirst={index === 0}
                      isLast={index === questions.length - 1}
                    />
                  ))}
                </div>
              </div>
              
              <div className="flex justify-end space-x-4 mt-8">
                <Button variant="outline" onClick={previewSurvey}>
                  Vista Previa
                </Button>
                <Button 
                  onClick={saveSurvey}
                  disabled={isCreating || isUpdating}
                >
                  <Save className="mr-2 h-4 w-4" /> 
                  {isCreating || isUpdating ? 'Guardando...' : (surveyId ? 'Actualizar Encuesta' : 'Guardar Encuesta')}
                </Button>
                {surveyId && (
                  <Button variant="secondary" onClick={shareSurvey}>
                    <Share2 className="mr-2 h-4 w-4" /> Compartir Encuesta
                  </Button>
                )}
              </div>
            </div>
          </TabsContent>
          
          <TabsContent value="delivery" className="animate-fade-in">
            <div className="grid gap-8">
              <EmailDeliverySettings 
                deliveryConfig={deliveryConfig}
                onConfigChange={setDeliveryConfig}
              />
              
              <div className="flex justify-end mt-4">
                <Button 
                  onClick={sendSurveyEmailsHandler} 
                  disabled={isSendingEmails || deliveryConfig.emailAddresses.length === 0 || !surveyId}
                >
                  {isSendingEmails ? (
                    <>
                      <Loader2 className="mr-2 h-4 w-4 animate-spin" /> Enviando...
                    </>
                  ) : (
                    <>
                      <Mail className="mr-2 h-4 w-4" /> Enviar Ahora
                    </>
                  )}
                </Button>
              </div>
            </div>
          </TabsContent>
          
          <TabsContent value="settings" className="animate-fade-in">
            <Card>
              <CardHeader>
                <CardTitle>Configuración de la Encuesta</CardTitle>
              </CardHeader>
              <CardContent className="space-y-6">
                <div>
                  <h3 className="text-lg font-medium mb-4">Recolección de Datos</h3>
                  <div className="space-y-4">
                    <div className="flex items-center justify-between">
                      <div>
                        <h4 className="font-medium">Permitir respuestas anónimas</h4>
                        <p className="text-sm text-muted-foreground">Los encuestados pueden enviar sin identificarse</p>
                      </div>
                      <div className="flex items-center h-6">
                        <input type="checkbox" id="anonymous" className="mr-2" defaultChecked />
                        <label htmlFor="anonymous">Habilitar</label>
                      </div>
                    </div>
                    
                    <div className="flex items-center justify-between">
                      <div>
                        <h4 className="font-medium">Múltiples envíos</h4>
                        <p className="text-sm text-muted-foreground">Permitir que los encuestados envíen varias veces</p>
                      </div>
                      <div className="flex items-center h-6">
                        <input type="checkbox" id="multiple" className="mr-2" />
                        <label htmlFor="multiple">Habilitar</label>
                      </div>
                    </div>
                  </div>
                </div>
                
                <div>
                  <h3 className="text-lg font-medium mb-4">Opciones de Visualización</h3>
                  <div className="space-y-4">
                    <div className="flex items-center justify-between">
                      <div>
                        <h4 className="font-medium">Mostrar barra de progreso</h4>
                        <p className="text-sm text-muted-foreground">Mostrar el progreso de finalización a los encuestados</p>
                      </div>
                      <div className="flex items-center h-6">
                        <input type="checkbox" id="progress" className="mr-2" defaultChecked />
                        <label htmlFor="progress">Habilitar</label>
                      </div>
                    </div>
                    
                    <div className="flex items-center justify-between">
                      <div>
                        <h4 className="font-medium">Mostrar números de pregunta</h4>
                        <p className="text-sm text-muted-foreground">Mostrar los números de pregunta a los encuestados</p>
                      </div>
                      <div className="flex items-center h-6">
                        <input type="checkbox" id="numbers" className="mr-2" defaultChecked />
                        <label htmlFor="numbers">Habilitar</label>
                      </div>
                    </div>
                  </div>
                </div>
              </CardContent>
            </Card>
          </TabsContent>
          
          <TabsContent value="preview" className="animate-fade-in">
            <Card>
              <CardHeader>
                <CardTitle>Vista Previa de tu Encuesta</CardTitle>
              </CardHeader>
              <CardContent>
                <div className="max-w-3xl mx-auto border rounded-lg p-6 bg-white shadow-sm">
                  <div className="mb-8">
                    <h2 className="text-2xl font-bold mb-2">{surveyTitle || "Encuesta sin título"}</h2>
                    {surveyDescription && <p className="text-muted-foreground">{surveyDescription}</p>}
                  </div>
                  
                  <div className="space-y-8">
                    {questions.map((question, index) => (
                      <div key={question.id} className="border-b pb-6 last:border-0">
                        <div className="mb-3">
                          <h3 className="text-lg font-medium">
                            {index + 1}. {question.title || "Pregunta sin título"}
                            {question.required && <span className="text-red-500 ml-1">*</span>}
                          </h3>
                          {question.description && (
                            <p className="text-sm text-muted-foreground mt-1">{question.description}</p>
                          )}
                        </div>
                        
                        <div className="mt-3">
                          {question.type === 'single-choice' && question.options && (
                            <div className="space-y-2">
                              {question.options.map((option, i) => (
                                <div key={i} className="flex items-center">
                                  <input 
                                    type="radio" 
                                    id={`q${index}-o${i}`} 
                                    name={`question-${question.id}`} 
                                    className="mr-2"
                                  />
                                  <label htmlFor={`q${index}-o${i}`}>{option}</label>
                                </div>
                              ))}
                            </div>
                          )}
                          
                          {question.type === 'multiple-choice' && question.options && (
                            <div className="space-y-2">
                              {question.options.map((option, i) => (
                                <div key={i} className="flex items-center">
                                  <input 
                                    type="checkbox" 
                                    id={`q${index}-o${i}`} 
                                    name={`question-${question.id}`} 
                                    className="mr-2"
                                  />
                                  <label htmlFor={`q${index}-o${i}`}>{option}</label>
                                </div>
                              ))}
                            </div>
                          )}
                          
                          {question.type === 'text' && (
                            <textarea 
                              className="w-full rounded-md border border-input bg-background px-3 py-2 min-h-[100px]" 
                              placeholder="Tu respuesta"
                            />
                          )}
                          
                          {question.type === 'rating' && (
                            <StarRating 
                              name={`preview-${question.id}`} 
                              value="3" 
                              onChange={() => {}}
                              required={false}
                            />
                          )}
                          
                          {question.type === 'nps' && (
                            <NPSRating 
                              name={`preview-${question.id}`} 
                              value="7" 
                              onChange={() => {}}
                              required={false}
                            />
                          )}
                        </div>
                      </div>
                    ))}
                  </div>
                  
                  <div className="mt-8 flex justify-end">
                    <Button>Enviar Respuestas</Button>
                  </div>
                </div>
              </CardContent>
            </Card>
            
            <div className="flex justify-end mt-6">
              <Button variant="outline" onClick={() => setActiveTab('design')}>
                Volver a Editar
              </Button>
            </div>
          </TabsContent>
        </Tabs>
      </main>
      
      <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
        <DialogContent className="sm:max-w-md">
          <DialogHeader>
            <DialogTitle>Compartir Encuesta</DialogTitle>
            <DialogDescription>
              Tu encuesta está lista para ser compartida con los encuestados. Usa el enlace a continuación.
            </DialogDescription>
          </DialogHeader>
          <div className="flex items-center space-x-2 mt-4">
            <div className="bg-muted p-2 rounded-md flex-1 overflow-hidden">
              <p className="text-sm font-mono truncate">
                {window.location.origin}/survey/{surveyId}
              </p>
            </div>
            <Button variant="outline" size="sm" onClick={copySurveyLink}>
              <Copy className="h-4 w-4" />
            </Button>
          </div>
          <DialogFooter className="sm:justify-start mt-4">
            <Button variant="default" onClick={navigateToSurvey}>
              Abrir Encuesta
            </Button>
            <Button variant="outline" onClick={() => setDialogOpen(false)}>
              Cerrar
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>
      
      <Footer />
    </div>
  );
}
