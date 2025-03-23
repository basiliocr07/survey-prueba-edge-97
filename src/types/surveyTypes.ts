
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
  completionTime?: number; // Propiedad para tiempo de completado
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
  submittedAt?: string; // Fecha de envío
}

export interface DeliveryConfig {
  type: 'manual' | 'scheduled' | 'triggered';
  emailAddresses: string[];
  schedule?: {
    frequency: 'daily' | 'weekly' | 'monthly';
    dayOfMonth?: number;
    dayOfWeek?: number;
    time?: string;
    startDate?: Date;
  };
  trigger?: {
    type: 'ticket-closed' | 'purchase-completed';
    delayHours: number;
    sendAutomatically: boolean;
  };
}

// Define QuestionType to match the one in utils/sampleData.ts
export type QuestionType = 'multiple-choice' | 'single-choice' | 'text' | 'rating' | 'dropdown' | 'matrix' | 'ranking' | 'nps' | 'date' | 'file-upload';

