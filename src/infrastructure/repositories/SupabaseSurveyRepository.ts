
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
    // Convert SurveyQuestion[] to Json for Supabase
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
    // Fetch survey responses with explicit response type
    const { data, error } = await supabase
      .from('survey_responses')
      .select('*')
      .eq('survey_id', surveyId);

    if (error) throw error;

    // Define a simple array type for responses
    const responses: any[] = data || [];

    // Get survey to access questions
    const survey = await this.getSurveyById(surveyId);
    if (!survey) throw new Error('Survey not found');

    // Calculate statistics
    const totalResponses = responses.length;
    
    // Calculate average completion time
    let averageCompletionTime = 0;
    let completionTimeSum = 0;
    let completionTimeCount = 0;
    
    for (const response of responses) {
      if (response && 
          typeof response === 'object' && 
          'completion_time' in response && 
          typeof response.completion_time === 'number') {
        completionTimeSum += response.completion_time;
        completionTimeCount++;
      }
    }
    
    if (completionTimeCount > 0) {
      averageCompletionTime = completionTimeSum / completionTimeCount;
    }
    
    // Process question statistics with explicit typing
    const questionStats = survey.questions.map(question => {
      // Simple record to count answers
      const answerCounts: Record<string, number> = {};
      
      for (const response of responses) {
        // Skip invalid responses
        if (!response || typeof response !== 'object') continue;
        
        // Get answers with type assertion
        const answers = response.answers as Record<string, unknown> | null | undefined;
        if (!answers) continue;
        
        // Get answer for this question
        const answer = answers[question.id];
        if (answer === undefined || answer === null) continue;
        
        // Convert answer to string for counting
        let answerKey: string;
        if (Array.isArray(answer)) {
          answerKey = answer.join(', ');
        } else {
          answerKey = String(answer);
        }
        
        // Count the answer
        answerCounts[answerKey] = (answerCounts[answerKey] || 0) + 1;
      }
      
      // Convert to required format
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

    // Calculate completion rate with explicit type handling
    let completedResponsesCount = 0;
    
    for (const response of responses) {
      if (response && 
          typeof response === 'object' && 
          response.answers && 
          typeof response.answers === 'object' &&
          Object.keys(response.answers).length > 0) {
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
