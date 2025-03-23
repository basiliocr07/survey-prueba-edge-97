
import { SurveyResponse, SurveyResponseSubmission } from '../../domain/models/Survey';
import { SurveyResponseRepository } from '../../domain/repositories/SurveyResponseRepository';
import { supabase } from '../../integrations/supabase/client';

export class SupabaseSurveyResponseRepository implements SurveyResponseRepository {
  async getResponsesBySurveyId(surveyId: string): Promise<SurveyResponse[]> {
    const { data, error } = await supabase
      .from('survey_responses')
      .select('*')
      .eq('survey_id', surveyId)
      .order('submitted_at', { ascending: false });

    if (error) throw error;
    
    return (data || []).map(this.mapToSurveyResponse);
  }

  async getResponseById(id: string): Promise<SurveyResponse | null> {
    const { data, error } = await supabase
      .from('survey_responses')
      .select('*')
      .eq('id', id)
      .single();

    if (error) return null;
    if (!data) return null;
    
    return this.mapToSurveyResponse(data);
  }

  async submitResponse(response: SurveyResponseSubmission): Promise<SurveyResponse> {
    const { data, error } = await supabase
      .from('survey_responses')
      .insert([{
        survey_id: response.surveyId,
        respondent_name: response.respondentName,
        respondent_email: response.respondentEmail,
        respondent_phone: response.respondentPhone,
        respondent_company: response.respondentCompany,
        answers: response.answers,
        submitted_at: response.submittedAt || new Date().toISOString()
      }])
      .select();

    if (error) throw error;
    
    return this.mapToSurveyResponse(data[0]);
  }

  async deleteResponse(id: string): Promise<boolean> {
    const { error } = await supabase
      .from('survey_responses')
      .delete()
      .eq('id', id);

    return !error;
  }

  private mapToSurveyResponse(data: any): SurveyResponse {
    const answers = Object.entries(data.answers || {}).map(([questionId, value]) => ({
      questionId,
      questionTitle: questionId, // In a real app, fetch the title from the survey
      questionType: Array.isArray(value) ? 'multiple-choice' : 'single-choice',
      value: value as string | string[],
      isValid: true
    }));

    return {
      id: data.id,
      surveyId: data.survey_id,
      respondentName: data.respondent_name,
      respondentEmail: data.respondent_email,
      respondentPhone: data.respondent_phone,
      respondentCompany: data.respondent_company,
      submittedAt: data.submitted_at,
      answers,
      completionTime: data.completion_time
    };
  }
}
