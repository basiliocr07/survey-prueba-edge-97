
import { useState, useEffect } from 'react';
import Navbar from '@/components/layout/Navbar';
import Footer from '@/components/layout/Footer';
import ClientRequirementForm from '@/components/client/ClientRequirementForm';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { FileText } from 'lucide-react';

export default function Requirements() {
  const [userRole, setUserRole] = useState('');
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  
  useEffect(() => {
    const loggedIn = localStorage.getItem('isLoggedIn') === 'true';
    const role = localStorage.getItem('userRole') || '';
    
    setIsLoggedIn(loggedIn);
    setUserRole(role);
  }, []);
  
  return (
    <div className="min-h-screen flex flex-col bg-background">
      <Navbar />
      
      <main className="flex-1 w-full px-4 sm:px-6 lg:px-8 pt-24 pb-20 flex justify-center">
        <div className="w-[900px] max-w-[900px] h-full bg-white p-6">
          <div className="text-center mb-8">
            <h1 className="text-3xl font-bold tracking-tight sm:text-4xl mb-2">Requirements Management</h1>
            <p className="text-muted-foreground max-w-2xl mx-auto">
              Submit your project requirements and help us better understand your needs
            </p>
          </div>
          
          {isLoggedIn && userRole.toLowerCase() === 'admin' ? (
            // Admin view would typically include more features
            <div className="space-y-6">
              {/* This is just a placeholder for the admin view */}
              <p className="text-center text-muted-foreground mb-4">
                As an administrator, you would see the full requirements management interface here.
              </p>
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
            </div>
          ) : (
            // Client view with only submission form
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
          )}
        </div>
      </main>
      
      <Footer />
    </div>
  );
}
