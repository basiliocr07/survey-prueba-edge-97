
export interface Requirement {
  id: string;
  title: string;
  content: string;
  customerName: string;
  customerEmail: string;
  createdAt: string;
  status: 'proposed' | 'in-progress' | 'implemented' | 'rejected';
  category?: string;
  priority?: 'low' | 'medium' | 'high' | 'critical';
  isAnonymous?: boolean;
  response?: string;
  responseDate?: string;
  completionPercentage?: number;
  projectArea?: string;
  acceptanceCriteria?: string;
  targetDate?: string;
}

export interface RequirementFormData {
  title: string;
  content: string;
  customerName: string;
  customerEmail: string;
  category?: string;
  priority?: string;
  isAnonymous: boolean;
  projectArea?: string;
  acceptanceCriteria?: string;
  targetDate?: string;
}

export interface RequirementAnalytics {
  month: string;
  year: number;
  totalRequirements: number;
  implementedRequirements: number;
  topCategories: {
    category: string;
    count: number;
  }[];
  requirements: Requirement[];
}

export interface RequirementStatusUpdateDto {
  status: string;
  response?: string;
  completionPercentage?: number;
}
