
import { Survey, SurveyStatistics, DeliveryConfig } from '../../domain/models/Survey';
import { SurveyRepository } from '../../domain/repositories/SurveyRepository';
import { supabase } from '../../integrations/supabase/client';

// Define a simple interface for response records to avoid complex type issues
interface ResponseRecord {
  id: string;
  survey_id: string;
  respondent_name: string;
  respondent_email: string | null;
  respondent_company: string | null;
  respondent_phone: string | null;
  submitted_at: string;
  answers: Record<string, any>;
  completion_time?: number;
}

export class SupabaseSurveyRepository implements SurveyRepository {
  async getAllSurveys(): Promise<Survey[]> {
    const { data, error } = await supabase
      .from('surveys')
      .select('*')
      .order('created_at', { ascending: false });

    if (error) throw error;
    
    return (data || []).map(this.mapToSurvey);
  }

  async getSurveyById(id: string): Promise<Survey | null> {
    const { data, error } = await supabase
      .from('surveys')
      .select('*')
      .eq('id', id)
      .single();

    if (error) return null;
    if (!data) return null;
    
    return this.mapToSurvey(data);
  }

  async createSurvey(survey: Omit<Survey, 'id' | 'createdAt'>): Promise<Survey> {
    const { data, error } = await supabase
      .from('surveys')
      .insert([{
        title: survey.title,
        description: survey.description,
        questions: survey.questions as any,
        delivery_config: survey.deliveryConfig as any
      }])
      .select();

    if (error) throw error;
    
    return this.mapToSurvey(data[0]);
  }

  async updateSurvey(survey: Survey): Promise<boolean> {
    const { error } = await supabase
      .from('surveys')
      .update({
        title: survey.title,
        description: survey.description,
        questions: survey.questions as any,
        delivery_config: survey.deliveryConfig as any
      })
      .eq('id', survey.id);

    return !error;
  }

  async deleteSurvey(id: string): Promise<boolean> {
    const { error } = await supabase
      .from('surveys')
      .delete()
      .eq('id', id);

    return !error;
  }

  async getSurveysByStatus(status: string): Promise<Survey[]> {
    const { data, error } = await supabase
      .from('surveys')
      .select('*')
      .eq('status', status)
      .order('created_at', { ascending: false });

    if (error) throw error;
    
    return (data || []).map(this.mapToSurvey);
  }

  async getSurveyStatistics(surveyId: string): Promise<SurveyStatistics> {
    const { data, error } = await supabase
      .from('survey_responses')
      .select('*')
      .eq('survey_id', surveyId);

    if (error) throw error;

    const responses: ResponseRecord[] = [];
    
    if (data) {
      for (const item of data) {
        const responseData: ResponseRecord = {
          id: item.id,
          survey_id: item.survey_id,
          respondent_name: item.respondent_name,
          respondent_email: item.respondent_email,
          respondent_company: item.respondent_company,
          respondent_phone: item.respondent_phone,
          submitted_at: item.submitted_at,
          answers: item.answers as Record<string, any> || {}
        };
        
        if ('completion_time' in item && item.completion_time !== undefined) {
          responseData.completion_time = 
            typeof item.completion_time === 'number' ? item.completion_time : 
            typeof item.completion_time === 'string' ? parseInt(item.completion_time, 10) : 
            undefined;
        }
        
        responses.push(responseData);
      }
    }

    const survey = await this.getSurveyById(surveyId);
    if (!survey) throw new Error('Survey not found');

    const totalResponses = responses.length;
    
    let averageCompletionTime = 0;
    let completionTimeSum = 0;
    let completionTimeCount = 0;
    
    for (const response of responses) {
      if (response.completion_time !== undefined) {
        completionTimeSum += response.completion_time;
        completionTimeCount++;
      }
    }
    
    if (completionTimeCount > 0) {
      averageCompletionTime = completionTimeSum / completionTimeCount;
    }
    
    const questionStats = survey.questions.map(question => {
      const answerCounts: Record<string, number> = {};
      
      for (const response of responses) {
        if (!response.answers) continue;
        
        const answer = response.answers[question.id];
        if (answer === undefined || answer === null) continue;
        
        const answerKey = Array.isArray(answer) 
          ? answer.join(', ') 
          : String(answer);
          
        answerCounts[answerKey] = (answerCounts[answerKey] || 0) + 1;
      }
      
      return {
        questionId: question.id,
        questionTitle: question.title,
        responses: Object.entries(answerCounts).map(([answer, count]) => ({
          answer,
          count,
          percentage: totalResponses ? (count / totalResponses) * 100 : 0
        }))
      };
    });

    let completedResponsesCount = 0;
    
    for (const response of responses) {
      if (response.answers && Object.keys(response.answers).length > 0) {
        completedResponsesCount++;
      }
    }
    
    const completionRate = totalResponses ? (completedResponsesCount / totalResponses) * 100 : 0;
    
    return {
      totalResponses,
      averageCompletionTime,
      completionRate,
      questionStats
    };
  }

  async sendSurveyEmails(surveyId: string, emailAddresses: string[]): Promise<boolean> {
    try {
      console.log('Sending survey emails:', { surveyId, emailAddresses });
      
      const { data, error } = await supabase.functions.invoke('send-survey-emails', {
        body: { surveyId, emailAddresses }
      });
      
      if (error) {
        console.error('Error sending survey emails:', error);
        return false;
      }
      
      console.log('Survey email response:', data);
      return data?.success || false;
    } catch (error) {
      console.error('Error sending survey emails:', error);
      return false;
    }
  }

  private mapToSurvey(data: any): Survey {
    // Parse questions with explicit typing to avoid deep inference
    let parsedQuestions;
    try {
      parsedQuestions = Array.isArray(data.questions) 
        ? data.questions.map((q: any) => ({
            id: q.id || '',
            title: q.title || '',
            description: q.description,
            type: q.type || '',
            required: q.required || false,
            options: q.options,
            settings: q.settings
          }))
        : [];
    } catch (e) {
      console.error('Error parsing questions:', e);
      parsedQuestions = [];
    }

    // Create a simplified DeliveryConfig with explicit typing
    let deliveryConfig: DeliveryConfig | undefined = undefined;
    
    try {
      if (data.delivery_config) {
        // Normalize data to simple object to avoid deep type inference
        const rawConfig = typeof data.delivery_config === 'string' 
          ? JSON.parse(data.delivery_config) 
          : data.delivery_config;
          
        // Create a flat, simplified object with only the required fields
        deliveryConfig = {
          type: (rawConfig.type as 'manual' | 'scheduled' | 'triggered') || 'manual',
          emailAddresses: Array.isArray(rawConfig.emailAddresses) ? rawConfig.emailAddresses : []
        };

        // Only add schedule if present, with explicit types
        if (rawConfig.schedule) {
          deliveryConfig.schedule = {
            frequency: (rawConfig.schedule.frequency as 'daily' | 'weekly' | 'monthly') || 'monthly',
            dayOfMonth: typeof rawConfig.schedule.dayOfMonth === 'number' ? rawConfig.schedule.dayOfMonth : undefined,
            dayOfWeek: typeof rawConfig.schedule.dayOfWeek === 'number' ? rawConfig.schedule.dayOfWeek : undefined,
            time: rawConfig.schedule.time || '09:00',
            startDate: rawConfig.schedule.startDate ? new Date(rawConfig.schedule.startDate) : undefined
          };
        }
        
        // Only add trigger if present, with explicit types
        if (rawConfig.trigger) {
          deliveryConfig.trigger = {
            type: (rawConfig.trigger.type as 'ticket-closed' | 'purchase-completed') || 'ticket-closed',
            delayHours: typeof rawConfig.trigger.delayHours === 'number' ? rawConfig.trigger.delayHours : 24,
            sendAutomatically: Boolean(rawConfig.trigger.sendAutomatically)
          };
        }
      }
    } catch (e) {
      console.error('Error parsing delivery config:', e);
    }

    // Return a simpler object with explicit typing to avoid deep inference
    return {
      id: data.id || '',
      title: data.title || '',
      description: data.description || undefined,
      questions: parsedQuestions,
      createdAt: data.created_at || '',
      deliveryConfig: deliveryConfig
    };
  }
}
