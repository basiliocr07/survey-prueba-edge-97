
import { useState, useEffect } from 'react';
import Navbar from '@/components/layout/Navbar';
import Footer from '@/components/layout/Footer';
import SuggestionsDashboard from '@/components/suggestions/SuggestionsDashboard';
import { useToast } from "@/hooks/use-toast";

// Datos mockeados para demostración
const mockSuggestions = [
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
    response: 'Great suggestion! We've implemented dark mode and it will be available in the next release.',
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
    response: 'We're currently working on improving mobile responsiveness. Thank you for your feedback!',
    responseDate: '2023-06-21T09:20:00Z',
    completionPercentage: 50
  },
  {
    id: '3',
    title: 'Add export to PDF option',
    content: 'Please add an option to export survey results as PDF files for easier sharing.',
    customerName: 'Anonymous',
    customerEmail: 'anonymous@example.com',
    createdAt: '2023-06-19T12:10:00Z',
    status: 'new',
    category: 'Features',
    isAnonymous: true,
    completionPercentage: 0
  },
  {
    id: '4',
    title: 'Fix login issues on Firefox',
    content: 'There are some login issues when using the Firefox browser. The login button sometimes doesn't respond.',
    customerName: 'Carlos Rodriguez',
    customerEmail: 'carlos@example.com',
    createdAt: '2023-06-20T08:30:00Z',
    status: 'rejected',
    category: 'Bug',
    isAnonymous: false,
    response: 'We were unable to reproduce this issue after testing on multiple Firefox versions. Please provide more details if possible.',
    responseDate: '2023-06-22T11:05:00Z',
    completionPercentage: 100
  },
  {
    id: '5',
    title: 'Add integration with Google Forms',
    content: 'It would be helpful to have an integration with Google Forms to import existing surveys.',
    customerName: 'Sarah Johnson',
    customerEmail: 'sarah@example.com',
    createdAt: '2023-06-21T16:20:00Z',
    status: 'new',
    category: 'Integrations',
    isAnonymous: false,
    completionPercentage: 0
  }
];

export default function Suggestions() {
  const [userRole, setUserRole] = useState('');
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [suggestions, setSuggestions] = useState(mockSuggestions);
  const { toast } = useToast();
  
  useEffect(() => {
    const loggedIn = localStorage.getItem('isLoggedIn') === 'true';
    const role = localStorage.getItem('userRole') || '';
    
    setIsLoggedIn(loggedIn);
    setUserRole(role);
    
    // Fetch suggestions data from API in a real application
    // This would replace the mock data
    const fetchSuggestions = async () => {
      try {
        // In a real app, this would be an API call:
        // const response = await fetch('/api/suggestions');
        // const data = await response.json();
        // setSuggestions(data);
        
        // Using mock data for now
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
            <h1 className="text-3xl font-bold tracking-tight sm:text-4xl mb-2">Customer Suggestions</h1>
            <p className="text-muted-foreground max-w-2xl">
              {isAdmin 
                ? "Manage and respond to customer suggestions and feedback."
                : "We value your feedback! Submit your ideas and check out what others have suggested."}
            </p>
          </div>
          
          <SuggestionsDashboard 
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
