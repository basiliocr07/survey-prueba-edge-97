
import { Link } from "react-router-dom";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { BarChart3, ArrowRight, Eye, Clock, LineChart, CheckCircle2 } from "lucide-react";
import { useQuery } from "@tanstack/react-query";
import Navbar from "@/components/layout/Navbar";
import { Badge } from "@/components/ui/badge";

// Sample data fetching functions - in real app these would be API calls
const fetchLatestSurvey = async () => {
  // This would be an API call in a real application
  return {
    id: "1",
    title: "Customer Satisfaction Survey",
    description: "Gather feedback about our customer service quality",
    createdAt: "2023-12-15T12:00:00Z",
    responses: 8,
    status: "in-progress" // Added status
  };
};

const fetchLatestSuggestion = async () => {
  // This would be an API call in a real application
  return {
    id: "1",
    content: "Add dark mode to the customer portal",
    customerName: "John Doe",
    createdAt: "2023-12-10T09:30:00Z",
    status: "pending" // Changed from "new" to match our status system
  };
};

const fetchLatestRequirement = async () => {
  // This would be an API call in a real application
  return {
    id: "1",
    title: "Mobile responsive design",
    description: "The application needs to be fully responsive on all mobile devices",
    priority: "high",
    createdAt: "2023-12-05T14:20:00Z",
    status: "closed" // Added status
  };
};

// Helper function to render status badge with appropriate color and icon
const StatusBadge = ({ status }) => {
  switch (status) {
    case "pending":
      return (
        <Badge variant="outline" className="bg-amber-50 text-amber-700 border-amber-200">
          <Clock className="h-3 w-3 mr-1" /> Pendiente
        </Badge>
      );
    case "in-progress":
      return (
        <Badge variant="outline" className="bg-blue-50 text-blue-700 border-blue-200">
          <LineChart className="h-3 w-3 mr-1" /> En curso
        </Badge>
      );
    case "closed":
      return (
        <Badge variant="outline" className="bg-green-50 text-green-700 border-green-200">
          <CheckCircle2 className="h-3 w-3 mr-1" /> Cerrada
        </Badge>
      );
    default:
      return null;
  }
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
            <CardTitle className="text-xl">Vista General</CardTitle>
          </CardHeader>
          <CardContent>
            {/* Latest items in list format */}
            <div className="space-y-4">
              {/* Latest Survey */}
              <div className="flex items-start justify-between border-b pb-3">
                <div>
                  <div className="flex items-center gap-2 mb-1">
                    <h3 className="font-medium text-sm text-muted-foreground">Última Encuesta</h3>
                    {latestSurvey && <StatusBadge status={latestSurvey.status} />}
                  </div>
                  <p className="font-semibold">{latestSurvey?.title || "No surveys yet"}</p>
                  {latestSurvey && (
                    <p className="text-xs text-muted-foreground">
                      Creada {formatDate(latestSurvey.createdAt)} • {latestSurvey.responses} respuestas
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
              <div className="flex items-start justify-between border-b pb-3">
                <div>
                  <div className="flex items-center gap-2 mb-1">
                    <h3 className="font-medium text-sm text-muted-foreground">Última Sugerencia</h3>
                    {latestSuggestion && <StatusBadge status={latestSuggestion.status} />}
                  </div>
                  <p className="font-semibold">{latestSuggestion?.content || "No suggestions yet"}</p>
                  {latestSuggestion && (
                    <p className="text-xs text-muted-foreground">
                      De {latestSuggestion.customerName} • {formatDate(latestSuggestion.createdAt)}
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
              <div className="flex items-start justify-between pb-3">
                <div>
                  <div className="flex items-center gap-2 mb-1">
                    <h3 className="font-medium text-sm text-muted-foreground">Último Requerimiento</h3>
                    {latestRequirement && <StatusBadge status={latestRequirement.status} />}
                  </div>
                  <p className="font-semibold">{latestRequirement?.title || "No requirements yet"}</p>
                  {latestRequirement && (
                    <p className="text-xs text-muted-foreground">
                      Prioridad: {latestRequirement.priority} • {formatDate(latestRequirement.createdAt)}
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
                  Ver Análisis
                </Button>
              </Link>
              <Link to="/surveys">
                <Button variant="outline" size="sm">
                  Ver Todas las Encuestas
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
