
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

const formSchema = z.object({
  respondentName: z.string().min(1, { message: "Name is required" }),
  respondentEmail: z.string().email({ message: "Valid email is required" }),
  respondentPhone: z.string().optional(),
  respondentCompany: z.string().optional(),
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
  
  const [survey, setSurvey] = useState<Survey | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [submitted, setSubmitted] = useState(false);
  
  const form = useForm<FormValues>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      respondentName: "",
      respondentEmail: "",
      respondentPhone: "",
      respondentCompany: "",
      answers: {}
    }
  });
  
  useEffect(() => {
    const loadSurvey = () => {
      try {
        const savedSurveys = JSON.parse(localStorage.getItem('surveys') || '[]');
        const foundSurvey = savedSurveys.find((s: Survey) => s.id === surveyId);
        
        if (foundSurvey) {
          setSurvey(foundSurvey);
          
          const initialAnswers: Record<string, any> = {};
          foundSurvey.questions.forEach(question => {
            if (question.type === 'multiple-choice') {
              initialAnswers[question.id] = [];
            } else {
              initialAnswers[question.id] = '';
            }
          });
          
          form.reset({ 
            respondentName: "", 
            respondentEmail: "", 
            respondentPhone: "", 
            respondentCompany: "", 
            answers: initialAnswers 
          });
        } else {
          setError('Survey not found');
        }
        
        setLoading(false);
      } catch (err) {
        setError('Failed to load survey');
        setLoading(false);
      }
    };
    
    loadSurvey();
  }, [surveyId, form]);
  
  const onSubmit = (data: FormValues) => {
    if (!survey) return;
    
    const requiredQuestions = survey.questions.filter(q => q.required);
    const missingRequiredAnswers = requiredQuestions.filter(q => {
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
      respondentName: data.respondentName,
      respondentEmail: data.respondentEmail,
      respondentPhone: data.respondentPhone,
      respondentCompany: data.respondentCompany,
      answers: data.answers,
      submittedAt: new Date().toISOString()
    };
    
    console.log('Saving survey response:', submission);
    
    const savedResponses = JSON.parse(localStorage.getItem('surveyResponses') || '[]');
    localStorage.setItem('surveyResponses', JSON.stringify([...savedResponses, submission]));
    
    toast({
      title: "Thank you!",
      description: "Your response has been submitted successfully",
    });
    
    setSubmitted(true);
  };
  
  if (loading) {
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
            <Card className="w-full mb-6">
              <CardHeader>
                <CardTitle className="text-lg">Your Information</CardTitle>
              </CardHeader>
              <CardContent className="space-y-4">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <FormField
                    control={form.control}
                    name="respondentName"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Name <span className="text-red-500">*</span></FormLabel>
                        <FormControl>
                          <Input placeholder="Your name" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  
                  <FormField
                    control={form.control}
                    name="respondentEmail"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Email <span className="text-red-500">*</span></FormLabel>
                        <FormControl>
                          <Input type="email" placeholder="Your email" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </div>
                
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <FormField
                    control={form.control}
                    name="respondentPhone"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Phone</FormLabel>
                        <FormControl>
                          <Input placeholder="Your phone number" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                  
                  <FormField
                    control={form.control}
                    name="respondentCompany"
                    render={({ field }) => (
                      <FormItem>
                        <FormLabel>Company</FormLabel>
                        <FormControl>
                          <Input placeholder="Your company" {...field} />
                        </FormControl>
                        <FormMessage />
                      </FormItem>
                    )}
                  />
                </div>
              </CardContent>
            </Card>
            
            <div className="space-y-6">
              {survey.questions.map((question, index) => (
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
                                {question.options.map((option, i) => (
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
                                {question.options.map((option, i) => {
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
                Cancel
              </Button>
              <Button type="submit">
                Submit Responses
              </Button>
            </div>
          </form>
        </Form>
      </main>
      
      <Footer />
    </div>
  );
}
