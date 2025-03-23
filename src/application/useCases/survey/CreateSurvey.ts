
import { Survey } from '../../../domain/models/Survey';
import { SurveyRepository } from '../../../domain/repositories/SurveyRepository';

export class CreateSurvey {
  constructor(private surveyRepository: SurveyRepository) {}

  async execute(surveyData: Omit<Survey, 'id' | 'createdAt'>): Promise<Survey> {
    return await this.surveyRepository.createSurvey(surveyData);
  }
}
