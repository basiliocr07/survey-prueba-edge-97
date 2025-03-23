import { useState, useEffect, useCallback } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import { v4 as uuidv4 } from 'uuid';
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Separator } from "@/components/ui/separator";
import { useToast } from "@/hooks/use-toast";
import Navbar from '@/components/layout/Navbar';
import Footer from '@/components/layout/Footer';
import QuestionBuilder from '@/components/survey/QuestionBuilder';
import EmailDeliverySettings from '@/components/survey/EmailDeliverySettings';
import { useSurvey } from '@/application/hooks/useSurvey';
import { Survey, SurveyQuestion, DeliveryConfig, QuestionType } from '@/types/surveyTypes';
import { FilePlus, Save, Send, Settings } from 'lucide-react';

type Question = {
  id: string;
  title: string;
  description?: string;
  type: QuestionType;
  required: boolean;
  options?: string[];
  settings?: {
    min?: number;
    max?: number;
  };
};

export default function CreateSurvey() {
  const navigate = useNavigate();
  const location = useLocation();
  const { toast } = useToast();
  
  const query = new URLSearchParams(location.search);
  const editSurveyId = query.get('edit');
  
  const { survey, isLoading, createSurvey, updateSurvey, isCreating, isUpdating, sendSurveyEmails, isSendingEmails } = useSurvey(editSurveyId || undefined);
  
  const [title, setTitle] = useState<string>('');
  const [description, setDescription] = useState<string>('');
  const [questions, setQuestions] = useState<Question[]>([]);
  const [activeTab, setActiveTab] = useState<string>('edit');
  const [deliveryConfig, setDeliveryConfig] = useState<DeliveryConfig>({
    type: 'manual',
    emailAddresses: [],
  });
  
  useEffect(() => {
    if (survey) {
      setTitle(survey.title);
      setDescription(survey.description || '');
      setQuestions(survey.questions.map(q => ({
        ...q,
        type: q.type as QuestionType
      })));
      
      if (survey.deliveryConfig) {
        setDeliveryConfig(survey.deliveryConfig);
      }
    }
  }, [survey]);
  
  const addQuestion = useCallback(() => {
    const newQuestion: Question = {
      id: uuidv4(),
      title: 'New Question',
      type: 'text',
      required: true,
      options: [],
    };
    
    setQuestions(prev => [...prev, newQuestion]);
  }, []);
  
  const addSampleQuestions = useCallback(() => {
    const sampleQuestions: Question[] = [
      {
        id: uuidv4(),
        title: 'How satisfied are you with our service?',
        type: 'rating',
        required: true,
        options: [],
      },
      {
        id: uuidv4(),
        title: 'What features do you like most?',
        type: 'multiple-choice',
        required: true,
        options: ['User Interface', 'Performance', 'Customer Support', 'Price'],
      },
      {
        id: uuidv4(),
        title: 'Please provide any additional feedback',
        type: 'text',
        required: false,
        options: [],
      }
    ];
    
    setQuestions(prev => [...prev, ...sampleQuestions]);
  }, []);
  
  const updateQuestion = useCallback((questionId: string, updatedQuestion: Question) => {
    setQuestions(prev => 
      prev.map(q => q.id === questionId ? updatedQuestion : q)
    );
  }, []);
  
  const removeQuestion = useCallback((questionId: string) => {
    setQuestions(prev => prev.filter(q => q.id !== questionId));
  }, []);
  
  const moveQuestion = useCallback((questionId: string, direction: 'up' | 'down') => {
    setQuestions(prev => {
      const currentIndex = prev.findIndex(q => q.id === questionId);
      if (currentIndex === -1) return prev;
      
      if (direction === 'up' && currentIndex === 0) return prev;
      if (direction === 'down' && currentIndex === prev.length - 1) return prev;
      
      const newQuestions = [...prev];
      const targetIndex = direction === 'up' ? currentIndex - 1 : currentIndex + 1;
      
      [newQuestions[currentIndex], newQuestions[targetIndex]] = 
        [newQuestions[targetIndex], newQuestions[currentIndex]];
      
      return newQuestions;
    });
  }, []);
  
  const handleSubmit = async () => {
    if (!title.trim()) {
      toast({
        title: "Título requerido",
        description: "Por favor proporciona un título para tu encuesta",
        variant: "destructive"
      });
      return;
    }
    
    if (questions.length === 0) {
      toast({
        title: "Preguntas requeridas",
        description: "Tu encuesta debe tener al menos una pregunta",
        variant: "destructive"
      });
      return;
    }
    
    try {
      const surveyQuestions: SurveyQuestion[] = questions.map(q => ({
        id: q.id,
        title: q.title,
        description: q.description,
        type: q.type,
        required: q.required,
        options: q.options,
        settings: q.settings
      }));
      
      const surveyData = {
        title,
        description: description || undefined,
        questions: surveyQuestions,
        deliveryConfig
      };
      
      let result;
      
      if (editSurveyId) {
        await updateSurvey({
          ...surveyData,
          id: editSurveyId,
          createdAt: survey?.createdAt || new Date().toISOString()
        });
        result = { id: editSurveyId };
      } else {
        result = await createSurvey(surveyData);
      }
      
      if (
        deliveryConfig.type === 'manual' && 
        deliveryConfig.emailAddresses.length > 0 &&
        result && 'id' in result
      ) {
        const shouldSendEmails = window.confirm(
          `¿Quieres enviar la encuesta a ${deliveryConfig.emailAddresses.length} direcciones de correo ahora?`
        );
        
        if (shouldSendEmails) {
          await sendSurveyEmails(result.id, deliveryConfig.emailAddresses);
        }
      }
      
      toast({
        title: editSurveyId ? "Encuesta actualizada" : "Encuesta creada",
        description: editSurveyId 
          ? "La encuesta ha sido actualizada exitosamente" 
          : "La encuesta ha sido creada exitosamente"
      });
      
      navigate("/surveys");
    } catch (error) {
      toast({
        title: "Error",
        description: `No se pudo ${editSurveyId ? 'actualizar' : 'crear'} la encuesta: ${error instanceof Error ? error.message : 'Error desconocido'}`,
        variant: "destructive"
      });
    }
  };
  
  const isFormLoading = isLoading || isCreating || isUpdating || isSendingEmails;
  
  if (isLoading && editSurveyId) {
    return (
      <div className="min-h-screen flex flex-col bg-background">
        <Navbar />
        <main className="flex-1 w-full max-w-7xl mx-auto pt-24 px-6 pb-16 flex items-center justify-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary"></div>
        </main>
      </div>
    );
  }
  
  return (
    <div className="min-h-screen flex flex-col bg-background">
      <Navbar />
      
      <main className="flex-1 w-full max-w-7xl mx-auto pt-24 px-6 pb-16">
        <div className="flex justify-between items-center mb-8">
          <div>
            <h1 className="text-3xl font-bold">{editSurveyId ? 'Edit Survey' : 'Create Survey'}</h1>
            <p className="text-muted-foreground">
              {editSurveyId 
                ? 'Update your existing survey' 
                : 'Design a new survey for your users'}
            </p>
          </div>
          
          <div className="flex space-x-3">
            <Button 
              variant="outline" 
              onClick={() => navigate("/surveys")}
            >
              Cancel
            </Button>
            <Button 
              onClick={handleSubmit}
              disabled={isFormLoading}
            >
              {isFormLoading ? (
                <>
                  <span className="animate-spin mr-2">⏳</span>
                  {editSurveyId ? 'Updating...' : 'Creating...'}
                </>
              ) : (
                <>
                  <Save className="mr-2 h-4 w-4" />
                  {editSurveyId ? 'Update Survey' : 'Create Survey'}
                </>
              )}
            </Button>
          </div>
        </div>
        
        <Tabs 
          defaultValue="edit" 
          value={activeTab} 
          onValueChange={setActiveTab}
          className="space-y-4"
        >
          <TabsList className="grid w-full grid-cols-2">
            <TabsTrigger value="edit" disabled={isFormLoading}>
              <FilePlus className="h-4 w-4 mr-2" />
              Survey Content
            </TabsTrigger>
            <TabsTrigger value="delivery" disabled={isFormLoading}>
              <Send className="h-4 w-4 mr-2" />
              Delivery Settings
            </TabsTrigger>
          </TabsList>
          
          <TabsContent value="edit" className="space-y-4">
            <Card>
              <CardHeader>
                <CardTitle>Survey Details</CardTitle>
              </CardHeader>
              <CardContent className="space-y-4">
                <div>
                  <label htmlFor="title" className="block text-sm font-medium mb-1">
                    Title <span className="text-destructive">*</span>
                  </label>
                  <Input
                    id="title"
                    value={title}
                    onChange={(e) => setTitle(e.target.value)}
                    placeholder="Enter survey title"
                    className="w-full"
                  />
                </div>
                <div>
                  <label htmlFor="description" className="block text-sm font-medium mb-1">
                    Description (optional)
                  </label>
                  <Textarea
                    id="description"
                    value={description}
                    onChange={(e) => setDescription(e.target.value)}
                    placeholder="Enter a description for your survey"
                    className="min-h-[100px]"
                  />
                </div>
              </CardContent>
            </Card>
            
            <div className="flex justify-between items-center">
              <h2 className="text-xl font-semibold">Questions</h2>
              <div className="flex space-x-2">
                <Button 
                  variant="outline" 
                  onClick={addSampleQuestions}
                  disabled={isFormLoading}
                >
                  Add Sample Questions
                </Button>
                <Button 
                  onClick={addQuestion}
                  disabled={isFormLoading}
                >
                  Add Question
                </Button>
              </div>
            </div>
            
            {questions.length === 0 ? (
              <Card>
                <CardContent className="flex flex-col items-center justify-center p-8 text-center">
                  <div className="rounded-full bg-primary/10 p-4 mb-4">
                    <FilePlus className="h-6 w-6 text-primary" />
                  </div>
                  <h3 className="text-lg font-semibold mb-1">No questions yet</h3>
                  <p className="text-muted-foreground mb-4">Add questions to your survey</p>
                  <Button onClick={addQuestion} disabled={isFormLoading}>
                    Add First Question
                  </Button>
                </CardContent>
              </Card>
            ) : (
              <div className="space-y-4">
                {questions.map((question, index) => (
                  <QuestionBuilder
                    key={question.id}
                    question={question}
                    index={index}
                    total={questions.length}
                    onUpdate={(updatedQuestion) => updateQuestion(question.id, updatedQuestion as Question)}
                    onDelete={() => removeQuestion(question.id)}
                    onMoveUp={() => moveQuestion(question.id, 'up')}
                    onMoveDown={() => moveQuestion(question.id, 'down')}
                  />
                ))}
              </div>
            )}
          </TabsContent>
          
          <TabsContent value="delivery">
            <EmailDeliverySettings 
              deliveryConfig={deliveryConfig} 
              onConfigChange={(config) => setDeliveryConfig(config)}
            />
            
            <div className="mt-6 flex justify-end">
              <Button 
                onClick={() => setActiveTab('edit')}
                variant="outline"
                className="mr-2"
              >
                Back to Questions
              </Button>
              <Button 
                onClick={handleSubmit}
                disabled={isFormLoading}
              >
                {isFormLoading ? (
                  <>
                    <span className="animate-spin mr-2">⏳</span>
                    {editSurveyId ? 'Updating...' : 'Creating...'}
                  </>
                ) : (
                  <>
                    <Save className="mr-2 h-4 w-4" />
                    {editSurveyId ? 'Update Survey' : 'Create Survey'}
                  </>
                )}
              </Button>
            </div>
          </TabsContent>
        </Tabs>
      </main>
      
      <Footer />
    </div>
  );
}
