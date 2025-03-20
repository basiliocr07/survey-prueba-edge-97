
import { useState, useEffect } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { 
  Tabs, 
  TabsContent, 
  TabsList, 
  TabsTrigger 
} from "@/components/ui/tabs";
import { Badge } from "@/components/ui/badge";
import { 
  ClipboardList, 
  BarChart, 
  CheckCircle, 
  Clock,
  FileText,
  Lightbulb
} from 'lucide-react';
import { Requirement } from '@/types/requirements';
import AdvancedRequirementsList from './AdvancedRequirementsList';
import AdvancedRequirementForm from './AdvancedRequirementForm';
import AdvancedRequirementReports from './AdvancedRequirementReports';

interface AdvancedRequirementsDashboardProps {
  isAdmin: boolean;
  requirements: Requirement[];
  totalCount?: number;
  proposedCount?: number;
  inProgressCount?: number;
  implementedCount?: number;
}

export default function AdvancedRequirementsDashboard({ 
  isAdmin = false, 
  requirements = [],
  totalCount = 0,
  proposedCount = 0,
  inProgressCount = 0,
  implementedCount = 0
}: AdvancedRequirementsDashboardProps) {
  const [activeTab, setActiveTab] = useState<string>(isAdmin ? 'view' : 'submit');

  console.log("AdvancedRequirementsDashboard - Props received:", { 
    isAdmin, 
    requirementsCount: requirements?.length || 0,
    totalCount,
    proposedCount,
    inProgressCount,
    implementedCount
  });

  useEffect(() => {
    console.log("Dashboard component mounted or updated");
    if (!requirements || requirements.length === 0) {
      console.log("Warning: No requirements data received in dashboard");
    }
  }, [requirements]);

  // Ensure we always have a valid array, even if requirements is undefined
  const safeRequirements = requirements || [];
  
  // Use either provided counts or calculate them from the requirements array
  const displayTotalCount = totalCount || safeRequirements.length;
  const displayProposedCount = proposedCount || safeRequirements.filter(r => r.status === 'proposed').length;
  const displayInProgressCount = inProgressCount || safeRequirements.filter(r => r.status === 'in-progress').length;
  const displayImplementedCount = implementedCount || safeRequirements.filter(r => r.status === 'implemented').length;

  console.log("Calculated display counts:", {
    displayTotalCount,
    displayProposedCount,
    displayInProgressCount,
    displayImplementedCount
  });

  return (
    <div className="space-y-6">
      {isAdmin && (
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
          <Card>
            <CardContent className="flex flex-col items-center justify-center pt-6">
              <div className="rounded-full bg-primary/10 p-3 mb-3">
                <ClipboardList className="h-6 w-6 text-primary" />
              </div>
              <div className="text-2xl font-bold">{displayTotalCount}</div>
              <p className="text-sm text-muted-foreground">Total Requirements</p>
            </CardContent>
          </Card>
          
          <Card>
            <CardContent className="flex flex-col items-center justify-center pt-6">
              <div className="rounded-full bg-blue-100 p-3 mb-3">
                <Lightbulb className="h-6 w-6 text-blue-500" />
              </div>
              <div className="text-2xl font-bold">{displayProposedCount}</div>
              <p className="text-sm text-muted-foreground">Proposed</p>
            </CardContent>
          </Card>
          
          <Card>
            <CardContent className="flex flex-col items-center justify-center pt-6">
              <div className="rounded-full bg-yellow-100 p-3 mb-3">
                <Clock className="h-6 w-6 text-yellow-500" />
              </div>
              <div className="text-2xl font-bold">{displayInProgressCount}</div>
              <p className="text-sm text-muted-foreground">In Progress</p>
            </CardContent>
          </Card>
          
          <Card>
            <CardContent className="flex flex-col items-center justify-center pt-6">
              <div className="rounded-full bg-green-100 p-3 mb-3">
                <CheckCircle className="h-6 w-6 text-green-500" />
              </div>
              <div className="text-2xl font-bold">{displayImplementedCount}</div>
              <p className="text-sm text-muted-foreground">Implemented</p>
            </CardContent>
          </Card>
        </div>
      )}
      
      <Tabs value={activeTab} onValueChange={setActiveTab} className="w-full">
        <TabsList className="grid w-full grid-cols-3 mb-6">
          <TabsTrigger value="view">View Requirements</TabsTrigger>
          <TabsTrigger value="submit">Submit Requirement</TabsTrigger>
          <TabsTrigger value="reports">Reports</TabsTrigger>
        </TabsList>
        
        <TabsContent value="view" className="space-y-6">
          <AdvancedRequirementsList requirements={safeRequirements} isAdmin={isAdmin} />
        </TabsContent>
        
        <TabsContent value="submit">
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <FileText className="h-5 w-5" />
                New Requirement
              </CardTitle>
            </CardHeader>
            <CardContent>
              <AdvancedRequirementForm />
            </CardContent>
          </Card>
        </TabsContent>
        
        <TabsContent value="reports">
          <AdvancedRequirementReports requirements={safeRequirements} />
        </TabsContent>
      </Tabs>
    </div>
  );
}
