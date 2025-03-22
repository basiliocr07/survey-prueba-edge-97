
import { useState } from "react";
import { Link } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { FilePlus, Edit, Eye, Trash2 } from "lucide-react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import Navbar from "@/components/layout/Navbar";
import { supabase } from "@/integrations/supabase/client";
import { useToast } from "@/hooks/use-toast";
import { Json } from "@/integrations/supabase/types";

// Define the Survey interface based on the Supabase schema
interface Survey {
  id: string;
  title: string;
  description?: string | null;
  questions: Json; // Changed from any[] to Json to match Supabase schema
  created_at: string;
  updated_at: string;
  delivery_config?: Json | null;
  responses?: number; // Client-side calculated metric
  completionRate?: number; // Client-side calculated metric
}

// Function to fetch surveys from Supabase
const fetchSurveys = async (): Promise<Survey[]> => {
  const { data, error } = await supabase
    .from('surveys')
    .select('*');
  
  if (error) {
    throw new Error(error.message);
  }
  
  // Transform the data to include response metrics
  return data.map(survey => ({
    ...survey,
    responses: Math.floor(Math.random() * 100), // Placeholder for demo
    completionRate: Math.floor(Math.random() * 100) // Placeholder for demo
  }));
};

// Function to delete a survey
const deleteSurvey = async (id: string) => {
  const { error } = await supabase
    .from('surveys')
    .delete()
    .eq('id', id);
  
  if (error) {
    throw new Error(error.message);
  }
  
  return true;
};

export default function Surveys() {
  const [filterActive, setFilterActive] = useState<string>("all");
  const { toast } = useToast();
  const queryClient = useQueryClient();
  
  const { data: surveys = [], isLoading } = useQuery({
    queryKey: ['surveys'],
    queryFn: fetchSurveys
  });

  const deleteMutation = useMutation({
    mutationFn: deleteSurvey,
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['surveys'] });
      toast({
        title: "Survey deleted",
        description: "The survey has been successfully deleted.",
      });
    },
    onError: (error) => {
      toast({
        title: "Error",
        description: `Failed to delete survey: ${error instanceof Error ? error.message : 'Unknown error'}`,
        variant: "destructive",
      });
    }
  });

  const handleDeleteSurvey = (id: string) => {
    if (window.confirm("Are you sure you want to delete this survey?")) {
      deleteMutation.mutate(id);
    }
  };

  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    return new Intl.DateTimeFormat('en-US', { 
      year: 'numeric', 
      month: 'short', 
      day: 'numeric' 
    }).format(date);
  };

  return (
    <div className="min-h-screen bg-background">
      <Navbar />
      <div className="container mx-auto pt-20 pb-10 px-4 md:px-6">
        <div className="flex flex-col space-y-6">
          <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
            <div>
              <h1 className="text-3xl font-bold tracking-tight">Survey Management</h1>
              <p className="text-muted-foreground mt-1">Create and manage your surveys</p>
            </div>
            
            <div className="flex items-center gap-2">
              <Link to="/create">
                <Button size="sm">
                  <FilePlus className="mr-2 h-4 w-4" />
                  Create Survey
                </Button>
              </Link>
            </div>
          </div>

          <Card className="shadow-sm">
            <div className="border-b p-3">
              <div className="flex gap-2 overflow-x-auto pb-1">
                <Button 
                  variant={filterActive === "all" ? "default" : "outline"} 
                  size="sm"
                  onClick={() => setFilterActive("all")}
                >
                  All Surveys
                </Button>
                <Button 
                  variant={filterActive === "active" ? "default" : "outline"} 
                  size="sm"
                  onClick={() => setFilterActive("active")}
                >
                  Active
                </Button>
                <Button 
                  variant={filterActive === "draft" ? "default" : "outline"} 
                  size="sm"
                  onClick={() => setFilterActive("draft")}
                >
                  Draft
                </Button>
                <Button 
                  variant={filterActive === "archived" ? "default" : "outline"} 
                  size="sm"
                  onClick={() => setFilterActive("archived")}
                >
                  Archived
                </Button>
              </div>
            </div>
            
            {isLoading ? (
              <div className="flex justify-center items-center p-12">
                <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary"></div>
              </div>
            ) : surveys.length > 0 ? (
              <CardContent className="p-0">
                <ul className="divide-y">
                  {surveys.map((survey) => (
                    <li key={survey.id} className="p-4 hover:bg-accent/20 transition-colors">
                      <div className="flex items-center justify-between">
                        <div className="flex-grow">
                          <div className="flex items-center justify-between mb-1">
                            <h3 className="font-medium">{survey.title}</h3>
                            <Badge variant="outline" className="bg-primary/10 text-primary text-xs">
                              {survey.responses} responses
                            </Badge>
                          </div>
                          <p className="text-xs text-muted-foreground line-clamp-1 mb-1">
                            {survey.description}
                          </p>
                          <div className="flex items-center text-xs text-muted-foreground">
                            <span>Created {formatDate(survey.created_at)}</span>
                            <span className="mx-2">•</span>
                            <div className="flex items-center">
                              <div className="w-16 bg-secondary rounded-full h-1.5 mr-1">
                                <div 
                                  className="bg-primary h-1.5 rounded-full" 
                                  style={{ width: `${survey.completionRate}%` }}
                                ></div>
                              </div>
                              <span>{survey.completionRate}% completed</span>
                            </div>
                          </div>
                        </div>
                        <div className="flex space-x-1 ml-4">
                          <Link to={`/survey/${survey.id}`}>
                            <Button variant="ghost" size="icon" className="h-8 w-8">
                              <Eye className="h-4 w-4" />
                            </Button>
                          </Link>
                          <Link to={`/create?edit=${survey.id}`}>
                            <Button variant="ghost" size="icon" className="h-8 w-8">
                              <Edit className="h-4 w-4" />
                            </Button>
                          </Link>
                          <Button 
                            variant="ghost" 
                            size="icon" 
                            className="h-8 w-8 text-destructive hover:text-destructive"
                            onClick={() => handleDeleteSurvey(survey.id)}
                            disabled={deleteMutation.isPending}
                          >
                            <Trash2 className="h-4 w-4" />
                          </Button>
                        </div>
                      </div>
                    </li>
                  ))}
                </ul>
              </CardContent>
            ) : (
              <div className="flex flex-col items-center justify-center p-12 text-center">
                <div className="rounded-full bg-primary/10 p-4 mb-4">
                  <FilePlus className="h-6 w-6 text-primary" />
                </div>
                <h3 className="text-lg font-semibold mb-1">No surveys found</h3>
                <p className="text-muted-foreground mb-4">Get started by creating your first survey</p>
                <Link to="/create">
                  <Button>
                    <FilePlus className="mr-2 h-4 w-4" />
                    Create Survey
                  </Button>
                </Link>
              </div>
            )}
          </Card>
        </div>
      </div>
    </div>
  );
}
