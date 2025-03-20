
import { useState, useEffect } from 'react';
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
import { 
  Search, 
  Filter, 
  Flag, 
  CalendarClock, 
  Layers, 
  ArrowUpRight 
} from 'lucide-react';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
  DialogFooter
} from "@/components/ui/dialog";
import { Textarea } from "@/components/ui/textarea";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Requirement } from '@/types/requirements';
import { format } from 'date-fns';
import { useToast } from "@/hooks/use-toast";

interface AdvancedRequirementsListProps {
  requirements: Requirement[];
  isAdmin: boolean;
}

export default function AdvancedRequirementsList({ requirements, isAdmin }: AdvancedRequirementsListProps) {
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState('');
  const [priorityFilter, setPriorityFilter] = useState('');
  const [categoryFilter, setCategoryFilter] = useState('');
  const [projectAreaFilter, setProjectAreaFilter] = useState('');
  const [selectedRequirement, setSelectedRequirement] = useState<Requirement | null>(null);
  const [responseText, setResponseText] = useState('');
  const [selectedStatus, setSelectedStatus] = useState('');
  const [completionPercentage, setCompletionPercentage] = useState(0);
  const [dialogOpen, setDialogOpen] = useState(false);
  const { toast } = useToast();
  
  console.log("AdvancedRequirementsList - Requirements:", requirements);
  console.log("AdvancedRequirementsList - Requirements length:", requirements?.length || 0);

  useEffect(() => {
    if (requirements?.length > 0) {
      console.log("Requirements loaded successfully:", requirements.length);
    } else {
      console.log("No requirements data available or empty array received");
    }
  }, [requirements]);

  // Si requirements es undefined, establecerlo como un array vacío
  const safeRequirements = requirements || [];
  
  // Get unique categories from requirements
  const categories = [...new Set(safeRequirements.map(req => req.category).filter(Boolean))];
  
  // Get unique project areas from requirements
  const projectAreas = [...new Set(safeRequirements.map(req => req.projectArea).filter(Boolean))];
  
  // Filter requirements based on search term and selected filters
  const filteredRequirements = safeRequirements.filter(req => {
    const matchesSearch = 
      searchTerm === '' ||
      req.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
      (req.content && req.content.toLowerCase().includes(searchTerm.toLowerCase())) ||
      (req.description && req.description.toLowerCase().includes(searchTerm.toLowerCase())) ||
      (req.customerName && req.customerName.toLowerCase().includes(searchTerm.toLowerCase()));
    
    const matchesStatus = statusFilter ? req.status === statusFilter : true;
    const matchesPriority = priorityFilter ? req.priority === priorityFilter : true;
    const matchesCategory = categoryFilter ? req.category === categoryFilter : true;
    const matchesProjectArea = projectAreaFilter ? req.projectArea === projectAreaFilter : true;
    
    return matchesSearch && matchesStatus && matchesPriority && matchesCategory && matchesProjectArea;
  });
  
  console.log("Filtered requirements:", filteredRequirements);
  
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

  const handleViewRequirement = (requirement: Requirement) => {
    console.log("Selected requirement:", requirement);
    setSelectedRequirement(requirement);
    setResponseText(requirement.response || '');
    setSelectedStatus(requirement.status);
    setCompletionPercentage(requirement.completionPercentage || 0);
    setDialogOpen(true);
  };

  const handleUpdateStatus = async () => {
    if (!selectedRequirement) return;
    
    try {
      console.log('Updating requirement status:', {
        id: selectedRequirement.id,
        status: selectedStatus,
        response: responseText,
        completionPercentage
      });
      
      toast({
        title: "Status updated",
        description: "The requirement status has been updated successfully.",
      });
      
      setDialogOpen(false);
      setSelectedRequirement(null);
    } catch (error) {
      toast({
        title: "Error updating status",
        description: "There was a problem updating the requirement status.",
        variant: "destructive"
      });
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

          {projectAreas.length > 0 && (
            <Select value={projectAreaFilter} onValueChange={setProjectAreaFilter}>
              <SelectTrigger className="w-[160px]">
                <div className="flex items-center">
                  <Layers className="mr-2 h-4 w-4" />
                  <SelectValue placeholder="Project Area" />
                </div>
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="">All areas</SelectItem>
                {projectAreas.map((area) => (
                  <SelectItem key={area} value={area as string}>
                    {area}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          )}
        </div>
      </CardHeader>
      
      <CardContent>
        {safeRequirements.length === 0 ? (
          <div className="flex flex-col items-center justify-center py-8 text-center">
            <div className="rounded-full bg-muted p-3 mb-3">
              <Search className="h-6 w-6 text-muted-foreground" />
            </div>
            <h3 className="text-lg font-semibold">No requirements data available</h3>
            <p className="text-sm text-muted-foreground">
              There are no requirements to display. Please check your data source.
            </p>
          </div>
        ) : filteredRequirements.length === 0 ? (
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
                    <p className="text-muted-foreground text-sm mb-3">{req.content || req.description}</p>
                    
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
                    <Button 
                      variant="outline" 
                      size="sm"
                      onClick={() => handleViewRequirement(req)}
                    >
                      View Details
                    </Button>
                    <Button 
                      size="sm"
                      onClick={() => handleViewRequirement(req)}
                    >
                      Update Status
                    </Button>
                  </div>
                )}
              </div>
            ))}
          </div>
        )}
      </CardContent>

      <Dialog open={dialogOpen} onOpenChange={setDialogOpen}>
        <DialogContent className="max-w-3xl">
          <DialogHeader>
            <DialogTitle className="flex items-center gap-2">
              <ArrowUpRight className="h-5 w-5" />
              Requirement Details
            </DialogTitle>
            <DialogDescription>
              View and manage this requirement
            </DialogDescription>
          </DialogHeader>
          
          {selectedRequirement && (
            <Tabs defaultValue="details" className="mt-4">
              <TabsList className="grid w-full grid-cols-2">
                <TabsTrigger value="details">Details</TabsTrigger>
                <TabsTrigger value="response">Update Status</TabsTrigger>
              </TabsList>
              
              <TabsContent value="details" className="space-y-4">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <h4 className="text-sm font-medium mb-1">Submitted by</h4>
                    <p>{selectedRequirement.isAnonymous ? 'Anonymous' : selectedRequirement.customerName}</p>
                    {!selectedRequirement.isAnonymous && (
                      <p className="text-sm text-muted-foreground">{selectedRequirement.customerEmail}</p>
                    )}
                  </div>
                  
                  <div>
                    <h4 className="text-sm font-medium mb-1">Date</h4>
                    <p>{selectedRequirement.createdAt && format(new Date(selectedRequirement.createdAt), 'PPP')}</p>
                  </div>
                  
                  <div>
                    <h4 className="text-sm font-medium mb-1">Category</h4>
                    <p>{selectedRequirement.category || 'Not categorized'}</p>
                  </div>
                  
                  <div>
                    <h4 className="text-sm font-medium mb-1">Status</h4>
                    {getStatusBadge(selectedRequirement.status)}
                  </div>

                  <div>
                    <h4 className="text-sm font-medium mb-1">Priority</h4>
                    {selectedRequirement.priority ? getPriorityBadge(selectedRequirement.priority) : 'Not set'}
                  </div>

                  <div>
                    <h4 className="text-sm font-medium mb-1">Project Area</h4>
                    <p>{selectedRequirement.projectArea || 'Not specified'}</p>
                  </div>
                </div>
                
                <div>
                  <h4 className="text-sm font-medium mb-1">Description</h4>
                  <div className="p-4 bg-muted rounded-md">
                    {selectedRequirement.content || selectedRequirement.description}
                  </div>
                </div>

                {selectedRequirement.acceptanceCriteria && (
                  <div>
                    <h4 className="text-sm font-medium mb-1">Acceptance Criteria</h4>
                    <div className="p-4 bg-muted rounded-md">
                      {selectedRequirement.acceptanceCriteria}
                    </div>
                  </div>
                )}

                {selectedRequirement.response && (
                  <div>
                    <h4 className="text-sm font-medium mb-1">Response</h4>
                    <div className="p-4 bg-muted rounded-md">
                      {selectedRequirement.response}
                    </div>
                  </div>
                )}
              </TabsContent>
              
              <TabsContent value="response" className="space-y-4">
                <div className="space-y-2">
                  <h4 className="text-sm font-medium">Status</h4>
                  <Select value={selectedStatus} onValueChange={setSelectedStatus}>
                    <SelectTrigger>
                      <SelectValue placeholder="Select status" />
                    </SelectTrigger>
                    <SelectContent>
                      <SelectItem value="proposed">Proposed</SelectItem>
                      <SelectItem value="in-progress">In Progress</SelectItem>
                      <SelectItem value="implemented">Implemented</SelectItem>
                      <SelectItem value="rejected">Rejected</SelectItem>
                    </SelectContent>
                  </Select>
                </div>
                
                <div className="space-y-2">
                  <h4 className="text-sm font-medium">Completion Percentage</h4>
                  <div className="flex items-center gap-4">
                    <Input
                      type="number"
                      min="0"
                      max="100"
                      value={completionPercentage}
                      onChange={(e) => setCompletionPercentage(parseInt(e.target.value) || 0)}
                    />
                    <span>%</span>
                  </div>
                </div>
                
                <div className="space-y-2">
                  <h4 className="text-sm font-medium">Response</h4>
                  <Textarea 
                    placeholder="Enter response..."
                    value={responseText}
                    onChange={(e) => setResponseText(e.target.value)}
                    rows={5}
                  />
                </div>
                
                <Button onClick={handleUpdateStatus} className="w-full">
                  Update Status
                </Button>
              </TabsContent>
            </Tabs>
          )}
        </DialogContent>
      </Dialog>
    </Card>
  );
}
