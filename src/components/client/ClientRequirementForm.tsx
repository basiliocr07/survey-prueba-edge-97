
import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { v4 as uuidv4 } from 'uuid';
import { Search } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { Textarea } from '@/components/ui/textarea';
import { useToast } from '@/hooks/use-toast';
import { Suggestion, KnowledgeBaseItem } from '@/types/suggestions';

interface RequirementFormData {
  content: string;
  customerName: string;
  customerEmail: string;
  category?: string;
}

const categories = [
  'Feature Request', 
  'Bug Fix', 
  'Integration', 
  'Performance', 
  'Documentation', 
  'Security', 
  'Other'
];

export default function ClientRequirementForm() {
  const [isSearching, setIsSearching] = useState(false);
  const [searchResults, setSearchResults] = useState<{ requirements: Suggestion[], knowledgeItems: KnowledgeBaseItem[] }>({ requirements: [], knowledgeItems: [] });
  const { toast } = useToast();
  const { register, handleSubmit, reset, watch, formState: { errors } } = useForm<RequirementFormData>();
  
  const requirementContent = watch('content');

  const handleSearch = (content: string) => {
    if (!content || content.length < 10) return;
    
    // This is a simplified version for the client interface
    // In a real app, this would call an API endpoint to search for similar items
    setIsSearching(true);
    
    // Mock search results
    setSearchResults({
      requirements: [],
      knowledgeItems: []
    });
  };

  const onSubmit = (data: RequirementFormData) => {
    // In a real app, this would send the data to an API endpoint
    console.log('Submitting requirement:', data);
    
    toast({
      title: "Requirement submitted",
      description: "Your requirement has been successfully registered.",
    });
    reset();
    setIsSearching(false);
    setSearchResults({ requirements: [], knowledgeItems: [] });
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
      <div className="space-y-2">
        <Label htmlFor="content">Requirement Description</Label>
        <Textarea 
          id="content" 
          placeholder="Describe your requirement in detail..." 
          className="min-h-[120px]"
          {...register('content', { required: "Please provide a description" })}
          onChange={(e) => {
            if (e.target.value.length > 15) {
              handleSearch(e.target.value);
            }
          }}
        />
        {errors.content && (
          <p className="text-sm text-destructive">{errors.content.message}</p>
        )}
      </div>
      
      {isSearching && (searchResults.requirements.length > 0 || searchResults.knowledgeItems.length > 0) && (
        <div className="bg-muted p-4 rounded-md border border-border">
          <h4 className="font-medium flex items-center gap-2 mb-2">
            <Search className="h-4 w-4" />
            Similar items found
          </h4>
          
          {/* Render search results here */}
        </div>
      )}
      
      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        <div className="space-y-2">
          <Label htmlFor="customerName">Your Name</Label>
          <Input 
            id="customerName" 
            placeholder="John Doe" 
            {...register('customerName', { required: "Name is required" })}
          />
          {errors.customerName && (
            <p className="text-sm text-destructive">{errors.customerName.message}</p>
          )}
        </div>
        
        <div className="space-y-2">
          <Label htmlFor="customerEmail">Email Address</Label>
          <Input 
            id="customerEmail" 
            type="email" 
            placeholder="john@example.com" 
            {...register('customerEmail', { 
              required: "Email is required",
              pattern: {
                value: /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i,
                message: "Invalid email address"
              }
            })}
          />
          {errors.customerEmail && (
            <p className="text-sm text-destructive">{errors.customerEmail.message}</p>
          )}
        </div>
      </div>
      
      <div className="space-y-2">
        <Label htmlFor="category">Category</Label>
        <select 
          id="category"
          className="flex h-10 w-full rounded-md border border-input bg-background px-3 py-2 text-base ring-offset-background file:border-0 file:bg-transparent file:text-sm file:font-medium file:text-foreground placeholder:text-muted-foreground focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:cursor-not-allowed disabled:opacity-50 md:text-sm"
          {...register('category')}
        >
          <option value="">Select a category</option>
          {categories.map((category) => (
            <option key={category} value={category}>{category}</option>
          ))}
        </select>
      </div>
      
      <Button type="submit" className="w-full">Submit Requirement</Button>
    </form>
  );
}
