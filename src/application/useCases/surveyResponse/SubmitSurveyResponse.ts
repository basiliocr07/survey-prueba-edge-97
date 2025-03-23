
import { SurveyResponse, SurveyResponseSubmission } from '../../../domain/models/Survey';
import { SurveyResponseRepository } from '../../../domain/repositories/SurveyResponseRepository';

export class SubmitSurveyResponse {
  constructor(private surveyResponseRepository: SurveyResponseRepository) {}

  async execute(responseData: SurveyResponseSubmission): Promise<SurveyResponse> {
    return await this.surveyResponseRepository.submitResponse(responseData);
  }
}
