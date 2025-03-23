
import { Survey } from '../../../domain/models/Survey';
import { SurveyRepository } from '../../../domain/repositories/SurveyRepository';

export class CreateSurvey {
  constructor(private surveyRepository: SurveyRepository) {}

  async execute(surveyData: Omit<Survey, 'id' | 'createdAt'>): Promise<Survey> {
    try {
      const result = await this.surveyRepository.createSurvey(surveyData);
      return result;
    } catch (error) {
      console.error('Error creating survey:', error);
      throw error;
    }
  }

  async sendEmails(surveyId: string, emailAddresses: string[]): Promise<boolean> {
    try {
      return await this.surveyRepository.sendSurveyEmails(surveyId, emailAddresses);
    } catch (error) {
      console.error('Error sending survey emails:', error);
      throw error;
    }
  }
}
