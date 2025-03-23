
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
  deliveryConfig?: DeliveryConfig;
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
  completionTime?: number;
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
  submittedAt?: string;
}

export interface DeliveryConfig {
  type: 'manual' | 'scheduled' | 'triggered';
  emailAddresses: string[];
  schedule?: {
    frequency: 'daily' | 'weekly' | 'monthly';
    dayOfMonth?: number;
    dayOfWeek?: number;
    time: string;
  };
  trigger?: {
    type: string;
    delayHours: number;
    sendAutomatically: boolean;
  };
}

export interface SurveyStatistics {
  totalResponses: number;
  averageCompletionTime: number;
  completionRate: number;
  questionStats: {
    questionId: string;
    questionTitle: string;
    responses: {
      answer: string;
      count: number;
      percentage: number;
    }[];
  }[];
}
