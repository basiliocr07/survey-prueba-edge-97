
import { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { 
  Tabs, 
  TabsContent, 
  TabsList, 
  TabsTrigger 
} from "@/components/ui/tabs";
import { Badge } from "@/components/ui/badge";
import { 
  MessageSquare, 
  ClipboardList, 
  BarChart, 
  CheckCircle, 
  Clock, 
  AlertCircle 
} from 'lucide-react';
import { Suggestion } from '@/types/suggestions';
import AdvancedSuggestionsList from './AdvancedSuggestionsList';
import AdvancedSuggestionForm from './AdvancedSuggestionForm';
import AdvancedSuggestionReports from './AdvancedSuggestionReports';

interface AdvancedSuggestionsDashboardProps {
  isAdmin: boolean;
  suggestions: Suggestion[];
  totalCount?: number;
  newCount?: number;
  inProgressCount?: number;
  completedCount?: number;
}

export default function AdvancedSuggestionsDashboard({ 
  isAdmin = false, 
  suggestions = [],
  totalCount = 0,
  newCount = 0,
  inProgressCount = 0,
  completedCount = 0
}: AdvancedSuggestionsDashboardProps) {
  const [activeTab, setActiveTab] = useState<string>(isAdmin ? 'view' : 'submit');

  return (
    <div className="space-y-6">
      {isAdmin && (
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
          <Card>
            <CardContent className="flex flex-col items-center justify-center pt-6">
              <div className="rounded-full bg-primary/10 p-3 mb-3">
                <ClipboardList className="h-6 w-6 text-primary" />
              </div>
              <div className="text-2xl font-bold">{totalCount}</div>
              <p className="text-sm text-muted-foreground">Total Suggestions</p>
            </CardContent>
          </Card>
          
          <Card>
            <CardContent className="flex flex-col items-center justify-center pt-6">
              <div className="rounded-full bg-blue-100 p-3 mb-3">
                <Clock className="h-6 w-6 text-blue-500" />
              </div>
              <div className="text-2xl font-bold">{newCount}</div>
              <p className="text-sm text-muted-foreground">New</p>
            </CardContent>
          </Card>
          
          <Card>
            <CardContent className="flex flex-col items-center justify-center pt-6">
              <div className="rounded-full bg-yellow-100 p-3 mb-3">
                <MessageSquare className="h-6 w-6 text-yellow-500" />
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
              <div className="text-2xl font-bold">{completedCount}</div>
              <p className="text-sm text-muted-foreground">Completed</p>
            </CardContent>
          </Card>
        </div>
      )}
      
      <Tabs value={activeTab} onValueChange={setActiveTab} className="w-full">
        <TabsList className="grid w-full grid-cols-3 mb-6">
          <TabsTrigger value="view">View Suggestions</TabsTrigger>
          <TabsTrigger value="submit">Submit Suggestion</TabsTrigger>
          <TabsTrigger value="reports">Reports</TabsTrigger>
        </TabsList>
        
        <TabsContent value="view" className="space-y-6">
          <AdvancedSuggestionsList suggestions={suggestions} isAdmin={isAdmin} />
        </TabsContent>
        
        <TabsContent value="submit">
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2">
                <MessageSquare className="h-5 w-5" />
                New Suggestion
              </CardTitle>
            </CardHeader>
            <CardContent>
              <AdvancedSuggestionForm />
            </CardContent>
          </Card>
        </TabsContent>
        
        <TabsContent value="reports">
          <AdvancedSuggestionReports suggestions={suggestions} />
        </TabsContent>
      </Tabs>
    </div>
  );
}
