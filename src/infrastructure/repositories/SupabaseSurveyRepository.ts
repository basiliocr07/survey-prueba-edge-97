
import { supabase } from '@/integrations/supabase/client';
import { Survey, SurveyStatistics } from '../../domain/models/Survey';
import { SurveyRepository } from '../../domain/repositories/SurveyRepository';

export class SupabaseSurveyRepository implements SurveyRepository {
  async getAllSurveys(): Promise<Survey[]> {
    const { data, error } = await supabase.from('surveys').select('*');
    
    if (error) {
      console.error('Error fetching surveys:', error);
      throw error;
    }
    
    return data.map(this.mapToSurvey) || [];
  }
  
  async getSurveyById(id: string): Promise<Survey | null> {
    const { data, error } = await supabase
      .from('surveys')
      .select('*')
      .eq('id', id)
      .maybeSingle();
    
    if (error) {
      console.error(`Error fetching survey with id ${id}:`, error);
      throw error;
    }
    
    return data ? this.mapToSurvey(data) : null;
  }
  
  async createSurvey(survey: Omit<Survey, 'id' | 'createdAt'>): Promise<Survey> {
    // Transform the survey data to match the database schema
    const dbSurvey = {
      title: survey.title,
      description: survey.description,
      questions: JSON.stringify(survey.questions),
      delivery_config: survey.deliveryConfig ? JSON.stringify(survey.deliveryConfig) : null
    };
    
    const { data, error } = await supabase
      .from('surveys')
      .insert([dbSurvey])
      .select()
      .single();
    
    if (error) {
      console.error('Error creating survey:', error);
      throw error;
    }
    
    return this.mapToSurvey(data);
  }
  
  async updateSurvey(survey: Survey): Promise<boolean> {
    // Transform the survey data to match the database schema
    const dbSurvey = {
      title: survey.title,
      description: survey.description,
      questions: JSON.stringify(survey.questions),
      delivery_config: survey.deliveryConfig ? JSON.stringify(survey.deliveryConfig) : null
    };
    
    const { error } = await supabase
      .from('surveys')
      .update(dbSurvey)
      .eq('id', survey.id);
    
    if (error) {
      console.error(`Error updating survey with id ${survey.id}:`, error);
      throw error;
    }
    
    return true;
  }
  
  async deleteSurvey(id: string): Promise<boolean> {
    const { error } = await supabase
      .from('surveys')
      .delete()
      .eq('id', id);
    
    if (error) {
      console.error(`Error deleting survey with id ${id}:`, error);
      throw error;
    }
    
    return true;
  }

  // Simplified method to map database results to Survey objects
  private mapToSurvey(item: any): Survey {
    // Parse questions from JSON string if necessary
    let questions = [];
    try {
      if (typeof item.questions === 'string') {
        questions = JSON.parse(item.questions);
      } else if (Array.isArray(item.questions)) {
        questions = item.questions;
      }
    } catch (e) {
      console.error('Error parsing questions:', e);
      questions = [];
    }

    // Parse delivery_config from JSON string if necessary
    let deliveryConfig;
    try {
      if (item.delivery_config) {
        const configData = typeof item.delivery_config === 'string' 
          ? JSON.parse(item.delivery_config) 
          : item.delivery_config;
        
        deliveryConfig = {
          type: String(configData.type || 'manual'),
          emailAddresses: Array.isArray(configData.emailAddresses) 
            ? configData.emailAddresses 
            : [],
        };

        // Add schedule if it exists
        if (configData.schedule) {
          deliveryConfig.schedule = {
            frequency: String(configData.schedule.frequency || 'daily'),
            dayOfMonth: Number(configData.schedule.dayOfMonth || 1),
            dayOfWeek: Number(configData.schedule.dayOfWeek || 1),
            time: String(configData.schedule.time || '12:00'),
          };
        }

        // Add trigger if it exists
        if (configData.trigger) {
          deliveryConfig.trigger = {
            type: String(configData.trigger.type || 'ticket-closed'),
            delayHours: Number(configData.trigger.delayHours || 24),
            sendAutomatically: Boolean(configData.trigger.sendAutomatically || false),
          };
        }
      }
    } catch (e) {
      console.error('Error parsing delivery config:', e);
      deliveryConfig = undefined;
    }

    return {
      id: String(item.id),
      title: String(item.title),
      description: item.description ? String(item.description) : undefined,
      questions: questions,
      createdAt: String(item.created_at),
      deliveryConfig: deliveryConfig
    };
  }
  
  async getSurveysByStatus(status: string): Promise<Survey[]> {
    // This is a simple implementation that doesn't filter by status
    // since we're removing restrictions
    const { data, error } = await supabase
      .from('surveys')
      .select('*');
    
    if (error) {
      console.error('Error fetching surveys by status:', error);
      throw error;
    }
    
    return (data || []).map(this.mapToSurvey);
  }
  
  async getSurveyStatistics(surveyId: string): Promise<SurveyStatistics> {
    // Obtenemos las respuestas para esta encuesta
    const { data: responses, error: responsesError } = await supabase
      .from('survey_responses')
      .select('*')
      .eq('survey_id', surveyId);
    
    if (responsesError) {
      console.error(`Error fetching responses for survey ${surveyId}:`, responsesError);
      throw responsesError;
    }
    
    // Obtenemos la encuesta
    const { data: survey, error: surveyError } = await supabase
      .from('surveys')
      .select('*')
      .eq('id', surveyId)
      .maybeSingle();
    
    if (surveyError) {
      console.error(`Error fetching survey ${surveyId}:`, surveyError);
      throw surveyError;
    }
    
    if (!survey) {
      throw new Error(`Survey with id ${surveyId} not found`);
    }
    
    // Calculamos estadísticas básicas
    const totalResponses = responses.length;
    let completionRate = 0;
    let averageCompletionTime = 0;
    
    // Calculamos tiempo promedio de respuesta si hay datos disponibles
    if (totalResponses > 0) {
      // Calculate completion time if available
      let totalCompletionTime = 0;
      let responsesWithCompletionTime = 0;
      
      for (const response of responses) {
        // Fixed the property access to handle any potential completion_time format
        const completionTime = response.completion_time || 
                              (response as any).completion_time || 
                              0;
        
        if (completionTime) {
          totalCompletionTime += Number(completionTime);
          responsesWithCompletionTime++;
        }
      }
      
      averageCompletionTime = responsesWithCompletionTime > 0 
        ? totalCompletionTime / responsesWithCompletionTime 
        : 0;
        
      completionRate = 100; // Asumimos una tasa de finalización del 100% para respuestas enviadas
    }
    
    return {
      totalResponses,
      averageCompletionTime,
      completionRate,
      questionStats: [] // En un caso real, aquí calcularíamos estadísticas para cada pregunta
    };
  }
  
  async sendSurveyEmails(surveyId: string, emailAddresses: string[]): Promise<boolean> {
    // En un caso real, aquí enviaríamos correos electrónicos.
    // Como es una simulación, simplemente devolvemos true
    console.log(`Sending survey ${surveyId} to:`, emailAddresses);
    return true;
  }
}
