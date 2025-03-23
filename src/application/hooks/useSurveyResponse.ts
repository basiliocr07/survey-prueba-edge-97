
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { SubmitSurveyResponse } from '../useCases/surveyResponse/SubmitSurveyResponse';
import { SupabaseSurveyResponseRepository } from '../../infrastructure/repositories/SupabaseSurveyResponseRepository';
import { SurveyResponseSubmission } from '../../domain/models/Survey';

// Initialize repositories and use cases
const surveyResponseRepository = new SupabaseSurveyResponseRepository();
const submitSurveyResponseUseCase = new SubmitSurveyResponse(surveyResponseRepository);

export const useSurveyResponse = (surveyId?: string) => {
  const queryClient = useQueryClient();

  const { data: responses, isLoading, error } = useQuery({
    queryKey: ['surveyResponses', surveyId],
    queryFn: () => surveyResponseRepository.getResponsesBySurveyId(surveyId || ''),
    enabled: !!surveyId,
  });

  const submitResponseMutation = useMutation({
    mutationFn: (responseData: SurveyResponseSubmission) => submitSurveyResponseUseCase.execute(responseData),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['surveyResponses', surveyId] });
      queryClient.invalidateQueries({ queryKey: ['surveyStatistics', surveyId] });
    },
  });

  return {
    responses,
    isLoading,
    error,
    submitResponse: submitResponseMutation.mutate,
    isSubmitting: submitResponseMutation.isPending,
    submitError: submitResponseMutation.error,
  };
};
