
import { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { 
  Select, 
  SelectContent, 
  SelectItem, 
  SelectTrigger, 
  SelectValue 
} from "@/components/ui/select";
import { Search, Filter, Flag, CalendarClock } from 'lucide-react';
import { Requirement } from '@/types/requirements';

interface AdvancedRequirementsListProps {
  requirements: Requirement[];
  isAdmin: boolean;
}

export default function AdvancedRequirementsList({ requirements, isAdmin }: AdvancedRequirementsListProps) {
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState('');
  const [priorityFilter, setPriorityFilter] = useState('');
  const [categoryFilter, setCategoryFilter] = useState('');
  
  // Get unique categories from requirements
  const categories = [...new Set(requirements.map(req => req.category).filter(Boolean))];
  
  // Filter requirements based on search term and selected filters
  const filteredRequirements = requirements.filter(req => {
    const matchesSearch = 
      req.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
      req.content.toLowerCase().includes(searchTerm.toLowerCase()) ||
      req.customerName.toLowerCase().includes(searchTerm.toLowerCase());
    
    const matchesStatus = statusFilter ? req.status === statusFilter : true;
    const matchesPriority = priorityFilter ? req.priority === priorityFilter : true;
    const matchesCategory = categoryFilter ? req.category === categoryFilter : true;
    
    return matchesSearch && matchesStatus && matchesPriority && matchesCategory;
  });
  
  // Function to determine badge color based on status
  const getStatusBadge = (status: string) => {
    switch (status) {
      case 'proposed':
        return <Badge variant="outline" className="bg-blue-100 text-blue-800 hover:bg-blue-100">{status}</Badge>;
      case 'in-progress':
        return <Badge variant="outline" className="bg-yellow-100 text-yellow-800 hover:bg-yellow-100">{status}</Badge>;
      case 'implemented':
        return <Badge variant="outline" className="bg-green-100 text-green-800 hover:bg-green-100">{status}</Badge>;
      case 'rejected':
        return <Badge variant="outline" className="bg-red-100 text-red-800 hover:bg-red-100">{status}</Badge>;
      default:
        return <Badge variant="outline">{status}</Badge>;
    }
  };
  
  // Function to determine badge color based on priority
  const getPriorityBadge = (priority?: string) => {
    if (!priority) return null;
    
    switch (priority) {
      case 'critical':
        return <Badge variant="outline" className="bg-red-100 text-red-800 hover:bg-red-100">{priority}</Badge>;
      case 'high':
        return <Badge variant="outline" className="bg-orange-100 text-orange-800 hover:bg-orange-100">{priority}</Badge>;
      case 'medium':
        return <Badge variant="outline" className="bg-blue-100 text-blue-800 hover:bg-blue-100">{priority}</Badge>;
      case 'low':
        return <Badge variant="outline" className="bg-green-100 text-green-800 hover:bg-green-100">{priority}</Badge>;
      default:
        return <Badge variant="outline">{priority}</Badge>;
    }
  };
  
  return (
    <Card>
      <CardHeader>
        <CardTitle className="flex items-center justify-between">
          <span>Requirements List</span>
          <div className="flex items-center space-x-2">
            <span className="text-sm text-muted-foreground">
              {filteredRequirements.length} requirement(s)
            </span>
          </div>
        </CardTitle>
        
        <div className="flex flex-col space-y-4 md:flex-row md:space-y-0 md:space-x-4">
          <div className="relative flex-1">
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
            <SelectTrigger className="w-[160px]">
              <div className="flex items-center">
                <CalendarClock className="mr-2 h-4 w-4" />
                <SelectValue placeholder="Status" />
              </div>
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="">All statuses</SelectItem>
              <SelectItem value="proposed">Proposed</SelectItem>
              <SelectItem value="in-progress">In Progress</SelectItem>
              <SelectItem value="implemented">Implemented</SelectItem>
              <SelectItem value="rejected">Rejected</SelectItem>
            </SelectContent>
          </Select>
          
          <Select value={priorityFilter} onValueChange={setPriorityFilter}>
            <SelectTrigger className="w-[160px]">
              <div className="flex items-center">
                <Flag className="mr-2 h-4 w-4" />
                <SelectValue placeholder="Priority" />
              </div>
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="">All priorities</SelectItem>
              <SelectItem value="critical">Critical</SelectItem>
              <SelectItem value="high">High</SelectItem>
              <SelectItem value="medium">Medium</SelectItem>
              <SelectItem value="low">Low</SelectItem>
            </SelectContent>
          </Select>
          
          {categories.length > 0 && (
            <Select value={categoryFilter} onValueChange={setCategoryFilter}>
              <SelectTrigger className="w-[160px]">
                <div className="flex items-center">
                  <Filter className="mr-2 h-4 w-4" />
                  <SelectValue placeholder="Category" />
                </div>
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="">All categories</SelectItem>
                {categories.map((category) => (
                  <SelectItem key={category} value={category as string}>
                    {category}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          )}
        </div>
      </CardHeader>
      
      <CardContent>
        {filteredRequirements.length === 0 ? (
          <div className="flex flex-col items-center justify-center py-8 text-center">
            <div className="rounded-full bg-muted p-3 mb-3">
              <Search className="h-6 w-6 text-muted-foreground" />
            </div>
            <h3 className="text-lg font-semibold">No requirements found</h3>
            <p className="text-sm text-muted-foreground">
              Try adjusting your search or filter criteria.
            </p>
          </div>
        ) : (
          <div className="space-y-4">
            {filteredRequirements.map((req) => (
              <div 
                key={req.id} 
                className="border rounded-lg p-4 hover:bg-muted/50 transition-colors"
              >
                <div className="flex justify-between items-start">
                  <div className="flex-1">
                    <h3 className="font-medium text-lg mb-1">{req.title}</h3>
                    <p className="text-muted-foreground text-sm mb-3">{req.content}</p>
                    
                    <div className="flex items-center space-x-2 text-xs text-muted-foreground mb-2">
                      {req.projectArea && <span>Area: {req.projectArea}</span>}
                      <span>•</span>
                      <span>By: {req.isAnonymous ? 'Anonymous' : req.customerName}</span>
                      <span>•</span>
                      <span>Created: {new Date(req.createdAt).toLocaleDateString()}</span>
                    </div>
                    
                    <div className="flex flex-wrap gap-2">
                      {getStatusBadge(req.status)}
                      {req.priority && getPriorityBadge(req.priority)}
                      {req.category && (
                        <Badge variant="outline" className="bg-purple-100 text-purple-800 hover:bg-purple-100">
                          {req.category}
                        </Badge>
                      )}
                    </div>
                  </div>
                  
                  {req.completionPercentage !== undefined && (
                    <div className="flex flex-col items-end">
                      <div className="text-xs text-muted-foreground mb-1">
                        Progress: {req.completionPercentage}%
                      </div>
                      <div className="w-16 h-1.5 bg-muted rounded-full overflow-hidden">
                        <div 
                          className="h-full bg-primary rounded-full" 
                          style={{ width: `${req.completionPercentage}%` }} 
                        />
                      </div>
                    </div>
                  )}
                </div>
                
                {isAdmin && (
                  <div className="mt-4 flex justify-end space-x-2">
                    <Button variant="outline" size="sm">View Details</Button>
                    <Button size="sm">Update Status</Button>
                  </div>
                )}
              </div>
            ))}
          </div>
        )}
      </CardContent>
    </Card>
  );
}
