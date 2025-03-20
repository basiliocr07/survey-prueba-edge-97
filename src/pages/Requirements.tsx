
import { useState, useEffect } from 'react';
import Navbar from '@/components/layout/Navbar';
import Footer from '@/components/layout/Footer';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { FileText, Filter, Search, ClipboardList, BarChart, CheckCircle, Clock, Lightbulb } from 'lucide-react';
import { useQuery } from '@tanstack/react-query';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Input } from '@/components/ui/input';
import { Button } from '@/components/ui/button';
import { Badge } from "@/components/ui/badge";
import { 
  Select, 
  SelectContent, 
  SelectItem, 
  SelectTrigger, 
  SelectValue 
} from '@/components/ui/select';
import { useToast } from "@/hooks/use-toast";
import { Requirement } from '@/types/requirements';
import ClientRequirementForm from '@/components/client/ClientRequirementForm';

// Mock data for demonstration
const mockRequirements: Requirement[] = [
  {
    id: '1',
    title: 'Implement user authentication',
    content: 'Add secure user authentication with email and social login options.',
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
    projectArea: 'Authentication'
  },
  {
    id: '2',
    title: 'Create responsive dashboard',
    content: 'The dashboard should be fully responsive on all device sizes.',
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
    projectArea: 'Frontend'
  },
  {
    id: '3',
    title: 'Optimize database queries',
    content: 'Improve the performance of database queries on the products listing page.',
    customerName: 'Robert Johnson',
    customerEmail: 'robert@example.com',
    createdAt: '2023-07-10T11:30:00Z',
    status: 'proposed',
    category: 'Performance',
    priority: 'critical',
    isAnonymous: false,
    completionPercentage: 0,
    projectArea: 'Backend'
  }
];

export default function Requirements() {
  const [userRole, setUserRole] = useState('');
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [requirements, setRequirements] = useState<Requirement[]>(mockRequirements);
  const [activeTab, setActiveTab] = useState('view');
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState('');
  const [priorityFilter, setPriorityFilter] = useState('');
  const { toast } = useToast();
  
  useEffect(() => {
    const loggedIn = localStorage.getItem('isLoggedIn') === 'true';
    const role = localStorage.getItem('userRole') || '';
    
    setIsLoggedIn(loggedIn);
    setUserRole(role);
    
    // If user is not admin, default to submit tab
    if (!loggedIn || role.toLowerCase() !== 'admin') {
      setActiveTab('submit');
    }
    
    // Fetch requirements data from API in a real application
    const fetchRequirements = async () => {
      try {
        // Mock data for now
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
  
  // Calculate counts for dashboard
  const totalCount = requirements.length;
  const proposedCount = requirements.filter(r => r.status === 'proposed').length;
  const inProgressCount = requirements.filter(r => r.status === 'in-progress').length;
  const implementedCount = requirements.filter(r => r.status === 'implemented').length;
  
  const isAdmin = isLoggedIn && userRole.toLowerCase() === 'admin';
  
  // Simulate loading requirements with React Query
  const { data, isLoading } = useQuery({
    queryKey: ['requirements', statusFilter, priorityFilter, searchTerm],
    queryFn: async () => {
      // In production, this function would make a real API request
      console.log('Fetching requirements with filters:', { statusFilter, priorityFilter, searchTerm });
      
      // Simulate filtering
      let filtered = [...requirements];
      
      if (statusFilter) {
        filtered = filtered.filter(req => req.status.toLowerCase() === statusFilter.toLowerCase());
      }
      
      if (priorityFilter) {
        filtered = filtered.filter(req => req.priority?.toLowerCase() === priorityFilter.toLowerCase());
      }
      
      if (searchTerm) {
        filtered = filtered.filter(req => 
          req.title.toLowerCase().includes(searchTerm.toLowerCase()) || 
          req.content.toLowerCase().includes(searchTerm.toLowerCase())
        );
      }
      
      return filtered;
    },
    enabled: isAdmin
  });
  
  const filteredRequirements = data || requirements;
  
  // Function to display status with appropriate styling
  const getStatusBadge = (status: string) => {
    const statusColors: Record<string, string> = {
      'proposed': 'bg-blue-100 text-blue-800',
      'in-progress': 'bg-yellow-100 text-yellow-800',
      'implemented': 'bg-green-100 text-green-800',
      'rejected': 'bg-red-100 text-red-800',
    };
    
    const normalizedStatus = status.toLowerCase();
    const colorClass = statusColors[normalizedStatus] || 'bg-gray-100 text-gray-800';
    
    return (
      <span className={`px-2 py-1 rounded-full text-xs font-medium ${colorClass}`}>
        {status === 'in-progress' ? 'In Progress' : status.charAt(0).toUpperCase() + status.slice(1)}
      </span>
    );
  };
  
  // Function to display priority with appropriate styling
  const getPriorityBadge = (priority: string | undefined) => {
    if (!priority) return null;
    
    const priorityColors: Record<string, string> = {
      'critical': 'bg-red-100 text-red-800',
      'high': 'bg-orange-100 text-orange-800',
      'medium': 'bg-blue-100 text-blue-800',
      'low': 'bg-green-100 text-green-800',
    };
    
    const normalizedPriority = priority.toLowerCase();
    const colorClass = priorityColors[normalizedPriority] || 'bg-gray-100 text-gray-800';
    
    return (
      <span className={`px-2 py-1 rounded-full text-xs font-medium ${colorClass}`}>
        {priority.charAt(0).toUpperCase() + priority.slice(1)}
      </span>
    );
  };
  
  return (
    <div className="min-h-screen flex flex-col bg-background">
      <Navbar />
      
      <main className="flex-1 w-full px-4 sm:px-6 lg:px-8 pt-24 pb-20 flex justify-center">
        <div className="w-full max-w-[1000px]">
          <div className="mb-8">
            <h1 className="text-3xl font-bold tracking-tight sm:text-4xl mb-2">Requirements Management</h1>
            <p className="text-muted-foreground max-w-2xl">
              {isAdmin 
                ? "Advanced management and analysis of project requirements."
                : "Submit your project requirements and help us better understand your needs"}
            </p>
          </div>
          
          {isAdmin && (
            <div className="grid grid-cols-1 md:grid-cols-4 gap-4 mb-6">
              <Card>
                <CardContent className="flex flex-col items-center justify-center pt-6">
                  <div className="rounded-full bg-primary/10 p-3 mb-3">
                    <ClipboardList className="h-6 w-6 text-primary" />
                  </div>
                  <div className="text-2xl font-bold">{totalCount}</div>
                  <p className="text-sm text-muted-foreground">Total Requirements</p>
                </CardContent>
              </Card>
              
              <Card>
                <CardContent className="flex flex-col items-center justify-center pt-6">
                  <div className="rounded-full bg-blue-100 p-3 mb-3">
                    <Lightbulb className="h-6 w-6 text-blue-500" />
                  </div>
                  <div className="text-2xl font-bold">{proposedCount}</div>
                  <p className="text-sm text-muted-foreground">Proposed</p>
                </CardContent>
              </Card>
              
              <Card>
                <CardContent className="flex flex-col items-center justify-center pt-6">
                  <div className="rounded-full bg-yellow-100 p-3 mb-3">
                    <Clock className="h-6 w-6 text-yellow-500" />
                  </div>
                  <div className="text-2xl font-bold">{inProgressCount}</div>
                  <p className="text-sm text-muted-foreground">In Progress</p>
                </CardContent>
              </Card>
              
              <Card>
                <CardContent className="flex flex-col items-center justify-center pt-6">
                  <div className="rounded-full bg-green-100 p-3 mb-3">
                    <CheckCircle className="h-6 w-6 text-green-500" />
                  </div>
                  <div className="text-2xl font-bold">{implementedCount}</div>
                  <p className="text-sm text-muted-foreground">Implemented</p>
                </CardContent>
              </Card>
            </div>
          )}
          
          <Tabs value={activeTab} onValueChange={setActiveTab} className="w-full">
            <TabsList className="w-full grid grid-cols-3 mb-6">
              <TabsTrigger value="view">View Requirements</TabsTrigger>
              <TabsTrigger value="submit">Submit Requirement</TabsTrigger>
              <TabsTrigger value="reports">Reports</TabsTrigger>
            </TabsList>
            
            <TabsContent value="view" className="pt-4">
              <Card>
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <FileText className="h-5 w-5" />
                    Requirements List
                  </CardTitle>
                  <CardDescription>
                    View and manage all requirements
                  </CardDescription>
                  
                  <div className="flex flex-col md:flex-row gap-4 mt-4">
                    <div className="flex-1 relative">
                      <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
                      <Input
                        type="search"
                        placeholder="Search requirements..."
                        className="pl-8"
                        value={searchTerm}
                        onChange={(e) => setSearchTerm(e.target.value)}
                      />
                    </div>
                    <Select value={statusFilter} onValueChange={setStatusFilter}>
                      <SelectTrigger className="w-[180px]">
                        <SelectValue placeholder="Status" />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="all">All Statuses</SelectItem>
                        <SelectItem value="proposed">Proposed</SelectItem>
                        <SelectItem value="in-progress">In Progress</SelectItem>
                        <SelectItem value="implemented">Implemented</SelectItem>
                        <SelectItem value="rejected">Rejected</SelectItem>
                      </SelectContent>
                    </Select>
                    <Select value={priorityFilter} onValueChange={setPriorityFilter}>
                      <SelectTrigger className="w-[180px]">
                        <SelectValue placeholder="Priority" />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="all">All Priorities</SelectItem>
                        <SelectItem value="critical">Critical</SelectItem>
                        <SelectItem value="high">High</SelectItem>
                        <SelectItem value="medium">Medium</SelectItem>
                        <SelectItem value="low">Low</SelectItem>
                      </SelectContent>
                    </Select>
                  </div>
                </CardHeader>
                <CardContent>
                  {isLoading ? (
                    <div className="text-center py-8">Loading requirements...</div>
                  ) : filteredRequirements.length === 0 ? (
                    <div className="text-center py-8 text-muted-foreground">
                      No requirements found matching the current filters.
                    </div>
                  ) : (
                    <div className="space-y-4">
                      {filteredRequirements.map((req) => (
                        <Card key={req.id} className="hover:bg-gray-50">
                          <CardContent className="p-4">
                            <div className="flex justify-between items-start mb-2">
                              <h3 className="font-medium">{req.title}</h3>
                              <div className="flex gap-2">
                                {getStatusBadge(req.status)}
                                {getPriorityBadge(req.priority)}
                              </div>
                            </div>
                            <p className="text-sm text-muted-foreground mb-2 line-clamp-2">
                              {req.content}
                            </p>
                            <div className="flex justify-between text-xs text-muted-foreground">
                              <span>Area: {req.projectArea}</span>
                              <span>Created: {new Date(req.createdAt).toLocaleDateString()}</span>
                              <span>By: {req.isAnonymous ? 'Anonymous' : req.customerName}</span>
                            </div>
                            {isAdmin && (
                              <div className="mt-3 flex justify-end">
                                <Button variant="outline" size="sm" className="mr-2">
                                  View details
                                </Button>
                                <Button size="sm">
                                  Edit
                                </Button>
                              </div>
                            )}
                          </CardContent>
                        </Card>
                      ))}
                    </div>
                  )}
                </CardContent>
              </Card>
            </TabsContent>
            
            <TabsContent value="submit">
              <Card>
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <FileText className="h-5 w-5" />
                    New Requirement
                  </CardTitle>
                  <CardDescription>
                    Submit a new project requirement
                  </CardDescription>
                </CardHeader>
                <CardContent>
                  <ClientRequirementForm />
                </CardContent>
              </Card>
            </TabsContent>
            
            <TabsContent value="reports">
              <Card>
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <BarChart className="h-5 w-5" />
                    Requirements Analytics
                  </CardTitle>
                  <CardDescription>
                    View analytics and reports for all requirements
                  </CardDescription>
                </CardHeader>
                <CardContent className="flex justify-center py-10">
                  {isAdmin ? (
                    <div className="text-center text-muted-foreground">
                      Requirements analytics will be implemented soon.
                    </div>
                  ) : (
                    <div className="text-center text-muted-foreground">
                      You need admin privileges to view analytics.
                    </div>
                  )}
                </CardContent>
              </Card>
            </TabsContent>
          </Tabs>
        </div>
      </main>
      
      <Footer />
    </div>
  );
}
