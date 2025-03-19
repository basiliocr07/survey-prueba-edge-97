
// Interfaces para encuestas y respuestas
export interface SurveyQuestion {
  id: string;
  title: string;
  description?: string;
  type: string;
  required: boolean;
  options?: string[];
  settings?: {
    min?: number;
    max?: number;
  };
}

export interface Survey {
  id: string;
  title: string;
  description?: string;
  questions: SurveyQuestion[];
  createdAt: string;
}

export interface QuestionResponse {
  questionId: string;
  questionTitle: string;
  questionType: string;
  value: string | string[];
  isValid: boolean;
}

export interface SurveyResponse {
  id?: string;
  surveyId: string;
  respondentName: string;
  respondentEmail: string;
  respondentPhone?: string;
  respondentCompany?: string;
  submittedAt: string;
  answers: QuestionResponse[];
  isExistingClient?: boolean;
  existingClientId?: string;
}

export interface SurveyResponseSubmission {
  surveyId: string;
  respondentName: string;
  respondentEmail: string;
  respondentPhone?: string;
  respondentCompany?: string;
  answers: Record<string, string | string[]>;
  isExistingClient?: boolean;
  existingClientId?: string;
  submittedAt?: string; // Added this property to fix the errors
}
