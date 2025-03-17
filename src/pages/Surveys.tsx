
import { useState } from "react";
import { Link } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Separator } from "@/components/ui/separator";
import { FilePlus, BarChart3, Trash2, Edit, Eye } from "lucide-react";
import { useQuery } from "@tanstack/react-query";

// Sample data - in a real app this would come from an API
const fetchSurveys = async () => {
  // This would be an API call in a real application
  return [
    {
      id: "1",
      title: "Customer Satisfaction Survey",
      description: "Gather feedback about our customer service quality",
      createdAt: "2023-10-15T12:00:00Z",
      responses: 42,
      completionRate: 78
    },
    {
      id: "2",
      title: "Product Feedback Survey",
      description: "Help us improve our product offerings",
      createdAt: "2023-09-22T15:30:00Z",
      responses: 103,
      completionRate: 89
    },
    {
      id: "3",
      title: "Website Usability Survey",
      description: "Evaluate the user experience of our new website",
      createdAt: "2023-11-05T09:15:00Z",
      responses: 28,
      completionRate: 65
    }
  ];
};

export default function Surveys() {
  const [filterActive, setFilterActive] = useState<string>("all");
  
  const { data: surveys = [], isLoading } = useQuery({
    queryKey: ['surveys'],
    queryFn: fetchSurveys
  });

  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    return new Intl.DateTimeFormat('en-US', { 
      year: 'numeric', 
      month: 'short', 
      day: 'numeric' 
    }).format(date);
  };

  return (
    <div className="container mx-auto py-10 px-4 md:px-6">
      <div className="flex flex-col space-y-8">
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

        <div className="bg-white rounded-lg shadow">
          <div className="border-b p-4">
            <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
              <div className="flex items-center gap-2">
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
          </div>
          
          {isLoading ? (
            <div className="flex justify-center items-center p-12">
              <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-primary"></div>
            </div>
          ) : surveys.length > 0 ? (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 p-4">
              {surveys.map((survey) => (
                <Card key={survey.id} className="flex flex-col">
                  <CardHeader>
                    <div className="flex justify-between items-start">
                      <CardTitle className="text-lg font-semibold">{survey.title}</CardTitle>
                      <Badge variant="outline" className="bg-primary/10 text-primary">
                        {survey.responses} responses
                      </Badge>
                    </div>
                    <CardDescription className="line-clamp-2 mt-1">
                      {survey.description}
                    </CardDescription>
                  </CardHeader>
                  <CardContent className="flex-grow">
                    <div className="text-sm text-muted-foreground">
                      Created on {formatDate(survey.createdAt)}
                    </div>
                    <div className="mt-4">
                      <p className="text-sm font-medium mb-1">Completion rate</p>
                      <div className="w-full bg-secondary rounded-full h-2">
                        <div 
                          className="bg-primary h-2 rounded-full" 
                          style={{ width: `${survey.completionRate}%` }}
                        ></div>
                      </div>
                      <p className="text-xs text-right mt-1">{survey.completionRate}%</p>
                    </div>
                  </CardContent>
                  <CardFooter className="border-t pt-4 flex justify-between">
                    <div className="flex space-x-2">
                      <Button variant="ghost" size="sm">
                        <Eye className="h-4 w-4 mr-1" />
                        View
                      </Button>
                      <Button variant="ghost" size="sm">
                        <Edit className="h-4 w-4 mr-1" />
                        Edit
                      </Button>
                    </div>
                    <Button variant="ghost" size="icon" className="text-destructive hover:text-destructive">
                      <Trash2 className="h-4 w-4" />
                    </Button>
                  </CardFooter>
                </Card>
              ))}
            </div>
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
        </div>
      </div>
    </div>
  );
}
