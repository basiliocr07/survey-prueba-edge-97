
import { SurveyStatistics } from '../../../domain/models/Survey';
import { SurveyRepository } from '../../../domain/repositories/SurveyRepository';

export class GetSurveyStatistics {
  constructor(private surveyRepository: SurveyRepository) {}

  async execute(surveyId: string): Promise<SurveyStatistics> {
    return await this.surveyRepository.getSurveyStatistics(surveyId);
  }
}
