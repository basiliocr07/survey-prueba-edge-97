
import { SurveyRepository } from "@/domain/repositories/SurveyRepository";
import { Survey, SurveyQuestion, SurveyStatistics } from "@/domain/models/Survey";
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
    const { data, error } = await supabase
      .from('surveys')
      .update({
        title: survey.title,
        description: survey.description || '',
        questions: survey.questions as unknown as Json,
        delivery_config: survey.deliveryConfig as unknown as Json,
        updated_at: new Date().toISOString()
      })
      .eq('id', survey.id)
      .select()
      .single();

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
    const { data, error } = await supabase
      .from('surveys')
      .select('*')
      .eq('created_by', userId)
      .order('created_at', { ascending: false });

    if (error) {
      console.error('Error fetching user surveys:', error);
      return [];
    }

    return data.map(this.mapDbSurveyToModel);
  }

  // Implement missing methods required by the interface
  async getSurveysByStatus(status: string): Promise<Survey[]> {
    const { data, error } = await supabase
      .from('surveys')
      .select('*')
      .eq('status', status)
      .order('created_at', { ascending: false });

    if (error) {
      console.error('Error fetching surveys by status:', error);
      return [];
    }

    return data.map(this.mapDbSurveyToModel);
  }

  async sendSurveyEmails(surveyId: string, emailAddresses: string[]): Promise<boolean> {
    try {
      // In a real implementation, this would call a Supabase Edge Function
      // or another service to send emails
      console.log(`Would send emails for survey ${surveyId} to:`, emailAddresses);
      
      // For now, simulate success
      return true;
    } catch (error) {
      console.error('Error sending survey emails:', error);
      return false;
    }
  }

  private mapDbSurveyToModel(dbSurvey: any): Survey {
    const questions = Array.isArray(dbSurvey.questions) 
      ? dbSurvey.questions 
      : [];
    
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
    
    // Count responses with completion times
    let totalCompletionTime = 0;
    let responsesWithCompletionTime = 0;
    
    for (const response of responses) {
      // Use safe property access since completion_time might not exist
      const completionTime = response.completion_time as number | undefined;
      
      if (typeof completionTime === 'number') {
        totalCompletionTime += completionTime;
        responsesWithCompletionTime++;
      }
    }

    // Calculate averages
    const completionRate = responseCount > 0 ? 
      (responsesWithCompletionTime / responseCount) * 100 : 0;
    
    const averageCompletionTime = responsesWithCompletionTime > 0 ? 
      totalCompletionTime / responsesWithCompletionTime : 0;

    // For now, we'll return an empty questionStats array
    // In a real implementation, this would analyze each question's responses
    return {
      totalResponses: responseCount,
      completionRate,
      averageCompletionTime,
      questionStats: []
    };
  }
}
