
import { useState, useEffect } from 'react';
import { format } from 'date-fns';
import { 
  Card, 
  CardContent, 
  CardDescription, 
  CardHeader, 
  CardTitle 
} from "@/components/ui/card";
import { 
  Table, 
  TableBody, 
  TableCell, 
  TableHead, 
  TableHeader, 
  TableRow 
} from "@/components/ui/table";
import { 
  Select, 
  SelectContent, 
  SelectItem, 
  SelectTrigger, 
  SelectValue 
} from "@/components/ui/select";
import { Input } from "@/components/ui/input";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Suggestion } from '@/types/suggestions';
import { 
  Calendar, 
  Search, 
  Filter, 
  MessageSquare, 
  CheckCircle, 
  AlertCircle,
  Clock 
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
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Textarea } from "@/components/ui/textarea";
import { useToast } from '@/hooks/use-toast';

interface EnhancedSuggestionsListProps {
  suggestions: Suggestion[];
  isAdmin?: boolean;
}

export default function EnhancedSuggestionsList({ suggestions, isAdmin = false }: EnhancedSuggestionsListProps) {
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState<string>('');
  const [categoryFilter, setCategoryFilter] = useState<string>('');
  const [selectedSuggestion, setSelectedSuggestion] = useState<Suggestion | null>(null);
  const [responseText, setResponseText] = useState('');
  const [selectedStatus, setSelectedStatus] = useState('');
  const { toast } = useToast();

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'new':
        return 'bg-blue-500';
      case 'reviewed':
        return 'bg-yellow-500';
      case 'implemented':
        return 'bg-green-500';
      case 'rejected':
        return 'bg-red-500';
      default:
        return 'bg-gray-500';
    }
  };

  const getStatusIcon = (status: string) => {
    switch (status) {
      case 'new':
        return <Clock className="h-4 w-4" />;
      case 'reviewed':
        return <MessageSquare className="h-4 w-4" />;
      case 'implemented':
        return <CheckCircle className="h-4 w-4" />;
      case 'rejected':
        return <AlertCircle className="h-4 w-4" />;
      default:
        return <Clock className="h-4 w-4" />;
    }
  };

  const filteredSuggestions = suggestions.filter((suggestion) => {
    const matchesSearch = searchTerm === '' || 
      suggestion.content.toLowerCase().includes(searchTerm.toLowerCase()) ||
      (suggestion.customerName && suggestion.customerName.toLowerCase().includes(searchTerm.toLowerCase()));
    
    const matchesStatus = statusFilter === '' || suggestion.status === statusFilter;
    
    const matchesCategory = categoryFilter === '' || suggestion.category === categoryFilter;
    
    return matchesSearch && matchesStatus && matchesCategory;
  });

  const categories = Array.from(new Set(suggestions.map(s => s.category).filter(Boolean))) as string[];
  const statuses = Array.from(new Set(suggestions.map(s => s.status)));

  const handleViewSuggestion = (suggestion: Suggestion) => {
    setSelectedSuggestion(suggestion);
    setResponseText(suggestion.response || '');
    setSelectedStatus(suggestion.status);
  };

  const handleUpdateStatus = async () => {
    if (!selectedSuggestion) return;
    
    try {
      console.log('Updating suggestion status:', {
        id: selectedSuggestion.id,
        status: selectedStatus,
        response: responseText
      });
      
      toast({
        title: "Status updated",
        description: "The suggestion status has been updated successfully.",
      });
      
      setSelectedSuggestion(null);
      setResponseText('');
      setSelectedStatus('');
    } catch (error) {
      toast({
        title: "Error updating status",
        description: "There was a problem updating the suggestion status.",
        variant: "destructive"
      });
    }
  };

  return (
    <Card>
      <CardHeader>
        <CardTitle className="flex items-center gap-2">
          <MessageSquare className="h-5 w-5" />
          Customer Suggestions
        </CardTitle>
        <CardDescription>
          Browse and manage customer feedback and suggestions
        </CardDescription>
        
        <div className="flex flex-col md:flex-row gap-4 mt-4">
          <div className="relative flex-grow">
            <Search className="absolute left-3 top-3 h-4 w-4 text-muted-foreground" />
            <Input
              placeholder="Search suggestions..."
              className="pl-10"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
            />
          </div>
          
          <div className="flex gap-2">
            <Select value={statusFilter} onValueChange={setStatusFilter}>
              <SelectTrigger className="w-[140px]">
                <Filter className="mr-2 h-4 w-4" />
                <SelectValue placeholder="Status" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">All Statuses</SelectItem>
                {statuses.map((status) => (
                  <SelectItem key={status} value={status}>
                    {status.charAt(0).toUpperCase() + status.slice(1)}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
            
            <Select value={categoryFilter} onValueChange={setCategoryFilter}>
              <SelectTrigger className="w-[140px]">
                <Filter className="mr-2 h-4 w-4" />
                <SelectValue placeholder="Category" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">All Categories</SelectItem>
                {categories.map((category) => (
                  <SelectItem key={category} value={category || "uncategorized"}>
                    {category || "Uncategorized"}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>
        </div>
      </CardHeader>
      
      <CardContent>
        {filteredSuggestions.length === 0 ? (
          <div className="text-center py-8 text-muted-foreground">
            No suggestions found matching your filters
          </div>
        ) : (
          <div className="overflow-x-auto">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead>Suggestion</TableHead>
                  <TableHead>Customer</TableHead>
                  <TableHead>Date</TableHead>
                  <TableHead>Category</TableHead>
                  <TableHead>Status</TableHead>
                  {isAdmin && <TableHead className="text-right">Actions</TableHead>}
                </TableRow>
              </TableHeader>
              <TableBody>
                {filteredSuggestions.map((suggestion) => (
                  <TableRow key={suggestion.id}>
                    <TableCell className="max-w-md">
                      <div className="font-medium">{suggestion.title || suggestion.content.substring(0, 50)}</div>
                      <div className="text-sm text-muted-foreground line-clamp-2">
                        {suggestion.content}
                      </div>
                    </TableCell>
                    <TableCell>
                      {suggestion.isAnonymous ? 'Anonymous' : suggestion.customerName}
                    </TableCell>
                    <TableCell className="whitespace-nowrap">
                      <div className="flex items-center">
                        <Calendar className="mr-2 h-4 w-4 text-muted-foreground" />
                        {suggestion.createdAt && format(new Date(suggestion.createdAt), 'MMM d, yyyy')}
                      </div>
                    </TableCell>
                    <TableCell>
                      {suggestion.category && (
                        <Badge variant="outline">{suggestion.category}</Badge>
                      )}
                    </TableCell>
                    <TableCell>
                      <Badge 
                        className={`flex items-center gap-1 ${getStatusColor(suggestion.status)} text-white`}
                      >
                        {getStatusIcon(suggestion.status)}
                        {suggestion.status.charAt(0).toUpperCase() + suggestion.status.slice(1)}
                      </Badge>
                    </TableCell>
                    {isAdmin && (
                      <TableCell className="text-right">
                        <Dialog>
                          <DialogTrigger asChild>
                            <Button 
                              variant="ghost" 
                              size="sm"
                              onClick={() => handleViewSuggestion(suggestion)}
                            >
                              View
                            </Button>
                          </DialogTrigger>
                          <DialogContent className="max-w-3xl">
                            <DialogHeader>
                              <DialogTitle className="flex items-center gap-2">
                                <MessageSquare className="h-5 w-5" />
                                Suggestion Details
                              </DialogTitle>
                              <DialogDescription>
                                View and manage this customer suggestion
                              </DialogDescription>
                            </DialogHeader>
                            
                            {selectedSuggestion && (
                              <Tabs defaultValue="details" className="mt-4">
                                <TabsList className="grid w-full grid-cols-2">
                                  <TabsTrigger value="details">Details</TabsTrigger>
                                  <TabsTrigger value="response">Response</TabsTrigger>
                                </TabsList>
                                
                                <TabsContent value="details" className="space-y-4">
                                  <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                                    <div>
                                      <h4 className="text-sm font-medium mb-1">Submitted by</h4>
                                      <p>{selectedSuggestion.isAnonymous ? 'Anonymous' : selectedSuggestion.customerName}</p>
                                      {!selectedSuggestion.isAnonymous && (
                                        <p className="text-sm text-muted-foreground">{selectedSuggestion.customerEmail}</p>
                                      )}
                                    </div>
                                    
                                    <div>
                                      <h4 className="text-sm font-medium mb-1">Date</h4>
                                      <p>{selectedSuggestion.createdAt && format(new Date(selectedSuggestion.createdAt), 'PPP')}</p>
                                    </div>
                                    
                                    <div>
                                      <h4 className="text-sm font-medium mb-1">Category</h4>
                                      <p>{selectedSuggestion.category || 'Not categorized'}</p>
                                    </div>
                                    
                                    <div>
                                      <h4 className="text-sm font-medium mb-1">Status</h4>
                                      <Badge 
                                        className={`${getStatusColor(selectedSuggestion.status)} text-white`}
                                      >
                                        {selectedSuggestion.status.charAt(0).toUpperCase() + selectedSuggestion.status.slice(1)}
                                      </Badge>
                                    </div>
                                  </div>
                                  
                                  <div>
                                    <h4 className="text-sm font-medium mb-1">Suggestion</h4>
                                    <div className="p-4 bg-muted rounded-md">
                                      {selectedSuggestion.content}
                                    </div>
                                  </div>
                                </TabsContent>
                                
                                <TabsContent value="response" className="space-y-4">
                                  <div className="space-y-2">
                                    <h4 className="text-sm font-medium">Status</h4>
                                    <Select value={selectedStatus} onValueChange={setSelectedStatus}>
                                      <SelectTrigger>
                                        <SelectValue placeholder="Select status" />
                                      </SelectTrigger>
                                      <SelectContent>
                                        <SelectItem value="new">New</SelectItem>
                                        <SelectItem value="reviewed">Reviewed</SelectItem>
                                        <SelectItem value="implemented">Implemented</SelectItem>
                                        <SelectItem value="rejected">Rejected</SelectItem>
                                      </SelectContent>
                                    </Select>
                                  </div>
                                  
                                  <div className="space-y-2">
                                    <h4 className="text-sm font-medium">Response</h4>
                                    <Textarea 
                                      placeholder="Enter response to customer..."
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
                      </TableCell>
                    )}
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </div>
        )}
      </CardContent>
    </Card>
  );
}
