
import { Survey, SurveyStatistics } from '../../domain/models/Survey';
import { SurveyRepository } from '../../domain/repositories/SurveyRepository';
import { supabase } from '../../integrations/supabase/client';
import { Json } from '../../integrations/supabase/types';

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
      .insert({
        title: survey.title,
        description: survey.description,
        questions: survey.questions as unknown as Json,
        delivery_config: survey.deliveryConfig as unknown as Json
      })
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
        questions: survey.questions as unknown as Json,
        delivery_config: survey.deliveryConfig as unknown as Json
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
        
        // Add completion_time if it exists in the database record
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
        
        const answerKey = Array.isArray(answer) ? answer.join(', ') : String(answer);
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
      const { data, error } = await supabase.functions.invoke('send-survey-emails', {
        body: { surveyId, emailAddresses }
      });
      
      if (error) {
        console.error('Error sending survey emails:', error);
        return false;
      }
      
      return data?.success || false;
    } catch (error) {
      console.error('Error sending survey emails', error);
      return false;
    }
  }

  private mapToSurvey(data: any): Survey {
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

    // Parse the delivery_config if it exists
    let deliveryConfig = undefined;
    try {
      if (data.delivery_config) {
        deliveryConfig = typeof data.delivery_config === 'string' 
          ? JSON.parse(data.delivery_config) 
          : data.delivery_config;
      }
    } catch (e) {
      console.error('Error parsing delivery config:', e);
    }

    return {
      id: data.id,
      title: data.title,
      description: data.description || undefined,
      questions: parsedQuestions,
      createdAt: data.created_at,
      deliveryConfig: deliveryConfig
    };
  }
}
