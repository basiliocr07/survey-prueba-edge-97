
import { Link } from "react-router-dom";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { BarChart3, ArrowRight, Eye } from "lucide-react";
import { useQuery } from "@tanstack/react-query";
import Navbar from "@/components/layout/Navbar";

// Sample data fetching functions - in real app these would be API calls
const fetchLatestSurvey = async () => {
  // This would be an API call in a real application
  return {
    id: "1",
    title: "Customer Satisfaction Survey",
    description: "Gather feedback about our customer service quality",
    createdAt: "2023-12-15T12:00:00Z",
    responses: 8
  };
};

const fetchLatestSuggestion = async () => {
  // This would be an API call in a real application
  return {
    id: "1",
    content: "Add dark mode to the customer portal",
    customerName: "John Doe",
    createdAt: "2023-12-10T09:30:00Z",
    status: "new"
  };
};

const fetchLatestRequirement = async () => {
  // This would be an API call in a real application
  return {
    id: "1",
    title: "Mobile responsive design",
    description: "The application needs to be fully responsive on all mobile devices",
    priority: "high",
    createdAt: "2023-12-05T14:20:00Z"
  };
};

export default function Dashboard() {
  // Fetch latest data
  const { data: latestSurvey } = useQuery({
    queryKey: ['latestSurvey'],
    queryFn: fetchLatestSurvey
  });

  const { data: latestSuggestion } = useQuery({
    queryKey: ['latestSuggestion'],
    queryFn: fetchLatestSuggestion
  });

  const { data: latestRequirement } = useQuery({
    queryKey: ['latestRequirement'],
    queryFn: fetchLatestRequirement
  });

  // Format date helper
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
        <h1 className="text-3xl font-bold tracking-tight mb-6">Dashboard</h1>
        
        {/* Minimalist container */}
        <Card className="shadow-sm">
          <CardHeader className="pb-2">
            <CardTitle className="text-xl">Overview</CardTitle>
          </CardHeader>
          <CardContent>
            {/* Latest items in list format */}
            <div className="space-y-4">
              {/* Latest Survey */}
              <div className="flex items-center justify-between border-b pb-3">
                <div>
                  <h3 className="font-medium text-sm text-muted-foreground mb-1">Latest Survey</h3>
                  <p className="font-semibold">{latestSurvey?.title || "No surveys yet"}</p>
                  {latestSurvey && (
                    <p className="text-xs text-muted-foreground">
                      Created {formatDate(latestSurvey.createdAt)} • {latestSurvey.responses} responses
                    </p>
                  )}
                </div>
                {latestSurvey && (
                  <Link to={`/survey/${latestSurvey.id}`}>
                    <Button variant="ghost" size="sm" className="h-8 w-8">
                      <Eye className="h-4 w-4" />
                    </Button>
                  </Link>
                )}
              </div>
              
              {/* Latest Suggestion */}
              <div className="flex items-center justify-between border-b pb-3">
                <div>
                  <h3 className="font-medium text-sm text-muted-foreground mb-1">Latest Suggestion</h3>
                  <p className="font-semibold">{latestSuggestion?.content || "No suggestions yet"}</p>
                  {latestSuggestion && (
                    <p className="text-xs text-muted-foreground">
                      From {latestSuggestion.customerName} • {formatDate(latestSuggestion.createdAt)}
                    </p>
                  )}
                </div>
                {latestSuggestion && (
                  <Link to="/suggestions">
                    <Button variant="ghost" size="sm" className="h-8 w-8">
                      <Eye className="h-4 w-4" />
                    </Button>
                  </Link>
                )}
              </div>
              
              {/* Latest Requirement */}
              <div className="flex items-center justify-between pb-3">
                <div>
                  <h3 className="font-medium text-sm text-muted-foreground mb-1">Latest Requirement</h3>
                  <p className="font-semibold">{latestRequirement?.title || "No requirements yet"}</p>
                  {latestRequirement && (
                    <p className="text-xs text-muted-foreground">
                      Priority: {latestRequirement.priority} • {formatDate(latestRequirement.createdAt)}
                    </p>
                  )}
                </div>
                {latestRequirement && (
                  <Link to="/requirements">
                    <Button variant="ghost" size="sm" className="h-8 w-8">
                      <Eye className="h-4 w-4" />
                    </Button>
                  </Link>
                )}
              </div>
            </div>
            
            {/* Aligned action buttons */}
            <div className="flex justify-end space-x-2 mt-6">
              <Link to="/results">
                <Button size="sm">
                  <BarChart3 className="mr-2 h-4 w-4" />
                  View Analytics
                </Button>
              </Link>
              <Link to="/surveys">
                <Button variant="outline" size="sm">
                  View All Surveys
                  <ArrowRight className="ml-2 h-4 w-4" />
                </Button>
              </Link>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
