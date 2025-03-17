
import { useState, useEffect } from 'react';
import { Link, useLocation, useNavigate } from 'react-router-dom';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import ClientSuggestionForm from '@/components/client/ClientSuggestionForm';
import ClientRequirementForm from '@/components/client/ClientRequirementForm';
import Footer from '@/components/layout/Footer';
import { ChevronLeft, Home, MessageSquare, FileText } from 'lucide-react';
import {
  NavigationMenu,
  NavigationMenuContent,
  NavigationMenuItem,
  NavigationMenuLink,
  NavigationMenuList,
  NavigationMenuTrigger,
  navigationMenuTriggerStyle,
} from "@/components/ui/navigation-menu";

export default function ClientAccess() {
  const location = useLocation();
  const navigate = useNavigate();
  const [activeView, setActiveView] = useState('suggestion');
  
  useEffect(() => {
    // Get the view from the URL query parameters
    const params = new URLSearchParams(location.search);
    const view = params.get('view');
    if (view === 'requirement' || view === 'suggestion') {
      setActiveView(view);
    } else {
      setActiveView('suggestion'); // Default view
    }
  }, [location]);

  const handleViewChange = (view: string) => {
    setActiveView(view);
    navigate(`/client?view=${view}`);
  };

  return (
    <div className="min-h-screen flex flex-col bg-background">
      <header className="bg-primary py-4 px-6 flex justify-between items-center shadow-md">
        <div className="flex items-center">
          <h1 className="text-white text-xl font-semibold">Client Portal</h1>
        </div>
        
        {/* Navigation menu for client portal */}
        <NavigationMenu className="hidden md:flex">
          <NavigationMenuList>
            <NavigationMenuItem>
              <Link to="/client">
                <NavigationMenuLink className={navigationMenuTriggerStyle()}>
                  <Home className="w-4 h-4 mr-2" />
                  Home
                </NavigationMenuLink>
              </Link>
            </NavigationMenuItem>
            
            <NavigationMenuItem>
              <NavigationMenuTrigger className="h-9 px-3">
                <MessageSquare className="w-4 h-4 mr-2" />
                Suggestions
              </NavigationMenuTrigger>
              <NavigationMenuContent>
                <div className="grid gap-3 p-4 w-[400px]">
                  <div className="flex flex-col gap-1">
                    <Link 
                      to="/client?view=suggestion"
                      className="text-sm font-medium leading-none mb-1 group flex items-center"
                      onClick={(e) => {
                        e.preventDefault();
                        handleViewChange('suggestion');
                      }}
                    >
                      <MessageSquare className="w-4 h-4 mr-2" />
                      Submit Suggestion
                    </Link>
                    <p className="text-xs text-muted-foreground mb-2">Share your ideas on how we can improve our services</p>
                    <div className="grid gap-2">
                      <div className="group grid grid-cols-[20px_1fr] gap-1 rounded-md p-2 text-sm items-center">
                        <div className="h-1 w-1 rounded-full bg-primary"></div>
                        <div>
                          <div className="font-medium">New Suggestion</div>
                          <div className="text-xs text-muted-foreground">Submit a new suggestion or idea</div>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </NavigationMenuContent>
            </NavigationMenuItem>
            
            <NavigationMenuItem>
              <NavigationMenuTrigger className="h-9 px-3">
                <FileText className="w-4 h-4 mr-2" />
                Requirements
              </NavigationMenuTrigger>
              <NavigationMenuContent>
                <div className="grid gap-3 p-4 w-[400px]">
                  <div className="flex flex-col gap-1">
                    <Link 
                      to="/client?view=requirement"
                      className="text-sm font-medium leading-none mb-1 group flex items-center"
                      onClick={(e) => {
                        e.preventDefault();
                        handleViewChange('requirement');
                      }}
                    >
                      <FileText className="w-4 h-4 mr-2" />
                      Submit Requirement
                    </Link>
                    <p className="text-xs text-muted-foreground mb-2">Submit a specific requirement or feature request</p>
                    <div className="grid gap-2">
                      <div className="group grid grid-cols-[20px_1fr] gap-1 rounded-md p-2 text-sm items-center">
                        <div className="h-1 w-1 rounded-full bg-primary"></div>
                        <div>
                          <div className="font-medium">New Requirement</div>
                          <div className="text-xs text-muted-foreground">Submit a detailed feature requirement</div>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </NavigationMenuContent>
            </NavigationMenuItem>
          </NavigationMenuList>
        </NavigationMenu>
        
        <Link to="/" className="flex items-center text-white text-sm hover:underline">
          <ChevronLeft className="h-4 w-4 mr-1" />
          Back to main site
        </Link>
      </header>
      
      <main className="flex-1 w-full px-4 sm:px-6 lg:px-8 py-12 flex justify-center">
        <div className="w-[900px] max-w-[900px] h-full bg-white p-6 shadow-md rounded-lg">
          <div className="text-center mb-8">
            <h1 className="text-3xl font-bold tracking-tight sm:text-4xl mb-2">
              Welcome to SurveyMaster
            </h1>
            <p className="text-muted-foreground max-w-2xl mx-auto">
              Submit your {activeView === 'suggestion' ? 'suggestions' : 'requirements'} to help us improve our services
            </p>
          </div>

          {activeView === 'suggestion' ? (
            <Card>
              <CardHeader>
                <CardTitle>New Suggestion</CardTitle>
                <CardDescription>
                  Share your ideas on how we can improve our services
                </CardDescription>
              </CardHeader>
              <CardContent>
                <ClientSuggestionForm />
              </CardContent>
            </Card>
          ) : (
            <Card>
              <CardHeader>
                <CardTitle>New Requirement</CardTitle>
                <CardDescription>
                  Submit a specific requirement or feature request
                </CardDescription>
              </CardHeader>
              <CardContent>
                <ClientRequirementForm />
              </CardContent>
            </Card>
          )}
        </div>
      </main>
      
      <Footer />
    </div>
  );
}
