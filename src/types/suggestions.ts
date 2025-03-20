export interface Suggestion {
  id: string;
  title?: string; // Adding title as an optional property
  content: string;
  customerName: string;
  customerEmail: string;
  createdAt: string;
  status: 'new' | 'reviewed' | 'implemented' | 'rejected';
  category?: string;
  priority?: 'low' | 'medium' | 'high';
  isAnonymous?: boolean;
  response?: string;
  responseDate?: string;
  similarSuggestions?: string[];
  completionPercentage?: number; // Added this property to match usage
}

export interface SuggestionFormData {
  content: string;
  customerName: string;
  customerEmail: string;
  category?: string;
  isAnonymous: boolean;
}

export interface MonthlyReport {
  month: string;
  year: number;
  totalSuggestions: number;
  implementedSuggestions: number;
  topCategories: {
    category: string;
    count: number;
  }[];
  suggestions: Suggestion[];
}

export interface Customer {
  id: string;
  brandName: string;
  contactEmail: string;
  contactPhone: string;
  acquiredServices: string[];
  createdAt: string;
  growthMetrics?: {
    period: string;
    revenue: number;
    userCount: number;
  }[];
}

export interface KnowledgeBaseItem {
  id: string;
  title: string;
  content: string;
  category: string;
  tags: string[];
  createdAt: string;
  updatedAt: string;
}
