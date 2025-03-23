
import { Survey, SurveyStatistics } from '../../domain/models/Survey';
import { SurveyRepository } from '../../domain/repositories/SurveyRepository';
import { supabase } from '../../integrations/supabase/client';
import { Json } from '../../integrations/supabase/types';

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
    // Fetch survey responses
    const { data: responses, error } = await supabase
      .from('survey_responses')
      .select('*')
      .eq('survey_id', surveyId);

    if (error) throw error;

    // Get survey to access questions
    const survey = await this.getSurveyById(surveyId);
    if (!survey) throw new Error('Survey not found');

    // Calculate statistics
    const totalResponses = responses.length;
    
    // Safely handle completion time calculation
    let averageCompletionTime = 0;
    const completionTimes: number[] = [];
    
    responses.forEach(response => {
      const completionTime = (response as any).completion_time;
      if (typeof completionTime === 'number') {
        completionTimes.push(completionTime);
      }
    });
    
    if (completionTimes.length > 0) {
      averageCompletionTime = completionTimes.reduce((sum, time) => sum + time, 0) / completionTimes.length;
    }
    
    // Create question statistics
    const questionStats = survey.questions.map(question => {
      const questionResponses: Record<string, number> = {};
      
      responses.forEach(response => {
        if (response.answers && typeof response.answers === 'object') {
          const answers = response.answers as Record<string, any>;
          const answer = answers[question.id];
          if (answer) {
            const answerKey = Array.isArray(answer) ? answer.join(', ') : answer.toString();
            questionResponses[answerKey] = (questionResponses[answerKey] || 0) + 1;
          }
        }
      });
      
      return {
        questionId: question.id,
        questionTitle: question.title,
        responses: Object.entries(questionResponses).map(([answer, count]) => ({
          answer,
          count,
          percentage: totalResponses ? (count / totalResponses) * 100 : 0
        }))
      };
    });

    return {
      totalResponses,
      averageCompletionTime,
      completionRate: responses.length ? (responses.filter(r => r.answers && Object.keys(r.answers).length > 0).length / responses.length) * 100 : 0,
      questionStats
    };
  }

  async sendSurveyEmails(surveyId: string, emailAddresses: string[]): Promise<boolean> {
    try {
      // In a real implementation, this would connect to an email service
      // For demo purposes, we'll simulate success
      console.log(`Sending survey ${surveyId} to emails: ${emailAddresses.join(', ')}`);
      return true;
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

    return {
      id: data.id,
      title: data.title,
      description: data.description || undefined,
      questions: parsedQuestions,
      createdAt: data.created_at
    };
  }
}
