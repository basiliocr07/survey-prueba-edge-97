
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Requirement } from "@/types/requirements";
import { CalendarIcon, ClockIcon, UserIcon, ArrowRightIcon, FileText } from "lucide-react";
import { Separator } from "@/components/ui/separator";

interface RequirementDetailsDialogProps {
  requirement: Requirement | null;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export default function RequirementDetailsDialog({
  requirement,
  open,
  onOpenChange,
}: RequirementDetailsDialogProps) {
  if (!requirement) {
    return null;
  }

  // Format date
  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString(undefined, {
      year: "numeric",
      month: "long",
      day: "numeric",
    });
  };

  // Function to determine status badge color
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

  // Function to determine priority badge color
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
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent className="max-w-2xl">
        <DialogHeader>
          <DialogTitle className="text-xl font-bold">{requirement.title}</DialogTitle>
          <DialogDescription className="flex items-center gap-2 pt-2">
            {getStatusBadge(requirement.status)}
            {requirement.priority && getPriorityBadge(requirement.priority)}
            {requirement.category && (
              <Badge variant="outline" className="bg-purple-100 text-purple-800">
                {requirement.category}
              </Badge>
            )}
          </DialogDescription>
        </DialogHeader>
        
        <div className="space-y-4">
          <div className="bg-muted/40 p-4 rounded-md">
            <h3 className="font-medium text-sm mb-2 flex items-center gap-2">
              <FileText className="h-4 w-4" /> Description
            </h3>
            <p className="text-muted-foreground">{requirement.content}</p>
          </div>
          
          {requirement.acceptanceCriteria && (
            <div className="bg-muted/40 p-4 rounded-md">
              <h3 className="font-medium text-sm mb-2">Acceptance Criteria</h3>
              <p className="text-muted-foreground">{requirement.acceptanceCriteria}</p>
            </div>
          )}
          
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            <div>
              <h3 className="font-medium text-sm mb-2 flex items-center gap-2">
                <UserIcon className="h-4 w-4" /> Submitted By
              </h3>
              <p className="text-muted-foreground">
                {requirement.isAnonymous ? "Anonymous" : requirement.customerName}
              </p>
              {!requirement.isAnonymous && (
                <p className="text-xs text-muted-foreground">{requirement.customerEmail}</p>
              )}
            </div>
            
            <div>
              <h3 className="font-medium text-sm mb-2 flex items-center gap-2">
                <CalendarIcon className="h-4 w-4" /> Date Information
              </h3>
              <p className="text-muted-foreground text-sm">
                Created: {formatDate(requirement.createdAt)}
              </p>
              {requirement.targetDate && (
                <p className="text-muted-foreground text-sm">
                  Target completion: {formatDate(requirement.targetDate)}
                </p>
              )}
            </div>
          </div>
          
          {requirement.projectArea && (
            <div>
              <h3 className="font-medium text-sm mb-1">Project Area</h3>
              <p className="text-muted-foreground">{requirement.projectArea}</p>
            </div>
          )}
          
          {requirement.response && (
            <>
              <Separator />
              <div>
                <h3 className="font-medium text-sm mb-2 flex items-center gap-2">
                  <ArrowRightIcon className="h-4 w-4" /> Response
                </h3>
                <p className="text-muted-foreground">{requirement.response}</p>
                {requirement.responseDate && (
                  <p className="text-xs text-muted-foreground mt-1">
                    Responded on {formatDate(requirement.responseDate)}
                  </p>
                )}
              </div>
            </>
          )}
          
          {requirement.completionPercentage !== undefined && (
            <div>
              <h3 className="font-medium text-sm mb-2 flex items-center gap-2">
                <ClockIcon className="h-4 w-4" /> Progress
              </h3>
              <div className="flex items-center gap-2">
                <div className="w-full h-2 bg-muted rounded-full overflow-hidden">
                  <div 
                    className="h-full bg-primary rounded-full" 
                    style={{ width: `${requirement.completionPercentage}%` }} 
                  />
                </div>
                <span className="text-sm font-medium">{requirement.completionPercentage}%</span>
              </div>
            </div>
          )}
        </div>
        
        <DialogFooter>
          <Button variant="outline" onClick={() => onOpenChange(false)}>Close</Button>
          {/* Additional action buttons could go here */}
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
