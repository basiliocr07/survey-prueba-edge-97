
import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { Card, CardContent, CardHeader, CardTitle, CardFooter } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { Form, FormControl, FormField, FormItem, FormLabel, FormMessage } from "@/components/ui/form";
import { useToast } from "@/hooks/use-toast";
import Navbar from '@/components/layout/Navbar';
import Footer from '@/components/layout/Footer';
import { AlertCircle, CheckCircle2 } from 'lucide-react';
import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { Survey, SurveyResponseSubmission } from '@/types/surveyTypes';
import StarRating from '@/components/survey/StarRating';
import NPSRating from '@/components/survey/NPSRating';
import { supabase } from "@/integrations/supabase/client";
import { useQuery, useMutation } from "@tanstack/react-query";
import { Json } from '@/integrations/supabase/types';

const formSchema = z.object({
  answers: z.record(z.union([
    z.string(),
    z.array(z.string())
  ]))
});

type FormValues = z.infer<typeof formSchema>;

export default function TakeSurvey() {
  const { surveyId } = useParams<{ surveyId: string }>();
  const navigate = useNavigate();
  const { toast } = useToast();
  
  const [submitted, setSubmitted] = useState(false);
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [userData, setUserData] = useState({
    username: '',
    email: '',
    phone: '',
    company: ''
  });
  
  const form = useForm<FormValues>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      answers: {}
    }
  });
  
  useEffect(() => {
    const loggedIn = localStorage.getItem('isLoggedIn') === 'true';
    setIsLoggedIn(loggedIn);
    
    if (loggedIn) {
      const username = localStorage.getItem('username') || '';
      const email = localStorage.getItem('userEmail') || '';
      const phone = localStorage.getItem('userPhone') || '';
      const company = localStorage.getItem('userCompany') || '';
      
      setUserData({
        username,
        email,
        phone,
        company
      });
    }
  }, []);
  
  // Fetch survey from Supabase
  const { data: survey, isLoading, error } = useQuery({
    queryKey: ['survey', surveyId],
    queryFn: async () => {
      if (!surveyId) throw new Error('Survey ID is required');
      
      const { data, error } = await supabase
        .from('surveys')
        .select('*')
        .eq('id', surveyId)
        .single();
        
      if (error) throw error;
      if (!data) throw new Error('Survey not found');
      
      // Transform Supabase data to match Survey interface
      const transformedData: Survey = {
        id: data.id,
        title: data.title,
        description: data.description || undefined,
        questions: Array.isArray(data.questions) 
          ? data.questions 
          : typeof data.questions === 'object' 
            ? Object.values(data.questions) 
            : [],
        createdAt: data.created_at
      };
      
      return transformedData;
    }
  });
  
  // Initialize form when survey data is loaded
  useEffect(() => {
    if (survey) {
      // Initialize form with empty answers
      const initialAnswers: Record<string, any> = {};
      survey.questions.forEach((question: any) => {
        if (question.type === 'multiple-choice') {
          initialAnswers[question.id] = [];
        } else {
          initialAnswers[question.id] = '';
        }
      });
      
      form.reset({ answers: initialAnswers });
    }
  }, [survey, form]);
  
  // Submit response mutation
  const submitResponseMutation = useMutation({
    mutationFn: async (responseData: SurveyResponseSubmission) => {
      const { data, error } = await supabase
        .from('survey_responses')
        .insert([
          {
            survey_id: responseData.surveyId,
            respondent_name: responseData.respondentName,
            respondent_email: responseData.respondentEmail,
            respondent_phone: responseData.respondentPhone,
            respondent_company: responseData.respondentCompany,
            answers: responseData.answers,
            submitted_at: new Date().toISOString()
          }
        ])
        .select();
        
      if (error) throw error;
      return data;
    },
    onSuccess: () => {
      toast({
        title: "Thank you!",
        description: "Your response has been submitted successfully",
      });
      setSubmitted(true);
    },
    onError: (error) => {
      toast({
        title: "Error",
        description: `Failed to submit response: ${error instanceof Error ? error.message : 'Unknown error'}`,
        variant: "destructive"
      });
    }
  });
  
  const onSubmit = (data: FormValues) => {
    if (!survey) return;
    
    const requiredQuestions = survey.questions.filter((q: any) => q.required);
    const missingRequiredAnswers = requiredQuestions.filter((q: any) => {
      const answer = data.answers[q.id];
      if (Array.isArray(answer)) {
        return answer.length === 0;
      }
      return !answer;
    });
    
    if (missingRequiredAnswers.length > 0) {
      toast({
        title: "Missing answers",
        description: "Please answer all required questions",
        variant: "destructive"
      });
      return;
    }
    
    const submission: SurveyResponseSubmission = {
      surveyId: survey.id,
      respondentName: isLoggedIn ? userData.username : 'Anonymous User',
      respondentEmail: isLoggedIn ? userData.email : '',
      respondentPhone: isLoggedIn ? userData.phone : '',
      respondentCompany: isLoggedIn ? userData.company : '',
      answers: data.answers,
      submittedAt: new Date().toISOString()
    };
    
    submitResponseMutation.mutate(submission);
  };
  
  if (isLoading) {
    return (
      <div className="min-h-screen flex flex-col bg-background">
        <Navbar />
        <main className="flex-1 w-full max-w-4xl mx-auto pt-24 px-6 pb-16 flex items-center justify-center">
          <div className="text-center">
            <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary mx-auto mb-4"></div>
            <p className="text-lg">Loading survey...</p>
          </div>
        </main>
        <Footer />
      </div>
    );
  }
  
  if (error || !survey) {
    return (
      <div className="min-h-screen flex flex-col bg-background">
        <Navbar />
        <main className="flex-1 w-full max-w-4xl mx-auto pt-24 px-6 pb-16 flex items-center justify-center">
          <Card className="w-full max-w-md">
            <CardHeader>
              <CardTitle className="flex items-center text-destructive">
                <AlertCircle className="mr-2 h-5 w-5" />
                Survey Not Found
              </CardTitle>
            </CardHeader>
            <CardContent>
              <p>The survey you're looking for doesn't exist or has been removed.</p>
            </CardContent>
            <CardFooter>
              <Button onClick={() => navigate('/')}>
                Return to Home
              </Button>
            </CardFooter>
          </Card>
        </main>
        <Footer />
      </div>
    );
  }
  
  if (submitted) {
    return (
      <div className="min-h-screen flex flex-col bg-background">
        <Navbar />
        <main className="flex-1 w-full max-w-4xl mx-auto pt-24 px-6 pb-16 flex items-center justify-center">
          <Card className="w-full max-w-md">
            <CardHeader>
              <CardTitle className="flex items-center text-green-600">
                <CheckCircle2 className="mr-2 h-5 w-5" />
                Thank You!
              </CardTitle>
            </CardHeader>
            <CardContent>
              <p className="mb-4">Your responses have been submitted successfully.</p>
              <p className="text-muted-foreground">Thank you for completing this survey.</p>
            </CardContent>
            <CardFooter>
              <Button onClick={() => navigate('/')}>
                Return to Home
              </Button>
            </CardFooter>
          </Card>
        </main>
        <Footer />
      </div>
    );
  }
  
  // Process questions from the survey data format
  const questions = Array.isArray(survey.questions) 
    ? survey.questions 
    : typeof survey.questions === 'object' 
      ? Object.values(survey.questions) 
      : [];
  
  return (
    <div className="min-h-screen flex flex-col bg-background">
      <Navbar />
      
      <main className="flex-1 w-full max-w-4xl mx-auto pt-24 px-6 pb-16">
        <Card className="w-full mb-8">
          <CardHeader>
            <CardTitle className="text-2xl">{survey.title}</CardTitle>
            {survey.description && (
              <p className="text-muted-foreground mt-2">{survey.description}</p>
            )}
          </CardHeader>
        </Card>
        
        <Form {...form}>
          <form onSubmit={form.handleSubmit(onSubmit)}>
            <div className="space-y-6">
              {questions.map((question: any, index) => (
                <Card key={question.id} className="w-full">
                  <CardHeader>
                    <CardTitle className="text-base font-medium flex items-start">
                      <span className="mr-2">{index + 1}.</span>
                      <span>
                        {question.title}
                        {question.required && <span className="text-red-500 ml-1">*</span>}
                      </span>
                    </CardTitle>
                    {question.description && (
                      <p className="text-sm text-muted-foreground pl-6">{question.description}</p>
                    )}
                  </CardHeader>
                  
                  <CardContent>
                    <FormField
                      control={form.control}
                      name={`answers.${question.id}`}
                      render={({ field }) => (
                        <FormItem>
                          <FormControl>
                            <>
                              {question.type === 'text' && (
                                <Textarea 
                                  placeholder="Your answer"
                                  value={field.value as string}
                                  onChange={(e) => field.onChange(e.target.value)}
                                  className="min-h-[100px]"
                                />
                              )}
                              
                              {question.type === 'single-choice' && question.options && (
                                <div className="space-y-2">
                                  {question.options.map((option: string, i: number) => (
                                    <div key={i} className="flex items-center">
                                      <input
                                        type="radio"
                                        id={`q${index}-o${i}`}
                                        name={`question-${question.id}`}
                                        value={option}
                                        checked={field.value === option}
                                        onChange={() => field.onChange(option)}
                                        className="mr-2"
                                      />
                                      <label htmlFor={`q${index}-o${i}`}>{option}</label>
                                    </div>
                                  ))}
                                </div>
                              )}
                              
                              {question.type === 'multiple-choice' && question.options && (
                                <div className="space-y-2">
                                  {question.options.map((option: string, i: number) => {
                                    const values = field.value as string[] || [];
                                    return (
                                      <div key={i} className="flex items-center">
                                        <input
                                          type="checkbox"
                                          id={`q${index}-o${i}`}
                                          value={option}
                                          checked={values.includes(option)}
                                          onChange={(e) => {
                                            const newValues = e.target.checked
                                              ? [...values, option]
                                              : values.filter(v => v !== option);
                                            field.onChange(newValues);
                                          }}
                                          className="mr-2"
                                        />
                                        <label htmlFor={`q${index}-o${i}`}>{option}</label>
                                      </div>
                                    );
                                  })}
                                </div>
                              )}
                              
                              {question.type === 'rating' && (
                                <StarRating
                                  name={`rating-${question.id}`}
                                  value={typeof field.value === 'string' ? field.value : ''}
                                  onChange={(value) => field.onChange(value)}
                                  required={question.required}
                                />
                              )}
                              
                              {question.type === 'nps' && (
                                <NPSRating
                                  name={`nps-${question.id}`}
                                  value={typeof field.value === 'string' ? field.value : ''}
                                  onChange={(value) => field.onChange(value)}
                                  required={question.required}
                                />
                              )}
                            </>
                          </FormControl>
                          <FormMessage />
                        </FormItem>
                      )}
                    />
                  </CardContent>
                </Card>
              ))}
            </div>
            
            <div className="mt-8 flex justify-end">
              <Button variant="outline" type="button" className="mr-4" onClick={() => navigate('/')}>
                Cancelar
              </Button>
              <Button type="submit" disabled={submitResponseMutation.isPending}>
                {submitResponseMutation.isPending ? 'Enviando...' : 'Enviar Respuestas'}
              </Button>
            </div>
          </form>
        </Form>
      </main>
      
      <Footer />
    </div>
  );
}
