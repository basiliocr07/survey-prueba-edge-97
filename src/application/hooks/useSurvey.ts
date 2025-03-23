
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { GetSurveyById } from '../useCases/survey/GetSurveyById';
import { CreateSurvey } from '../useCases/survey/CreateSurvey';
import { GetSurveyStatistics } from '../useCases/survey/GetSurveyStatistics';
import { SupabaseSurveyRepository } from '../../infrastructure/repositories/SupabaseSurveyRepository';
import { Survey } from '../../domain/models/Survey';
import { useToast } from '@/hooks/use-toast';

// Initialize repositories and use cases
const surveyRepository = new SupabaseSurveyRepository();
const getSurveyByIdUseCase = new GetSurveyById(surveyRepository);
const createSurveyUseCase = new CreateSurvey(surveyRepository);
const getSurveyStatisticsUseCase = new GetSurveyStatistics(surveyRepository);

export const useSurvey = (surveyId?: string) => {
  const queryClient = useQueryClient();
  const { toast } = useToast();

  const { data: survey, isLoading, error } = useQuery({
    queryKey: ['survey', surveyId],
    queryFn: () => getSurveyByIdUseCase.execute(surveyId || ''),
    enabled: !!surveyId,
  });

  const createSurveyMutation = useMutation({
    mutationFn: (surveyData: Omit<Survey, 'id' | 'createdAt'>) => {
      return createSurveyUseCase.execute(surveyData);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['surveys'] });
    },
  });

  const updateSurveyMutation = useMutation({
    mutationFn: (surveyData: Survey) => {
      return surveyRepository.updateSurvey(surveyData);
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['surveys'] });
      queryClient.invalidateQueries({ queryKey: ['survey', surveyId] });
      toast({
        title: "Éxito",
        description: "Encuesta actualizada correctamente",
      });
    },
    onError: (error) => {
      toast({
        title: "Error",
        description: `Error al actualizar la encuesta: ${error instanceof Error ? error.message : 'Error desconocido'}`,
        variant: "destructive",
      });
    },
  });

  const sendEmailsMutation = useMutation({
    mutationFn: ({ id, emails }: { id: string; emails: string[] }) => {
      return createSurveyUseCase.sendEmails(id, emails);
    },
    onSuccess: () => {
      toast({
        title: "Emails enviados",
        description: "Los emails han sido enviados correctamente",
      });
    },
    onError: (error) => {
      toast({
        title: "Error",
        description: `Error al enviar emails: ${error instanceof Error ? error.message : 'Error desconocido'}`,
        variant: "destructive",
      });
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
    createSurvey: (surveyData: Omit<Survey, 'id' | 'createdAt'>) => {
      return createSurveyMutation.mutateAsync(surveyData);
    },
    updateSurvey: (surveyData: Survey) => {
      return updateSurveyMutation.mutateAsync(surveyData);
    },
    isUpdating: updateSurveyMutation.isPending,
    isCreating: createSurveyMutation.isPending,
    createError: createSurveyMutation.error,
    sendSurveyEmails: (id: string, emails: string[]) => {
      return sendEmailsMutation.mutateAsync({ id, emails });
    },
    isSendingEmails: sendEmailsMutation.isPending,
    statistics,
    isLoadingStats,
  };
};
