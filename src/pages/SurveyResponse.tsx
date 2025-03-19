
import { useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { z } from 'zod';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { useToast } from '@/hooks/use-toast';
import Navbar from '@/components/layout/Navbar';
import Footer from '@/components/layout/Footer';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle, CardDescription, CardFooter } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Textarea } from '@/components/ui/textarea';
import { RadioGroup, RadioGroupItem } from '@/components/ui/radio-group';
import { Checkbox } from '@/components/ui/checkbox';
import { Separator } from '@/components/ui/separator';
import { ArrowRight, CheckCircle2 } from 'lucide-react';
import { SurveyResponseSubmission } from '@/types/surveyTypes';

// Mock survey data - in a real app, this would come from an API
const mockSurveys = {
  'survey123': {
    id: 'survey123',
    title: 'Customer Satisfaction Survey',
    description: 'Please help us improve our services by answering a few questions.',
    questions: [
      {
        id: 'q1',
        type: 'rating',
        question: 'How would you rate your overall experience with our service?',
        options: ['1', '2', '3', '4', '5'],
        required: true
      },
      {
        id: 'q2',
        type: 'text',
        question: 'What did you like most about our service?',
        required: false
      },
      {
        id: 'q3',
        type: 'text',
        question: 'What could we improve?',
        required: true
      },
      {
        id: 'q4',
        type: 'multiple',
        question: 'Which features do you use most often?',
        options: ['Dashboard', 'Reports', 'Surveys', 'Analytics', 'Other'],
        required: true
      }
    ]
  }
};

const formSchema = z.object({
  name: z.string().min(1, { message: 'Name is required' }),
  email: z.string().email({ message: 'Valid email is required' }),
  phone: z.string().optional(),
  company: z.string().optional(),
  answers: z.record(z.union([z.string(), z.array(z.string())]))
});

type FormData = z.infer<typeof formSchema>;

export default function SurveyResponse() {
  const { surveyId } = useParams();
  const navigate = useNavigate();
  const { toast } = useToast();
  const [submitted, setSubmitted] = useState(false);
  
  const survey = surveyId ? mockSurveys[surveyId as keyof typeof mockSurveys] : null;

  const { register, handleSubmit, formState: { errors } } = useForm<FormData>({
    resolver: zodResolver(formSchema),
    defaultValues: {
      name: '',
      email: '',
      phone: '',
      company: '',
      answers: {}
    }
  });

  if (!survey) {
    return (
      <div className="min-h-screen flex flex-col bg-background">
        <Navbar />
        <main className="flex-1 w-full px-4 sm:px-6 lg:px-8 pt-24 pb-20 flex justify-center">
          <div className="w-[900px] max-w-[900px] h-full bg-white p-6">
            <Card>
              <CardHeader>
                <CardTitle>Survey Not Found</CardTitle>
                <CardDescription>
                  The survey you're looking for doesn't exist or has expired.
                </CardDescription>
              </CardHeader>
              <CardFooter>
                <Button onClick={() => navigate('/')}>Return to Home</Button>
              </CardFooter>
            </Card>
          </div>
        </main>
        <Footer />
      </div>
    );
  }

  const onSubmit = (data: FormData) => {
    // Preparar los datos para enviar al servidor
    const submission: SurveyResponseSubmission = {
      surveyId: survey.id,
      respondentName: data.name.trim(),
      respondentEmail: data.email.trim(),
      respondentPhone: data.phone?.trim(),
      respondentCompany: data.company?.trim(),
      answers: data.answers,
      submittedAt: new Date().toISOString() // This is now valid as we've added it to the type definition
    };
    
    console.log('Survey response submitted:', submission);
    
    // En una aplicación real, enviaríamos los datos a una API
    // fetch('/api/survey-responses', {
    //   method: 'POST',
    //   headers: { 'Content-Type': 'application/json' },
    //   body: JSON.stringify(submission)
    // })
    
    // Guardar en localStorage para simulación
    const savedResponses = JSON.parse(localStorage.getItem('surveyResponses') || '[]');
    localStorage.setItem('surveyResponses', JSON.stringify([...savedResponses, submission]));
    
    toast({
      title: "Response submitted",
      description: "Thank you for completing our survey!",
    });
    
    setSubmitted(true);
  };

  if (submitted) {
    return (
      <div className="min-h-screen flex flex-col bg-background">
        <Navbar />
        <main className="flex-1 w-full px-4 sm:px-6 lg:px-8 pt-24 pb-20 flex justify-center">
          <div className="w-[900px] max-w-[900px] h-full bg-white p-6">
            <Card className="text-center">
              <CardHeader>
                <div className="mx-auto mb-4">
                  <CheckCircle2 className="h-12 w-12 text-green-500" />
                </div>
                <CardTitle className="text-2xl">Thank You!</CardTitle>
                <CardDescription className="text-lg">
                  Your response has been recorded successfully.
                </CardDescription>
              </CardHeader>
              <CardContent>
                <p className="mb-6">We appreciate your feedback and will use it to improve our services.</p>
                <Button onClick={() => navigate('/')}>Return to Home</Button>
              </CardContent>
            </Card>
          </div>
        </main>
        <Footer />
      </div>
    );
  }

  return (
    <div className="min-h-screen flex flex-col bg-background">
      <Navbar />
      <main className="flex-1 w-full px-4 sm:px-6 lg:px-8 pt-24 pb-20 flex justify-center">
        <div className="w-[900px] max-w-[900px] h-full bg-white p-6">
          <Card>
            <CardHeader>
              <CardTitle>{survey.title}</CardTitle>
              <CardDescription>{survey.description}</CardDescription>
            </CardHeader>
            <form onSubmit={handleSubmit(onSubmit)}>
              <CardContent className="space-y-6">
                <div className="space-y-4">
                  <h3 className="text-lg font-medium">Your Information</h3>
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div className="space-y-2">
                      <Label htmlFor="name">Name <span className="text-red-500">*</span></Label>
                      <Input id="name" {...register('name')} />
                      {errors.name && (
                        <p className="text-sm text-destructive">{errors.name.message}</p>
                      )}
                    </div>
                    <div className="space-y-2">
                      <Label htmlFor="email">Email <span className="text-red-500">*</span></Label>
                      <Input id="email" type="email" {...register('email')} />
                      {errors.email && (
                        <p className="text-sm text-destructive">{errors.email.message}</p>
                      )}
                    </div>
                  </div>
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div className="space-y-2">
                      <Label htmlFor="phone">Phone</Label>
                      <Input id="phone" {...register('phone')} />
                    </div>
                    <div className="space-y-2">
                      <Label htmlFor="company">Company</Label>
                      <Input id="company" {...register('company')} />
                    </div>
                  </div>
                </div>
                
                <Separator />
                
                <div className="space-y-8">
                  <h3 className="text-lg font-medium">Survey Questions</h3>
                  
                  {survey.questions.map((question, index) => (
                    <div key={question.id} className="space-y-3">
                      <h4 className="font-medium">
                        {index + 1}. {question.question}
                        {question.required && <span className="text-red-500 ml-1">*</span>}
                      </h4>
                      
                      {question.type === 'text' && (
                        <div>
                          <Textarea 
                            {...register(`answers.${question.id}` as const, { 
                              required: question.required ? 'This field is required' : false 
                            })} 
                            placeholder="Your answer"
                          />
                          {errors.answers?.[question.id] && (
                            <p className="text-sm text-destructive mt-1">
                              {errors.answers[question.id]?.message as string}
                            </p>
                          )}
                        </div>
                      )}
                      
                      {question.type === 'rating' && (
                        <RadioGroup defaultValue="">
                          <div className="flex space-x-6">
                            {question.options?.map((option) => (
                              <div key={option} className="flex items-center space-x-2">
                                <RadioGroupItem
                                  id={`${question.id}-${option}`}
                                  value={option}
                                  {...register(`answers.${question.id}` as const, {
                                    required: question.required ? 'Please select a rating' : false
                                  })}
                                />
                                <Label htmlFor={`${question.id}-${option}`}>{option}</Label>
                              </div>
                            ))}
                          </div>
                          {errors.answers?.[question.id] && (
                            <p className="text-sm text-destructive mt-1">
                              {errors.answers[question.id]?.message as string}
                            </p>
                          )}
                        </RadioGroup>
                      )}
                      
                      {question.type === 'multiple' && (
                        <div className="grid grid-cols-1 sm:grid-cols-2 gap-3">
                          {question.options?.map((option) => (
                            <div key={option} className="flex items-center space-x-2">
                              <Checkbox 
                                id={`${question.id}-${option}`}
                                value={option}
                                {...register(`answers.${question.id}` as const, {
                                  validate: v => 
                                    !question.required || 
                                    (Array.isArray(v) && v.length > 0) || 
                                    'Please select at least one option'
                                })}
                              />
                              <Label htmlFor={`${question.id}-${option}`}>{option}</Label>
                            </div>
                          ))}
                          {errors.answers?.[question.id] && (
                            <p className="text-sm text-destructive col-span-2 mt-1">
                              {errors.answers[question.id]?.message as string}
                            </p>
                          )}
                        </div>
                      )}
                    </div>
                  ))}
                </div>
              </CardContent>
              
              <CardFooter className="flex justify-end space-x-4">
                <Button variant="outline" type="button" onClick={() => navigate('/')}>
                  Cancel
                </Button>
                <Button type="submit">
                  Submit Response <ArrowRight className="ml-2 h-4 w-4" />
                </Button>
              </CardFooter>
            </form>
          </Card>
        </div>
      </main>
      <Footer />
    </div>
  );
}
