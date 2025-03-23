
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { GetSurveyById } from '../useCases/survey/GetSurveyById';
import { CreateSurvey } from '../useCases/survey/CreateSurvey';
import { GetSurveyStatistics } from '../useCases/survey/GetSurveyStatistics';
import { SupabaseSurveyRepository } from '../../infrastructure/repositories/SupabaseSurveyRepository';
import { Survey } from '../../domain/models/Survey';

// Initialize repositories and use cases
const surveyRepository = new SupabaseSurveyRepository();
const getSurveyByIdUseCase = new GetSurveyById(surveyRepository);
const createSurveyUseCase = new CreateSurvey(surveyRepository);
const getSurveyStatisticsUseCase = new GetSurveyStatistics(surveyRepository);

export const useSurvey = (surveyId?: string) => {
  const queryClient = useQueryClient();

  const { data: survey, isLoading, error } = useQuery({
    queryKey: ['survey', surveyId],
    queryFn: () => getSurveyByIdUseCase.execute(surveyId || ''),
    enabled: !!surveyId,
  });

  const createSurveyMutation = useMutation({
    mutationFn: (surveyData: Omit<Survey, 'id' | 'createdAt'>) => createSurveyUseCase.execute(surveyData),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['surveys'] });
    },
  });

  const { data: statistics, isLoading: isLoadingStats } = useQuery({
    queryKey: ['surveyStatistics', surveyId],
    queryFn: () => getSurveyStatisticsUseCase.execute(surveyId || ''),
    enabled: !!surveyId,
  });

  return {
    survey,
    isLoading,
    error,
    createSurvey: createSurveyMutation.mutate,
    isCreating: createSurveyMutation.isPending,
    createError: createSurveyMutation.error,
    statistics,
    isLoadingStats,
  };
};
