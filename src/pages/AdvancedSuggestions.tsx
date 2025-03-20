
import { useState, useEffect } from 'react';
import Navbar from '@/components/layout/Navbar';
import Footer from '@/components/layout/Footer';
import AdvancedSuggestionsDashboard from '@/components/suggestions/AdvancedSuggestionsDashboard';
import { useToast } from "@/hooks/use-toast";
import { Suggestion } from '@/types/suggestions';

// Mock data for demonstration
const mockSuggestions: Suggestion[] = [
  {
    id: '1',
    title: 'Add dark mode to the dashboard',
    content: 'It would be great to have a dark mode option for the dashboard to reduce eye strain when working at night.',
    customerName: 'John Doe',
    customerEmail: 'john@example.com',
    createdAt: '2023-06-15T10:30:00Z',
    status: 'implemented',
    category: 'UI/UX',
    isAnonymous: false,
    response: 'Great suggestion! We have implemented dark mode and it will be available in the next release.',
    responseDate: '2023-06-20T14:15:00Z',
    completionPercentage: 100
  },
  {
    id: '2',
    title: 'Improve mobile responsiveness',
    content: 'The application could be more responsive on mobile devices, especially the survey form.',
    customerName: 'Jane Smith',
    customerEmail: 'jane@example.com',
    createdAt: '2023-06-17T15:45:00Z',
    status: 'reviewed',
    category: 'Mobile App',
    isAnonymous: false,
    response: 'We are currently working on improving mobile responsiveness. Thank you for your feedback!',
    responseDate: '2023-06-21T09:20:00Z',
    completionPercentage: 50
  }
];

export default function AdvancedSuggestions() {
  const [userRole, setUserRole] = useState('');
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [suggestions, setSuggestions] = useState<Suggestion[]>(mockSuggestions);
  const { toast } = useToast();
  
  useEffect(() => {
    const loggedIn = localStorage.getItem('isLoggedIn') === 'true';
    const role = localStorage.getItem('userRole') || '';
    
    setIsLoggedIn(loggedIn);
    setUserRole(role);
    
    // Fetch suggestions data from API in a real application
    const fetchSuggestions = async () => {
      try {
        // Mock data for now
        setSuggestions(mockSuggestions);
      } catch (error) {
        console.error('Error fetching suggestions:', error);
        toast({
          title: "Error",
          description: "Failed to load suggestions data",
          variant: "destructive"
        });
      }
    };
    
    fetchSuggestions();
  }, [toast]);
  
  // Calculate counts for admin dashboard
  const totalCount = suggestions.length;
  const newCount = suggestions.filter(s => s.status === 'new').length;
  const inProgressCount = suggestions.filter(s => s.status === 'reviewed').length;
  const completedCount = suggestions.filter(s => s.status === 'implemented').length;
  
  const isAdmin = isLoggedIn && userRole.toLowerCase() === 'admin';
  
  return (
    <div className="min-h-screen flex flex-col bg-background">
      <Navbar />
      
      <main className="flex-1 w-full px-4 sm:px-6 lg:px-8 pt-24 pb-20 flex justify-center">
        <div className="w-full max-w-[1000px]">
          <div className="mb-8">
            <h1 className="text-3xl font-bold tracking-tight sm:text-4xl mb-2">Advanced Suggestions</h1>
            <p className="text-muted-foreground max-w-2xl">
              {isAdmin 
                ? "Advanced management and analysis of customer suggestions and feedback."
                : "We value your feedback! Submit your ideas and track their progress."}
            </p>
          </div>
          
          <AdvancedSuggestionsDashboard 
            isAdmin={isAdmin}
            suggestions={suggestions}
            totalCount={totalCount}
            newCount={newCount}
            inProgressCount={inProgressCount}
            completedCount={completedCount}
          />
        </div>
      </main>
      
      <Footer />
    </div>
  );
}
