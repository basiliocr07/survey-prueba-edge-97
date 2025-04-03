
import React from "react";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import { Collapsible, CollapsibleContent, CollapsibleTrigger } from "@/components/ui/collapsible";
import { MoreHorizontal, UserCircle2, ChevronDown, ChevronUp, MessageSquare } from "lucide-react";
import { useSurveyResponse } from "@/application/hooks/useSurveyResponse";

const Dashboard = () => {
  const [isOpen, setIsOpen] = React.useState(false);
  const { responses, isLoading } = useSurveyResponse();
  
  // Tomar solo las últimas 5 respuestas
  const latestResponses = responses?.slice(0, 5) || [];
  
  const surveyData = [
    {
      id: 1,
      title: "Customer Satisfaction Survey",
      responses: 150,
      completionRate: 75,
      status: "Active",
    },
    {
      id: 2,
      title: "Employee Engagement Survey",
      responses: 200,
      completionRate: 85,
      status: "Draft",
    },
    {
      id: 3,
      title: "Product Feedback Survey",
      responses: 100,
      completionRate: 60,
      status: "Archived",
    },
  ];

  const requirementsData = [
    {
      id: 1,
      title: "Login System Enhancement",
      status: "In Progress",
      priority: "High",
      summary: "Implement additional security features for login system",
      assignee: "John Smith",
      dueDate: "2023-09-30"
    },
    {
      id: 2,
      title: "Dashboard Performance",
      status: "To Do",
      priority: "Medium",
      summary: "Optimize dashboard load time and responsiveness",
      assignee: "Jane Doe",
      dueDate: "2023-10-15"
    },
    {
      id: 3,
      title: "User Profile Customization",
      status: "Done",
      priority: "Low",
      summary: "Allow users to customize their profile settings and appearance",
      assignee: "Mike Johnson",
      dueDate: "2023-09-10"
    }
  ];

  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    return new Intl.DateTimeFormat('es-ES', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    }).format(date);
  };

  return (
    <div className="container py-8">
      <h1 className="text-2xl font-bold mb-4">Dashboard</h1>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4 mb-6">
        <Card>
          <CardHeader>
            <CardTitle>Total Surveys</CardTitle>
            <CardDescription>Number of surveys created</CardDescription>
          </CardHeader>
          <CardContent>
            <div className="text-3xl font-bold">3</div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Active Surveys</CardTitle>
            <CardDescription>Surveys currently being taken</CardDescription>
          </CardHeader>
          <CardContent>
            <div className="text-3xl font-bold">1</div>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Total Responses</CardTitle>
            <CardDescription>Responses received across all surveys</CardDescription>
          </CardHeader>
          <CardContent>
            <div className="text-3xl font-bold">450</div>
          </CardContent>
        </Card>
      </div>

      {/* Contenedor de últimas respuestas de encuestas */}
      <div className="mb-6">
        <Card className="shadow-md">
          <Collapsible open={isOpen} onOpenChange={setIsOpen}>
            <CardHeader className="py-3">
              <div className="flex justify-between items-center">
                <CardTitle className="text-lg flex items-center">
                  <MessageSquare className="h-5 w-5 mr-2" />
                  Últimas Respuestas
                </CardTitle>
                <CollapsibleTrigger asChild>
                  <Button variant="ghost" size="sm" className="h-8 w-8 p-0">
                    {isOpen ? <ChevronUp className="h-4 w-4" /> : <ChevronDown className="h-4 w-4" />}
                  </Button>
                </CollapsibleTrigger>
              </div>
              <CardDescription>
                {latestResponses.length > 0 
                  ? `Mostrando las últimas ${latestResponses.length} respuestas recibidas` 
                  : "No hay respuestas recientes"}
              </CardDescription>
            </CardHeader>
            
            <CollapsibleContent>
              <CardContent>
                {isLoading ? (
                  <div className="text-center py-4">
                    <p className="text-sm text-muted-foreground">Cargando respuestas...</p>
                  </div>
                ) : latestResponses.length === 0 ? (
                  <div className="text-center py-4">
                    <p className="text-sm text-muted-foreground">No hay respuestas recientes</p>
                  </div>
                ) : (
                  <div className="space-y-3">
                    {latestResponses.map((response) => (
                      <div key={response.id} className="border rounded-md p-3">
                        <div className="flex justify-between items-start">
                          <div>
                            <div className="font-medium">{response.respondentName}</div>
                            <div className="text-sm text-muted-foreground">{response.respondentEmail}</div>
                            {response.respondentCompany && (
                              <div className="text-xs text-muted-foreground mt-1">
                                {response.respondentCompany}
                              </div>
                            )}
                          </div>
                          <div className="text-xs text-muted-foreground">
                            {formatDate(response.submittedAt)}
                          </div>
                        </div>
                        <div className="mt-2">
                          <div className="text-xs text-muted-foreground">
                            {response.answers.length} respuestas enviadas
                          </div>
                        </div>
                      </div>
                    ))}
                  </div>
                )}
              </CardContent>
            </CollapsibleContent>
          </Collapsible>
        </Card>
      </div>

      <div className="mb-8">
        <h2 className="text-xl font-semibold mb-2">Recent Surveys</h2>
        <div className="overflow-x-auto">
          <table className="min-w-full divide-y divide-gray-200">
            <thead>
              <tr>
                <th className="px-4 py-2 text-left text-sm font-medium text-gray-500">Title</th>
                <th className="px-4 py-2 text-left text-sm font-medium text-gray-500">Responses</th>
                <th className="px-4 py-2 text-left text-sm font-medium text-gray-500">Completion Rate</th>
                <th className="px-4 py-2 text-left text-sm font-medium text-gray-500">Status</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-200">
              {surveyData.map((survey) => (
                <tr key={survey.id}>
                  <td className="px-4 py-4">{survey.title}</td>
                  <td className="px-4 py-4">{survey.responses}</td>
                  <td className="px-4 py-4">{survey.completionRate}%</td>
                  <td className="px-4 py-4">
                    <Badge variant={survey.status === 'Active' ? 'secondary' : survey.status === 'Draft' ? 'outline' : 'default'}>
                      {survey.status}
                    </Badge>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      <div>
        <h2 className="text-xl font-semibold mb-2">Requirements</h2>
        <div className="overflow-x-auto">
          <table className="min-w-full divide-y divide-gray-200">
            <thead>
              <tr>
                <th className="px-4 py-2 text-left text-sm font-medium text-gray-500">Title</th>
                <th className="px-4 py-2 text-left text-sm font-medium text-gray-500">Status</th>
                <th className="px-4 py-2 text-left text-sm font-medium text-gray-500">Assignee</th>
                <th className="px-4 py-2 text-left text-sm font-medium text-gray-500">Due Date</th>
                <th className="px-4 py-2 text-right text-sm font-medium text-gray-500"></th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-200">
              {requirementsData.map((req) => (
                <tr key={req.id} className="border-t border-gray-200">
                  <td className="px-4 py-4">
                    <div className="flex flex-col">
                      <span className="font-medium text-sm">{req.title}</span>
                      <span className="text-xs text-gray-500 mt-1 line-clamp-2">{req.summary}</span>
                    </div>
                  </td>
                  <td className="px-4 py-4">
                    <Badge variant={req.status === 'Done' ? 'secondary' : req.status === 'In Progress' ? 'outline' : 'default'}>
                      {req.status}
                    </Badge>
                  </td>
                  <td className="px-4 py-4">
                    <div className="flex items-center">
                      <UserCircle2 className="h-5 w-5 text-gray-400 mr-2" />
                      <span className="text-sm">{req.assignee}</span>
                    </div>
                  </td>
                  <td className="px-4 py-4 whitespace-nowrap">
                    <span className="text-sm">{new Date(req.dueDate).toLocaleDateString()}</span>
                  </td>
                  <td className="px-4 py-4 whitespace-nowrap text-right">
                    <DropdownMenu>
                      <DropdownMenuTrigger asChild>
                        <Button variant="ghost" size="icon">
                          <MoreHorizontal className="h-4 w-4" />
                        </Button>
                      </DropdownMenuTrigger>
                      <DropdownMenuContent align="end">
                        <DropdownMenuItem>View Details</DropdownMenuItem>
                        <DropdownMenuItem>Edit</DropdownMenuItem>
                        <DropdownMenuItem>Delete</DropdownMenuItem>
                      </DropdownMenuContent>
                    </DropdownMenu>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
};

export default Dashboard;
