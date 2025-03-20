
import { useState, useEffect } from 'react';
import Navbar from '@/components/layout/Navbar';
import Footer from '@/components/layout/Footer';
import AdvancedRequirementsDashboard from '@/components/requirements/AdvancedRequirementsDashboard';
import { useToast } from "@/hooks/use-toast";
import { Requirement } from '@/types/requirements';

// Mock data for demonstration
const mockRequirements: Requirement[] = [
  {
    id: '1',
    title: 'Implement user authentication',
    content: 'Add secure user authentication with email and social login options.',
    description: 'Add secure user authentication with email and social login options.',
    customerName: 'John Doe',
    customerEmail: 'john@example.com',
    createdAt: '2023-07-15T10:30:00Z',
    status: 'implemented',
    category: 'Security',
    priority: 'high',
    isAnonymous: false,
    response: 'We have implemented this feature with both email and social login options.',
    responseDate: '2023-07-25T14:15:00Z',
    completionPercentage: 100,
    projectArea: 'Authentication',
    acceptanceCriteria: 'Users should be able to log in with email and at least one social provider'
  },
  {
    id: '2',
    title: 'Create responsive dashboard',
    content: 'The dashboard should be fully responsive on all device sizes.',
    description: 'The dashboard should be fully responsive on all device sizes.',
    customerName: 'Jane Smith',
    customerEmail: 'jane@example.com',
    createdAt: '2023-07-17T15:45:00Z',
    status: 'in-progress',
    category: 'UI/UX',
    priority: 'medium',
    isAnonymous: false,
    response: 'Our team is working on implementing responsive design across all pages.',
    responseDate: '2023-07-21T09:20:00Z',
    completionPercentage: 50,
    projectArea: 'Frontend',
    acceptanceCriteria: 'Dashboard should work well on mobile, tablet and desktop'
  },
  {
    id: '3',
    title: 'Optimize database queries',
    content: 'Improve the performance of database queries on the products listing page.',
    description: 'Improve the performance of database queries on the products listing page.',
    customerName: 'Robert Johnson',
    customerEmail: 'robert@example.com',
    createdAt: '2023-07-10T11:30:00Z',
    status: 'proposed',
    category: 'Performance',
    priority: 'critical',
    isAnonymous: false,
    completionPercentage: 0,
    projectArea: 'Backend',
    acceptanceCriteria: 'Page load time should be reduced by at least 50%'
  }
];

export default function AdvancedRequirements() {
  const [userRole, setUserRole] = useState('admin'); // Default to admin for testing
  const [isLoggedIn, setIsLoggedIn] = useState(true); // Default to logged in for testing
  const [requirements, setRequirements] = useState<Requirement[]>(mockRequirements);
  const { toast } = useToast();
  
  useEffect(() => {
    // For debugging
    console.log("Requirements data:", requirements);
    
    const loggedIn = localStorage.getItem('isLoggedIn') === 'true';
    const role = localStorage.getItem('userRole') || 'admin'; // Default to admin for testing
    
    setIsLoggedIn(true); // Force to true for testing
    setUserRole(role);
    
    // Fetch requirements data from API in a real application
    const fetchRequirements = async () => {
      try {
        // Use mock data for now
        console.log("Setting requirements to:", mockRequirements);
        setRequirements(mockRequirements);
      } catch (error) {
        console.error('Error fetching requirements:', error);
        toast({
          title: "Error",
          description: "Failed to load requirements data",
          variant: "destructive"
        });
      }
    };
    
    fetchRequirements();
  }, [toast]);
  
  // Calculate counts for admin dashboard
  const totalCount = requirements.length;
  const proposedCount = requirements.filter(r => r.status === 'proposed').length;
  const inProgressCount = requirements.filter(r => r.status === 'in-progress').length;
  const implementedCount = requirements.filter(r => r.status === 'implemented').length;
  
  const isAdmin = isLoggedIn && userRole.toLowerCase() === 'admin';
  
  return (
    <div className="min-h-screen flex flex-col bg-background">
      <Navbar />
      
      <main className="flex-1 w-full px-4 sm:px-6 lg:px-8 pt-24 pb-20 flex justify-center">
        <div className="w-full max-w-[1000px]">
          <div className="mb-8">
            <h1 className="text-3xl font-bold tracking-tight sm:text-4xl mb-2">Advanced Requirements</h1>
            <p className="text-muted-foreground max-w-2xl">
              {isAdmin 
                ? "Advanced management and analysis of project requirements."
                : "We value your input! Submit your project requirements and track their progress."}
            </p>
          </div>
          
          <AdvancedRequirementsDashboard 
            isAdmin={isAdmin}
            requirements={requirements}
            totalCount={totalCount}
            proposedCount={proposedCount}
            inProgressCount={inProgressCount}
            implementedCount={implementedCount}
          />
        </div>
      </main>
      
      <Footer />
    </div>
  );
}
