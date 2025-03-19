export interface Survey {
  id: string;
  title: string;
  description: string;
  questions: Question[];
  createdAt: string;
  responses: number;
  completionRate: number;
}

export type QuestionType = 'multiple-choice' | 'single-choice' | 'text' | 'rating' | 'dropdown' | 'matrix' | 'ranking' | 'nps' | 'date' | 'file-upload';

export interface Question {
  id: string;
  type: QuestionType;
  title: string;
  description?: string;
  required: boolean;
  options?: string[];
  settings?: any;
}

export interface Response {
  id: string;
  surveyId: string;
  answers: Answer[];
  submittedAt: string;
  completionTime: number; // in seconds
  respondentName: string;  // Added property
  respondentEmail: string; // Added property
  respondentPhone?: string; // Added property
  respondentCompany?: string; // Added property
}

export interface Answer {
  questionId: string;
  value: string | string[] | number;
}

export const sampleSurveys: Survey[] = [
  {
    id: "1",
    title: "Customer Satisfaction Survey",
    description: "Help us improve our products and services by sharing your experience.",
    questions: [
      {
        id: "q1",
        type: "single-choice",
        title: "How satisfied are you with our product?",
        required: true,
        options: ["Very satisfied", "Satisfied", "Neutral", "Unsatisfied", "Very unsatisfied"]
      },
      {
        id: "q2",
        type: "text",
        title: "What do you like most about our product?",
        required: false
      },
      {
        id: "q3",
        type: "rating",
        title: "How likely are you to recommend our product to others?",
        required: true,
        settings: {
          min: 0,
          max: 10,
          step: 1
        }
      }
    ],
    createdAt: "2023-06-15T10:30:00Z",
    responses: 243,
    completionRate: 78
  },
  {
    id: "2",
    title: "Product Feedback Survey",
    description: "We value your feedback on our latest product release.",
    questions: [
      {
        id: "q1",
        type: "multiple-choice",
        title: "Which features do you use most frequently?",
        required: true,
        options: ["Dashboard", "Reports", "Analytics", "User management", "Settings", "API"]
      },
      {
        id: "q2",
        type: "text",
        title: "What features would you like to see in future updates?",
        required: false
      },
      {
        id: "q3",
        type: "nps",
        title: "How likely are you to recommend our product to a colleague?",
        required: true
      }
    ],
    createdAt: "2023-08-22T14:15:00Z",
    responses: 186,
    completionRate: 64
  },
  {
    id: "3",
    title: "Website Usability Survey",
    description: "Help us understand how we can improve your experience on our website.",
    questions: [
      {
        id: "q1",
        type: "single-choice",
        title: "How easy was it to find what you were looking for?",
        required: true,
        options: ["Very easy", "Easy", "Neutral", "Difficult", "Very difficult"]
      },
      {
        id: "q2",
        type: "rating",
        title: "Rate the overall design of our website",
        required: true,
        settings: {
          min: 1,
          max: 5,
          step: 1
        }
      },
      {
        id: "q3",
        type: "text",
        title: "What would make your experience on our website better?",
        required: false
      }
    ],
    createdAt: "2023-09-10T09:45:00Z",
    responses: 152,
    completionRate: 82
  }
];

export const sampleResponses: Response[] = [
  {
    id: "r1",
    surveyId: "1",
    answers: [
      { questionId: "q1", value: "Satisfied" },
      { questionId: "q2", value: "The intuitive interface and quick loading times." },
      { questionId: "q3", value: 8 }
    ],
    submittedAt: "2023-06-16T11:20:00Z",
    completionTime: 156,
    respondentName: "Juan Pérez",
    respondentEmail: "juan.perez@example.com",
    respondentPhone: "+34 612 345 678",
    respondentCompany: "Técnicas Avanzadas S.L."
  },
  {
    id: "r2",
    surveyId: "1",
    answers: [
      { questionId: "q1", value: "Very satisfied" },
      { questionId: "q2", value: "Customer support and product reliability." },
      { questionId: "q3", value: 9 }
    ],
    submittedAt: "2023-06-17T15:45:00Z",
    completionTime: 132,
    respondentName: "María García",
    respondentEmail: "maria.garcia@example.com",
    respondentPhone: "+34 623 456 789",
    respondentCompany: "Innovaciones Digitales"
  },
  {
    id: "r3",
    surveyId: "1",
    answers: [
      { questionId: "q1", value: "Neutral" },
      { questionId: "q2", value: "The range of features available." },
      { questionId: "q3", value: 6 }
    ],
    submittedAt: "2023-06-18T09:30:00Z",
    completionTime: 178,
    respondentName: "Carlos López",
    respondentEmail: "carlos.lopez@example.com",
    respondentPhone: "+34 634 567 890",
    respondentCompany: "Soluciones Empresariales"
  },
  {
    id: "r4",
    surveyId: "2",
    answers: [
      { questionId: "q1", value: ["Dashboard", "Analytics", "API"] },
      { questionId: "q2", value: "Better integration with third-party tools." },
      { questionId: "q3", value: 7 }
    ],
    submittedAt: "2023-08-23T10:15:00Z",
    completionTime: 245,
    respondentName: "Ana Martínez",
    respondentEmail: "ana.martinez@example.com",
    respondentPhone: "+34 645 678 901",
    respondentCompany: "Tech Solutions"
  },
  {
    id: "r5",
    surveyId: "2",
    answers: [
      { questionId: "q1", value: ["Reports", "User management"] },
      { questionId: "q2", value: "Mobile app and offline capabilities." },
      { questionId: "q3", value: 8 }
    ],
    submittedAt: "2023-08-24T16:40:00Z",
    completionTime: 198,
    respondentName: "Javier Rodríguez",
    respondentEmail: "javier.rodriguez@example.com",
    respondentPhone: "+34 656 789 012",
    respondentCompany: "Consultores Asociados"
  }
];

export const getQuestionTypeInfo = (type: QuestionType) => {
  const types = {
    'multiple-choice': {
      name: 'Multiple Choice',
      description: 'Allow respondents to select multiple options'
    },
    'single-choice': {
      name: 'Single Choice',
      description: 'Allow respondents to select only one option'
    },
    'text': {
      name: 'Text Input',
      description: 'Collect open-ended responses'
    },
    'rating': {
      name: 'Rating Scale',
      description: 'Collect feedback on a numeric scale'
    },
    'dropdown': {
      name: 'Dropdown',
      description: 'Allow selection from a dropdown menu'
    },
    'matrix': {
      name: 'Matrix / Likert Scale',
      description: 'Collect responses across multiple criteria'
    },
    'ranking': {
      name: 'Ranking',
      description: 'Allow respondents to rank options in order'
    },
    'nps': {
      name: 'Net Promoter Score',
      description: 'Measure customer satisfaction on a 0-10 scale'
    },
    'date': {
      name: 'Date',
      description: 'Collect date information'
    },
    'file-upload': {
      name: 'File Upload',
      description: 'Allow respondents to upload files'
    }
  };
  
  return types[type];
};
