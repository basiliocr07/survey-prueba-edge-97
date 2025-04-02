
import { Survey, SurveyStatistics, DeliveryConfig } from '../models/Survey';

export interface SurveyRepository {
  getAllSurveys(): Promise<Survey[]>;
  getSurveyById(id: string): Promise<Survey | null>;
  createSurvey(survey: Omit<Survey, 'id' | 'createdAt'>): Promise<Survey>;
  updateSurvey(survey: Survey): Promise<boolean>;
  deleteSurvey(id: string): Promise<boolean>;
  getSurveysByStatus(status: string): Promise<Survey[]>;
  getSurveyStatistics(surveyId: string): Promise<SurveyStatistics>;
  sendSurveyEmails(surveyId: string, emailAddresses: string[]): Promise<boolean>;
  scheduleEmailDelivery(surveyId: string, config: DeliveryConfig): Promise<boolean>;
}
