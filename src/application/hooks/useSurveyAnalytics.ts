
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { SupabaseSurveyRepository } from '../../infrastructure/repositories/SupabaseSurveyRepository';
import { SupabaseSurveyResponseRepository } from '../../infrastructure/repositories/SupabaseSurveyResponseRepository';
import { DeliveryConfig, Survey } from '../../domain/models/Survey';
import { useToast } from '../../hooks/use-toast';

// Initialize repositories
const surveyRepository = new SupabaseSurveyRepository();
const surveyResponseRepository = new SupabaseSurveyResponseRepository();

export function useSurveyAnalytics() {
  const queryClient = useQueryClient();
  const { toast } = useToast();

  // Fetch all surveys
  const { 
    data: surveys, 
    isLoading: isLoadingSurveys,
    error: surveysError
  } = useQuery({
    queryKey: ['surveys'],
    queryFn: () => surveyRepository.getAllSurveys(),
  });

  // Function to fetch statistics for a specific survey
  const fetchSurveyStatistics = async (surveyId: string) => {
    try {
      return await surveyRepository.getSurveyStatistics(surveyId);
    } catch (error) {
      console.error('Error fetching survey statistics:', error);
      throw error;
    }
  };

  // Function to fetch responses for a specific survey
  const fetchSurveyResponses = async (surveyId: string) => {
    try {
      return await surveyResponseRepository.getResponsesBySurveyId(surveyId);
    } catch (error) {
      console.error('Error fetching survey responses:', error);
      throw error;
    }
  };

  // Mutation for sending survey emails
  const sendEmailsMutation = useMutation({
    mutationFn: ({ surveyId, emailAddresses }: { surveyId: string, emailAddresses: string[] }) => {
      return surveyRepository.sendSurveyEmails(surveyId, emailAddresses);
    },
    onSuccess: (success) => {
      if (success) {
        toast({
          title: "Success",
          description: "Survey emails have been sent successfully",
        });
      } else {
        toast({
          title: "Error",
          description: "Failed to send survey emails",
          variant: "destructive",
        });
      }
    },
    onError: (error) => {
      toast({
        title: "Error",
        description: `Failed to send survey emails: ${error instanceof Error ? error.message : 'Unknown error'}`,
        variant: "destructive",
      });
    }
  });

  // Mutation for updating survey delivery config
  const updateDeliveryConfigMutation = useMutation({
    mutationFn: ({ surveyId, deliveryConfig }: { surveyId: string, deliveryConfig: DeliveryConfig }) => {
      return surveyRepository.updateSurvey({
        id: surveyId,
        title: '',  // These will be populated from the existing survey in the repository
        questions: [],
        createdAt: '',
        deliveryConfig
      });
    },
    onSuccess: (success) => {
      if (success) {
        toast({
          title: "Success",
          description: "Survey delivery configuration has been updated",
        });
        queryClient.invalidateQueries({ queryKey: ['surveys'] });
      } else {
        toast({
          title: "Error",
          description: "Failed to update survey delivery configuration",
          variant: "destructive",
        });
      }
    },
    onError: (error) => {
      toast({
        title: "Error",
        description: `Failed to update delivery config: ${error instanceof Error ? error.message : 'Unknown error'}`,
        variant: "destructive",
      });
    }
  });

  return {
    surveys,
    isLoadingSurveys,
    surveysError,
    fetchSurveyStatistics,
    fetchSurveyResponses,
    sendSurveyEmails: (surveyId: string, emailAddresses: string[]) => 
      sendEmailsMutation.mutateAsync({ surveyId, emailAddresses }),
    isSendingEmails: sendEmailsMutation.isPending,
    updateDeliveryConfig: (surveyId: string, deliveryConfig: DeliveryConfig) =>
      updateDeliveryConfigMutation.mutateAsync({ surveyId, deliveryConfig }),
    isUpdatingDeliveryConfig: updateDeliveryConfigMutation.isPending
  };
}
