
import { Survey } from '../../../domain/models/Survey';
import { SurveyRepository } from '../../../domain/repositories/SurveyRepository';

export class GetSurveyById {
  constructor(private surveyRepository: SurveyRepository) {}

  async execute(id: string): Promise<Survey | null> {
    return await this.surveyRepository.getSurveyById(id);
  }
}
