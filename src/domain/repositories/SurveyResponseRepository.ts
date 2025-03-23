
import { SurveyResponse, SurveyResponseSubmission } from '../models/Survey';

export interface SurveyResponseRepository {
  getResponsesBySurveyId(surveyId: string): Promise<SurveyResponse[]>;
  getResponseById(id: string): Promise<SurveyResponse | null>;
  submitResponse(response: SurveyResponseSubmission): Promise<SurveyResponse>;
  deleteResponse(id: string): Promise<boolean>;
}
