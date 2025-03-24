
import { SurveyRepository } from "@/domain/repositories/SurveyRepository";
import { Survey } from "@/domain/models/Survey";
import { supabase } from "@/integrations/supabase/client";
import { Surveys } from "@/integrations/supabase/types";

export class SupabaseSurveyRepository implements SurveyRepository {
  async createSurvey(survey: Survey): Promise<Survey> {
    const { data, error } = await supabase
      .from('surveys')
      .insert([
        {
          title: survey.title,
          description: survey.description,
          questions: survey.questions,
          settings: survey.settings,
          created_by: survey.createdBy,
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

  async updateSurvey(survey: Survey): Promise<Survey> {
    const { data, error } = await supabase
      .from('surveys')
      .update({
        title: survey.title,
        description: survey.description,
        questions: survey.questions,
        settings: survey.settings,
        updated_at: new Date().toISOString()
      })
      .eq('id', survey.id)
      .select()
      .single();

    if (error) {
      console.error('Error updating survey:', error);
      throw new Error('Failed to update survey');
    }

    return this.mapDbSurveyToModel(data);
  }

  async deleteSurvey(id: string): Promise<void> {
    const { error } = await supabase
      .from('surveys')
      .delete()
      .eq('id', id);

    if (error) {
      console.error('Error deleting survey:', error);
      throw new Error('Failed to delete survey');
    }
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

  private mapDbSurveyToModel(dbSurvey: Surveys): Survey {
    return {
      id: dbSurvey.id,
      title: dbSurvey.title,
      description: dbSurvey.description || '',
      questions: dbSurvey.questions || [],
      settings: dbSurvey.settings || {},
      createdAt: new Date(dbSurvey.created_at),
      updatedAt: dbSurvey.updated_at ? new Date(dbSurvey.updated_at) : undefined,
      createdBy: dbSurvey.created_by || '',
      status: dbSurvey.status || 'draft',
    };
  }

  async getSurveyStatistics(surveyId: string): Promise<{
    responseCount: number;
    completionRate: number;
    averageCompletionTime: number;
  }> {
    // Get all responses for the survey
    const { data: responses, error } = await supabase
      .from('survey_responses')
      .select('*')
      .eq('survey_id', surveyId);

    if (error) {
      console.error('Error fetching survey responses:', error);
      return {
        responseCount: 0,
        completionRate: 0,
        averageCompletionTime: 0
      };
    }

    // Calculate statistics
    const responseCount = responses.length;
    
    // Count responses with completion times
    let totalCompletionTime = 0;
    let responsesWithCompletionTime = 0;
    
    for (const response of responses) {
      // Fixed the property access method to ensure type safety
      if (typeof response.completion_time === 'number' || 
          typeof (response as any).completion_time === 'number') {
        const completionTime = typeof response.completion_time === 'number' ? 
                              response.completion_time : 
                              (response as any).completion_time;
        
        totalCompletionTime += completionTime;
        responsesWithCompletionTime++;
      }
    }

    // Calculate averages
    const completionRate = responseCount > 0 ? 
      (responsesWithCompletionTime / responseCount) * 100 : 0;
    
    const averageCompletionTime = responsesWithCompletionTime > 0 ? 
      totalCompletionTime / responsesWithCompletionTime : 0;

    return {
      responseCount,
      completionRate,
      averageCompletionTime
    };
  }
}
