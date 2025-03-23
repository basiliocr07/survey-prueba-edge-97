
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
    const { data, error } = await supabase
      .from('survey_responses')
      .select('*')
      .eq('survey_id', surveyId);

    if (error) throw error;

    // Use a simple type annotation to avoid deep type inference
    const responses = (data || []) as Record<string, any>[];

    const survey = await this.getSurveyById(surveyId);
    if (!survey) throw new Error('Survey not found');

    const totalResponses = responses.length;
    
    // Calculate average completion time with simple type handling
    let averageCompletionTime = 0;
    let completionTimeSum = 0;
    let completionTimeCount = 0;
    
    for (let i = 0; i < responses.length; i++) {
      const response = responses[i];
      if (response && typeof response.completion_time === 'number') {
        completionTimeSum += response.completion_time;
        completionTimeCount++;
      }
    }
    
    if (completionTimeCount > 0) {
      averageCompletionTime = completionTimeSum / completionTimeCount;
    }
    
    // Process question statistics with simple type handling
    const questionStats = survey.questions.map(question => {
      const answerCounts: Record<string, number> = {};
      
      for (let i = 0; i < responses.length; i++) {
        const response = responses[i];
        if (!response) continue;
        
        const answers = response.answers as Record<string, any>;
        if (!answers) continue;
        
        const answer = answers[question.id];
        if (answer === undefined || answer === null) continue;
        
        // Convert answer to string for counting
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

    // Calculate completion rate with simple type handling
    let completedResponsesCount = 0;
    
    for (let i = 0; i < responses.length; i++) {
      const response = responses[i];
      const answers = response?.answers as Record<string, any> | undefined;
      if (answers && Object.keys(answers).length > 0) {
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
