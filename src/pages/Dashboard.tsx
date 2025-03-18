
import { Link } from "react-router-dom";
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { BarChart3, ArrowRight, Eye, LineChart, Clock, CheckCircle2, AlertTriangle } from "lucide-react";
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

// New function to fetch recent surveys by status
const fetchRecentSurveys = async () => {
  // This would be an API call in a real application
  return [
    {
      id: "1",
      title: "Customer Satisfaction Survey",
      status: "In Progress",
      responses: 8,
      createdAt: "2023-12-15T12:00:00Z"
    },
    {
      id: "2",
      title: "Product Feedback Survey",
      status: "Pending",
      responses: 0,
      createdAt: "2023-12-16T10:00:00Z"
    },
    {
      id: "3",
      title: "Website Usability Survey",
      status: "Closed",
      responses: 24,
      createdAt: "2023-12-01T09:00:00Z"
    },
    {
      id: "4",
      title: "Employee Engagement Survey",
      status: "In Progress",
      responses: 12,
      createdAt: "2023-12-10T14:30:00Z"
    },
    {
      id: "5",
      title: "Training Effectiveness Survey",
      status: "Pending",
      responses: 0,
      createdAt: "2023-12-17T16:45:00Z"
    }
  ];
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

  const { data: recentSurveys } = useQuery({
    queryKey: ['recentSurveys'],
    queryFn: fetchRecentSurveys
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

  // Get status badge color
  const getStatusBadge = (status: string) => {
    switch (status) {
      case 'Pending':
        return <Badge variant="outline" className="bg-yellow-100 text-yellow-800 border-yellow-300">
          <Clock className="h-3 w-3 mr-1" /> Pending
        </Badge>;
      case 'In Progress':
        return <Badge variant="outline" className="bg-blue-100 text-blue-800 border-blue-300">
          <LineChart className="h-3 w-3 mr-1" /> In Progress
        </Badge>;
      case 'Closed':
        return <Badge variant="outline" className="bg-green-100 text-green-800 border-green-300">
          <CheckCircle2 className="h-3 w-3 mr-1" /> Closed
        </Badge>;
      default:
        return <Badge variant="outline" className="bg-gray-100 text-gray-800 border-gray-300">
          <AlertTriangle className="h-3 w-3 mr-1" /> {status}
        </Badge>;
    }
  };

  // Group surveys by status
  const surveysByStatus = recentSurveys ? {
    'Pending': recentSurveys.filter(s => s.status === 'Pending'),
    'In Progress': recentSurveys.filter(s => s.status === 'In Progress'),
    'Closed': recentSurveys.filter(s => s.status === 'Closed')
  } : { 'Pending': [], 'In Progress': [], 'Closed': [] };

  return (
    <div className="min-h-screen bg-background">
      <Navbar />
      <div className="container mx-auto pt-20 pb-10 px-4 md:px-6">
        <h1 className="text-3xl font-bold tracking-tight mb-6">Dashboard</h1>
        
        {/* Survey status overview */}
        <Card className="shadow-sm mb-8">
          <CardHeader className="pb-2">
            <CardTitle className="text-xl">Vista rápida de encuestas recientes</CardTitle>
            <CardDescription>Panorama de encuestas por estado</CardDescription>
          </CardHeader>
          <CardContent>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              {/* Pending Surveys */}
              <div className="rounded-lg border border-yellow-200 bg-yellow-50 p-4">
                <div className="flex items-center justify-between mb-3">
                  <h3 className="font-medium flex items-center">
                    <Clock className="h-4 w-4 mr-2 text-yellow-600" />
                    <span>Pendientes</span>
                  </h3>
                  <span className="text-sm font-semibold bg-yellow-100 text-yellow-800 px-2 py-1 rounded-full">
                    {surveysByStatus['Pending'].length}
                  </span>
                </div>
                <div className="space-y-2">
                  {surveysByStatus['Pending'].length > 0 ? (
                    surveysByStatus['Pending'].slice(0, 2).map(survey => (
                      <div key={survey.id} className="p-2 bg-white rounded border border-yellow-100 hover:shadow-sm transition-shadow">
                        <Link to={`/survey/${survey.id}`} className="block">
                          <p className="font-medium text-sm truncate">{survey.title}</p>
                          <p className="text-xs text-muted-foreground mt-1">
                            Creado: {formatDate(survey.createdAt)}
                          </p>
                        </Link>
                      </div>
                    ))
                  ) : (
                    <p className="text-sm text-muted-foreground py-2">No hay encuestas pendientes</p>
                  )}
                  {surveysByStatus['Pending'].length > 2 && (
                    <Link to="/surveys" className="text-xs text-yellow-600 font-medium hover:underline inline-flex items-center mt-1">
                      Ver {surveysByStatus['Pending'].length - 2} más <ArrowRight className="h-3 w-3 ml-1" />
                    </Link>
                  )}
                </div>
              </div>
              
              {/* In Progress Surveys */}
              <div className="rounded-lg border border-blue-200 bg-blue-50 p-4">
                <div className="flex items-center justify-between mb-3">
                  <h3 className="font-medium flex items-center">
                    <LineChart className="h-4 w-4 mr-2 text-blue-600" />
                    <span>En curso</span>
                  </h3>
                  <span className="text-sm font-semibold bg-blue-100 text-blue-800 px-2 py-1 rounded-full">
                    {surveysByStatus['In Progress'].length}
                  </span>
                </div>
                <div className="space-y-2">
                  {surveysByStatus['In Progress'].length > 0 ? (
                    surveysByStatus['In Progress'].slice(0, 2).map(survey => (
                      <div key={survey.id} className="p-2 bg-white rounded border border-blue-100 hover:shadow-sm transition-shadow">
                        <Link to={`/survey/${survey.id}`} className="block">
                          <p className="font-medium text-sm truncate">{survey.title}</p>
                          <div className="flex justify-between items-center mt-1">
                            <p className="text-xs text-muted-foreground">
                              Creado: {formatDate(survey.createdAt)}
                            </p>
                            <p className="text-xs bg-blue-50 text-blue-700 px-1.5 py-0.5 rounded">
                              {survey.responses} resp.
                            </p>
                          </div>
                        </Link>
                      </div>
                    ))
                  ) : (
                    <p className="text-sm text-muted-foreground py-2">No hay encuestas en curso</p>
                  )}
                  {surveysByStatus['In Progress'].length > 2 && (
                    <Link to="/surveys" className="text-xs text-blue-600 font-medium hover:underline inline-flex items-center mt-1">
                      Ver {surveysByStatus['In Progress'].length - 2} más <ArrowRight className="h-3 w-3 ml-1" />
                    </Link>
                  )}
                </div>
              </div>
              
              {/* Closed Surveys */}
              <div className="rounded-lg border border-green-200 bg-green-50 p-4">
                <div className="flex items-center justify-between mb-3">
                  <h3 className="font-medium flex items-center">
                    <CheckCircle2 className="h-4 w-4 mr-2 text-green-600" />
                    <span>Cerradas</span>
                  </h3>
                  <span className="text-sm font-semibold bg-green-100 text-green-800 px-2 py-1 rounded-full">
                    {surveysByStatus['Closed'].length}
                  </span>
                </div>
                <div className="space-y-2">
                  {surveysByStatus['Closed'].length > 0 ? (
                    surveysByStatus['Closed'].slice(0, 2).map(survey => (
                      <div key={survey.id} className="p-2 bg-white rounded border border-green-100 hover:shadow-sm transition-shadow">
                        <Link to={`/survey/${survey.id}`} className="block">
                          <p className="font-medium text-sm truncate">{survey.title}</p>
                          <div className="flex justify-between items-center mt-1">
                            <p className="text-xs text-muted-foreground">
                              Cerrado: {formatDate(survey.createdAt)}
                            </p>
                            <p className="text-xs bg-green-50 text-green-700 px-1.5 py-0.5 rounded">
                              {survey.responses} resp.
                            </p>
                          </div>
                        </Link>
                      </div>
                    ))
                  ) : (
                    <p className="text-sm text-muted-foreground py-2">No hay encuestas cerradas</p>
                  )}
                  {surveysByStatus['Closed'].length > 2 && (
                    <Link to="/surveys" className="text-xs text-green-600 font-medium hover:underline inline-flex items-center mt-1">
                      Ver {surveysByStatus['Closed'].length - 2} más <ArrowRight className="h-3 w-3 ml-1" />
                    </Link>
                  )}
                </div>
              </div>
            </div>
          </CardContent>
        </Card>
        
        {/* Original overview card */}
        <Card className="shadow-sm">
          <CardHeader className="pb-2">
            <CardTitle className="text-xl">Resumen general</CardTitle>
          </CardHeader>
          <CardContent>
            {/* Latest items in list format */}
            <div className="space-y-4">
              {/* Latest Survey */}
              <div className="flex items-center justify-between border-b pb-3">
                <div>
                  <h3 className="font-medium text-sm text-muted-foreground mb-1">Última encuesta</h3>
                  <p className="font-semibold">{latestSurvey?.title || "No hay encuestas aún"}</p>
                  {latestSurvey && (
                    <p className="text-xs text-muted-foreground">
                      Creado {formatDate(latestSurvey.createdAt)} • {latestSurvey.responses} respuestas
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
                  <h3 className="font-medium text-sm text-muted-foreground mb-1">Última sugerencia</h3>
                  <p className="font-semibold">{latestSuggestion?.content || "No hay sugerencias aún"}</p>
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
              <div className="flex items-center justify-between pb-3">
                <div>
                  <h3 className="font-medium text-sm text-muted-foreground mb-1">Último requerimiento</h3>
                  <p className="font-semibold">{latestRequirement?.title || "No hay requerimientos aún"}</p>
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
            
            {/* Action buttons */}
            <div className="flex justify-end space-x-2 mt-6">
              <Link to="/results">
                <Button size="sm">
                  <BarChart3 className="mr-2 h-4 w-4" />
                  Ver analíticas
                </Button>
              </Link>
              <Link to="/surveys">
                <Button variant="outline" size="sm">
                  Ver todas las encuestas
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
