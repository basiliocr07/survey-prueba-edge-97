
import { SurveyRepository } from "@/domain/repositories/SurveyRepository";
import { Survey, SurveyQuestion, SurveyStatistics, DeliveryConfig } from "@/domain/models/Survey";
import { supabase } from "@/integrations/supabase/client";
import { Json } from "@/integrations/supabase/types";

export class SupabaseSurveyRepository implements SurveyRepository {
  async createSurvey(survey: Omit<Survey, 'id' | 'createdAt'>): Promise<Survey> {
    const { data, error } = await supabase
      .from('surveys')
      .insert([
        {
          title: survey.title,
          description: survey.description || '',
          questions: survey.questions as unknown as Json,
          delivery_config: survey.deliveryConfig as unknown as Json,
          created_at: new Date().toISOString(),
        }
      ])
      .select()
      .single();

    if (error) {
      console.error('Error creating survey:', error);
      throw new Error('Failed to create survey');
    }

    return this.mapDbSurveyToModel(data);
  }

  async getSurveyById(id: string): Promise<Survey | null> {
    const { data, error } = await supabase
      .from('surveys')
      .select('*')
      .eq('id', id)
      .single();

    if (error) {
      console.error('Error fetching survey:', error);
      return null;
    }

    if (!data) {
      return null;
    }

    return this.mapDbSurveyToModel(data);
  }

  async getAllSurveys(): Promise<Survey[]> {
    const { data, error } = await supabase
      .from('surveys')
      .select('*')
      .order('created_at', { ascending: false });

    if (error) {
      console.error('Error fetching surveys:', error);
      return [];
    }

    return data.map(this.mapDbSurveyToModel);
  }

  async updateSurvey(survey: Survey): Promise<boolean> {
    const { error } = await supabase
      .from('surveys')
      .update({
        title: survey.title,
        description: survey.description || '',
        questions: survey.questions as unknown as Json,
        delivery_config: survey.deliveryConfig as unknown as Json,
        updated_at: new Date().toISOString()
      })
      .eq('id', survey.id);

    if (error) {
      console.error('Error updating survey:', error);
      return false;
    }

    return true;
  }

  async deleteSurvey(id: string): Promise<boolean> {
    const { error } = await supabase
      .from('surveys')
      .delete()
      .eq('id', id);

    if (error) {
      console.error('Error deleting survey:', error);
      return false;
    }
    
    return true;
  }

  async getSurveysByUser(userId: string): Promise<Survey[]> {
    // Note: This is a simplified implementation since we don't have a user_id column
    // In a real app, you'd filter by created_by or similar
    const { data, error } = await supabase
      .from('surveys')
      .select('*')
      .order('created_at', { ascending: false });

    if (error) {
      console.error('Error fetching user surveys:', error);
      return [];
    }

    return data.map(this.mapDbSurveyToModel);
  }

  async getSurveysByStatus(status: string): Promise<Survey[]> {
    // Note: This is a simplified implementation since we may not have a status column
    const { data, error } = await supabase
      .from('surveys')
      .select('*')
      .order('created_at', { ascending: false });

    if (error) {
      console.error('Error fetching surveys by status:', error);
      return [];
    }

    // Filter by status if needed - for now we return all
    return data.map(this.mapDbSurveyToModel);
  }

  async sendSurveyEmails(surveyId: string, emailAddresses: string[]): Promise<boolean> {
    try {
      console.log(`Sending emails for survey ${surveyId} to:`, emailAddresses);
      
      // Obtener los detalles de la encuesta
      const survey = await this.getSurveyById(surveyId);
      if (!survey) {
        console.error('Survey not found');
        return false;
      }
      
      // Llamar a la función de Supabase para enviar los correos
      const { data, error } = await supabase.functions.invoke('send-survey-emails', {
        body: {
          surveyId,
          emailAddresses,
          surveyTitle: survey.title
        }
      });
      
      if (error) {
        console.error('Error calling Supabase function to send emails:', error);
        return false;
      }
      
      console.log('Email sending result:', data);
      return data?.success === true;
    } catch (error) {
      console.error('Error sending survey emails:', error);
      return false;
    }
  }

  async scheduleEmailDelivery(surveyId: string, config: DeliveryConfig): Promise<boolean> {
    try {
      // Primero actualizamos la configuración de la encuesta
      const survey = await this.getSurveyById(surveyId);
      if (!survey) {
        console.error('Survey not found for scheduling');
        return false;
      }
      
      // Actualizamos la encuesta con la nueva configuración
      const updated = await this.updateSurvey({
        ...survey,
        deliveryConfig: config
      });
      
      if (!updated) {
        console.error('Failed to update survey with delivery config');
        return false;
      }
      
      // Llamamos a la función de Supabase para programar los envíos
      const { data, error } = await supabase.functions.invoke('schedule-survey-emails', {
        body: {
          surveyId,
          config
        }
      });
      
      if (error) {
        console.error('Error calling Supabase function to schedule emails:', error);
        return false;
      }
      
      console.log('Email scheduling result:', data);
      return data?.success === true;
    } catch (error) {
      console.error('Error scheduling survey emails:', error);
      return false;
    }
  }

  private mapDbSurveyToModel(dbSurvey: any): Survey {
    const questions = Array.isArray(dbSurvey.questions) 
      ? dbSurvey.questions as SurveyQuestion[]
      : [] as SurveyQuestion[];
    
    return {
      id: dbSurvey.id,
      title: dbSurvey.title,
      description: dbSurvey.description || '',
      questions: questions,
      deliveryConfig: dbSurvey.delivery_config || undefined,
      createdAt: dbSurvey.created_at,
      updatedAt: dbSurvey.updated_at || undefined,
      status: dbSurvey.status || 'draft',
    };
  }

  async getSurveyStatistics(surveyId: string): Promise<SurveyStatistics> {
    // Get all responses for the survey
    const { data: responses, error } = await supabase
      .from('survey_responses')
      .select('*')
      .eq('survey_id', surveyId);

    if (error) {
      console.error('Error fetching survey responses:', error);
      return {
        totalResponses: 0,
        completionRate: 0,
        averageCompletionTime: 0,
        questionStats: []
      };
    }

    // Calculate statistics
    const responseCount = responses.length;
    
    // Get the survey to access its questions
    const { data: surveyData } = await supabase
      .from('surveys')
      .select('*')
      .eq('id', surveyId)
      .single();
    
    const surveyQuestions = surveyData?.questions as unknown as SurveyQuestion[] || [];
    
    // For each response, analyze the answers
    const questionStats: {
      questionId: string;
      questionTitle: string;
      responses: {
        answer: string;
        count: number;
        percentage: number;
      }[];
    }[] = [];
    
    if (Array.isArray(surveyQuestions)) {
      for (const question of surveyQuestions) {
        const questionId = question.id;
        const questionTitle = question.title;
        
        const answerCounts: Record<string, number> = {};
        let totalAnswers = 0;
        
        // Count the answers for this question
        for (const response of responses) {
          const answers = response.answers as unknown as Record<string, string | string[]>;
          const answer = answers[questionId];
          
          if (answer) {
            if (Array.isArray(answer)) {
              // For multiple choice questions
              for (const option of answer) {
                answerCounts[option] = (answerCounts[option] || 0) + 1;
                totalAnswers++;
              }
            } else {
              // For single choice or text questions
              answerCounts[answer] = (answerCounts[answer] || 0) + 1;
              totalAnswers++;
            }
          }
        }
        
        // Convert the answer counts to percentages
        const responseData = Object.entries(answerCounts).map(([answer, count]) => ({
          answer,
          count,
          percentage: totalAnswers > 0 ? (count / totalAnswers) * 100 : 0
        }));
        
        questionStats.push({
          questionId,
          questionTitle,
          responses: responseData
        });
      }
    }
    
    return {
      totalResponses: responseCount,
      completionRate: responseCount > 0 ? 100 : 0, // Simplified for now
      averageCompletionTime: 0, // We don't have completion time in the schema
      questionStats
    };
  }
}
