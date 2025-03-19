
import { useState, useEffect } from 'react';
import Navbar from '@/components/layout/Navbar';
import Footer from '@/components/layout/Footer';
import ClientSuggestionForm from '@/components/client/ClientSuggestionForm';
import SuggestionsList from '@/components/suggestions/SuggestionsList';
import SuggestionReports from '@/components/suggestions/SuggestionReports';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { MessageSquare } from 'lucide-react';

export default function Suggestions() {
  const [userRole, setUserRole] = useState('');
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [activeTab, setActiveTab] = useState('submit');
  
  useEffect(() => {
    const loggedIn = localStorage.getItem('isLoggedIn') === 'true';
    const role = localStorage.getItem('userRole') || '';
    
    setIsLoggedIn(loggedIn);
    setUserRole(role);
    
    // If admin user, default to "view" tab
    if (role.toLowerCase() === 'admin') {
      setActiveTab('submit');
    }
  }, []);
  
  return (
    <div className="min-h-screen flex flex-col bg-background">
      <Navbar />
      
      <main className="flex-1 w-full px-4 sm:px-6 lg:px-8 pt-24 pb-20 flex justify-center">
        <div className="w-[900px] max-w-[900px] h-full bg-white p-6">
          <div className="text-center mb-8">
            <h1 className="text-3xl font-bold tracking-tight sm:text-4xl mb-2">Customer Suggestions</h1>
            <p className="text-muted-foreground max-w-2xl mx-auto">
              We value your feedback! Submit your ideas and check out what others have suggested.
            </p>
          </div>
          
          {isLoggedIn && userRole.toLowerCase() === 'admin' ? (
            // Admin view with all tabs
            <Tabs value={activeTab} onValueChange={setActiveTab} className="w-full">
              <TabsList className="grid grid-cols-3 mb-8">
                <TabsTrigger value="submit">Submit Suggestion</TabsTrigger>
                <TabsTrigger value="view">View Suggestions</TabsTrigger>
                <TabsTrigger value="reports">Monthly Reports</TabsTrigger>
              </TabsList>
              
              <TabsContent value="submit">
                <Card>
                  <CardHeader>
                    <CardTitle className="flex items-center gap-2">
                      <MessageSquare className="h-5 w-5" />
                      New Suggestion
                    </CardTitle>
                    <CardDescription>
                      Share your ideas on how we can improve our services
                    </CardDescription>
                  </CardHeader>
                  <CardContent>
                    <ClientSuggestionForm />
                  </CardContent>
                </Card>
              </TabsContent>
              
              <TabsContent value="view">
                <SuggestionsList suggestions={[]} />
              </TabsContent>
              
              <TabsContent value="reports">
                <SuggestionReports suggestions={[]} />
              </TabsContent>
            </Tabs>
          ) : (
            // Client view with only submission form
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <MessageSquare className="h-5 w-5" />
                  New Suggestion
                </CardTitle>
                <CardDescription>
                  Share your ideas on how we can improve our services
                </CardDescription>
              </CardHeader>
              <CardContent>
                <ClientSuggestionForm />
              </CardContent>
            </Card>
          )}
        </div>
      </main>
      
      <Footer />
    </div>
  );
}
