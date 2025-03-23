
import { useQuery } from '@tanstack/react-query';
import { SupabaseSurveyRepository } from '../../infrastructure/repositories/SupabaseSurveyRepository';
import { SupabaseSurveyResponseRepository } from '../../infrastructure/repositories/SupabaseSurveyResponseRepository';

// Initialize repositories
const surveyRepository = new SupabaseSurveyRepository();
const surveyResponseRepository = new SupabaseSurveyResponseRepository();

export function useSurveyAnalytics() {
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

  return {
    surveys,
    isLoadingSurveys,
    surveysError,
    fetchSurveyStatistics,
    fetchSurveyResponses
  };
}
