
import { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { 
  Tabs, 
  TabsContent, 
  TabsList, 
  TabsTrigger 
} from "@/components/ui/tabs";
import { 
  FileText, 
  ClipboardList, 
  BarChart, 
  CheckCircle, 
  Clock, 
  AlertCircle 
} from 'lucide-react';
import ClientRequirementForm from '@/components/client/ClientRequirementForm';
import AdvancedRequirementsList from '@/components/requirements/AdvancedRequirementsList';
import RequirementReports from '@/components/requirements/RequirementReports';
import { Requirement } from '@/types/requirements';

interface RequirementsDashboardProps {
  isAdmin: boolean;
  requirements: Requirement[];
  totalCount?: number;
  proposedCount?: number;
  inProgressCount?: number;
  implementedCount?: number;
  rejectedCount?: number;
}

export default function RequirementsDashboard({ 
  isAdmin = false, 
  requirements = [],
  totalCount = 0,
  proposedCount = 0,
  inProgressCount = 0,
  implementedCount = 0,
  rejectedCount = 0
}: RequirementsDashboardProps) {
  const [activeTab, setActiveTab] = useState<string>(isAdmin ? 'view' : 'submit');

  // Ensure we have valid counts, falling back to calculating from requirements if needed
  const safeRequirements = Array.isArray(requirements) ? requirements : [];
  const safeTotalCount = totalCount || safeRequirements.length;
  const safeProposedCount = proposedCount || safeRequirements.filter(req => req.status === 'proposed').length;
  const safeInProgressCount = inProgressCount || safeRequirements.filter(req => req.status === 'in-progress').length;
  const safeImplementedCount = implementedCount || safeRequirements.filter(req => req.status === 'implemented').length;
  const safeRejectedCount = rejectedCount || safeRequirements.filter(req => req.status === 'rejected').length;

  return (
    <div className="space-y-6">
      {isAdmin && (
        <div className="grid grid-cols-1 md:grid-cols-5 gap-4">
          <Card>
            <CardContent className="flex flex-col items-center justify-center pt-6">
              <div className="rounded-full bg-primary/10 p-3 mb-3">
                <ClipboardList className="h-6 w-6 text-primary" />
              </div>
              <div className="text-2xl font-bold">{safeTotalCount}</div>
              <p className="text-sm text-muted-foreground">Total Requirements</p>
            </CardContent>
          </Card>
          
          <Card>
            <CardContent className="flex flex-col items-center justify-center pt-6">
              <div className="rounded-full bg-blue-100 p-3 mb-3">
                <Clock className="h-6 w-6 text-blue-500" />
              </div>
              <div className="text-2xl font-bold">{safeProposedCount}</div>
              <p className="text-sm text-muted-foreground">Proposed</p>
            </CardContent>
          </Card>
          
          <Card>
            <CardContent className="flex flex-col items-center justify-center pt-6">
              <div className="rounded-full bg-yellow-100 p-3 mb-3">
                <FileText className="h-6 w-6 text-yellow-500" />
              </div>
              <div className="text-2xl font-bold">{safeInProgressCount}</div>
              <p className="text-sm text-muted-foreground">In Progress</p>
            </CardContent>
          </Card>
          
          <Card>
            <CardContent className="flex flex-col items-center justify-center pt-6">
              <div className="rounded-full bg-green-100 p-3 mb-3">
                <CheckCircle className="h-6 w-6 text-green-500" />
              </div>
              <div className="text-2xl font-bold">{safeImplementedCount}</div>
              <p className="text-sm text-muted-foreground">Implemented</p>
            </CardContent>
          </Card>
          
          <Card>
            <CardContent className="flex flex-col items-center justify-center pt-6">
              <div className="rounded-full bg-red-100 p-3 mb-3">
                <AlertCircle className="h-6 w-6 text-red-500" />
              </div>
              <div className="text-2xl font-bold">{safeRejectedCount}</div>
              <p className="text-sm text-muted-foreground">Rejected</p>
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
          <AdvancedRequirementsList requirements={requirements} isAdmin={isAdmin} />
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
              <ClientRequirementForm />
            </CardContent>
          </Card>
        </TabsContent>
        
        <TabsContent value="reports">
          <RequirementReports requirements={requirements} />
        </TabsContent>
      </Tabs>
    </div>
  );
}
