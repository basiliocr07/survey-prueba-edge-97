
import { useState } from 'react';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import ClientSuggestionForm from '@/components/client/ClientSuggestionForm';
import ClientRequirementForm from '@/components/client/ClientRequirementForm';
import Footer from '@/components/layout/Footer';
import { Link } from 'react-router-dom';
import { ChevronLeft, Home, MessageSquare, FileText } from 'lucide-react';

export default function ClientAccess() {
  const [activeTab, setActiveTab] = useState('suggestion');

  return (
    <div className="min-h-screen flex flex-col bg-background">
      <header className="bg-primary py-4 px-6 flex justify-between items-center shadow-md">
        <div className="flex items-center">
          <h1 className="text-white text-xl font-semibold">Client Portal</h1>
        </div>
        <nav className="hidden md:flex items-center space-x-6">
          <Link to="/client" className="text-white hover:text-white/80 flex items-center">
            <Home className="w-4 h-4 mr-1" />
            Home
          </Link>
          <Link 
            to="/client?tab=suggestion" 
            onClick={(e) => {
              e.preventDefault();
              setActiveTab('suggestion');
            }} 
            className="text-white hover:text-white/80 flex items-center"
          >
            <MessageSquare className="w-4 h-4 mr-1" />
            Suggestions
          </Link>
          <Link 
            to="/client?tab=requirement" 
            onClick={(e) => {
              e.preventDefault();
              setActiveTab('requirement');
            }} 
            className="text-white hover:text-white/80 flex items-center"
          >
            <FileText className="w-4 h-4 mr-1" />
            Requirements
          </Link>
        </nav>
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
              Submit your suggestions or requirements to help us improve our services
            </p>
          </div>

          <Tabs value={activeTab} onValueChange={setActiveTab} className="w-full">
            <TabsList className="grid grid-cols-2 mb-8">
              <TabsTrigger value="suggestion">Submit Suggestion</TabsTrigger>
              <TabsTrigger value="requirement">Submit Requirement</TabsTrigger>
            </TabsList>
            
            <TabsContent value="suggestion">
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
            </TabsContent>
            
            <TabsContent value="requirement">
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
            </TabsContent>
          </Tabs>
        </div>
      </main>
      
      <Footer />
    </div>
  );
}
