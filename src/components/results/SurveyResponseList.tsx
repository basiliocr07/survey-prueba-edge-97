
import { useState } from 'react';
import { 
  Card, 
  CardContent, 
  CardHeader, 
  CardTitle, 
  CardDescription 
} from "@/components/ui/card";
import { 
  Table, 
  TableHeader, 
  TableRow, 
  TableHead, 
  TableBody, 
  TableCell 
} from "@/components/ui/table";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { SurveyResponse } from "@/types/surveyTypes";
import { 
  ChevronDown, 
  ChevronUp, 
  CheckCircle, 
  XCircle, 
  ChevronRight, 
  Mail, 
  Phone, 
  Building
} from "lucide-react";

interface SurveyResponseListProps {
  responses: SurveyResponse[];
}

export default function SurveyResponseList({ responses }: SurveyResponseListProps) {
  const [expandedResponseId, setExpandedResponseId] = useState<string | null>(null);
  const [sortField, setSortField] = useState<keyof SurveyResponse>("submittedAt");
  const [sortDirection, setSortDirection] = useState<"asc" | "desc">("desc");

  const toggleExpandResponse = (responseId: string) => {
    if (expandedResponseId === responseId) {
      setExpandedResponseId(null);
    } else {
      setExpandedResponseId(responseId);
    }
  };

  const handleSort = (field: keyof SurveyResponse) => {
    if (sortField === field) {
      setSortDirection(sortDirection === "asc" ? "desc" : "asc");
    } else {
      setSortField(field);
      setSortDirection("desc");
    }
  };

  const sortResponses = (a: SurveyResponse, b: SurveyResponse) => {
    if (sortField === "submittedAt") {
      return sortDirection === "asc"
        ? new Date(a.submittedAt).getTime() - new Date(b.submittedAt).getTime()
        : new Date(b.submittedAt).getTime() - new Date(a.submittedAt).getTime();
    }
    
    if (sortField === "respondentName" || sortField === "respondentEmail" || sortField === "respondentCompany") {
      const aValue = a[sortField] || "";
      const bValue = b[sortField] || "";
      return sortDirection === "asc"
        ? aValue.localeCompare(bValue)
        : bValue.localeCompare(aValue);
    }
    
    return 0;
  };

  const getSortIcon = (field: keyof SurveyResponse) => {
    if (sortField !== field) return null;
    return sortDirection === "asc" ? <ChevronUp className="h-4 w-4" /> : <ChevronDown className="h-4 w-4" />;
  };
  
  const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    return new Intl.DateTimeFormat('default', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    }).format(date);
  };

  const countValidAnswers = (response: SurveyResponse) => {
    return response.answers.filter(answer => answer.isValid).length;
  };

  const sortedResponses = [...responses].sort(sortResponses);

  return (
    <Card>
      <CardHeader>
        <CardTitle>Survey Responses</CardTitle>
        <CardDescription>
          Showing {responses.length} responses received from respondents
        </CardDescription>
      </CardHeader>
      <CardContent>
        {responses.length === 0 ? (
          <div className="text-center py-8">
            <p className="text-muted-foreground">No responses received yet</p>
          </div>
        ) : (
          <div className="border rounded-md overflow-hidden">
            <Table>
              <TableHeader>
                <TableRow>
                  <TableHead className="w-[40px]"></TableHead>
                  <TableHead 
                    className="cursor-pointer hover:bg-muted/50 transition-colors"
                    onClick={() => handleSort("respondentName")}
                  >
                    <div className="flex items-center space-x-1">
                      <span>Respondent</span>
                      {getSortIcon("respondentName")}
                    </div>
                  </TableHead>
                  <TableHead 
                    className="cursor-pointer hover:bg-muted/50 transition-colors"
                    onClick={() => handleSort("respondentCompany")}
                  >
                    <div className="flex items-center space-x-1">
                      <span>Company</span>
                      {getSortIcon("respondentCompany")}
                    </div>
                  </TableHead>
                  <TableHead 
                    className="cursor-pointer hover:bg-muted/50 transition-colors"
                    onClick={() => handleSort("submittedAt")}
                  >
                    <div className="flex items-center space-x-1">
                      <span>Submitted</span>
                      {getSortIcon("submittedAt")}
                    </div>
                  </TableHead>
                  <TableHead>Validation</TableHead>
                </TableRow>
              </TableHeader>
              <TableBody>
                {sortedResponses.map((response) => {
                  const isExpanded = expandedResponseId === response.id;
                  const validAnswers = countValidAnswers(response);
                  const totalAnswers = response.answers.length;
                  const validationStatus = validAnswers === totalAnswers ? "valid" : "partially-valid";
                  
                  return (
                    <>
                      <TableRow key={response.id} className="hover:bg-muted/40 transition-colors">
                        <TableCell>
                          <Button 
                            variant="ghost" 
                            size="icon" 
                            onClick={() => toggleExpandResponse(response.id || '')}
                            className="h-8 w-8"
                          >
                            {isExpanded ? <ChevronDown className="h-4 w-4" /> : <ChevronRight className="h-4 w-4" />}
                          </Button>
                        </TableCell>
                        <TableCell>
                          <div className="font-medium">{response.respondentName}</div>
                          <div className="text-sm text-muted-foreground">{response.respondentEmail}</div>
                        </TableCell>
                        <TableCell>
                          {response.respondentCompany || "Not specified"}
                        </TableCell>
                        <TableCell>
                          {formatDate(response.submittedAt)}
                        </TableCell>
                        <TableCell>
                          <div className="flex items-center space-x-2">
                            <Badge 
                              variant={validationStatus === "valid" ? "default" : "secondary"}
                              className={validationStatus === "valid" ? "bg-green-500" : "bg-yellow-500"}
                            >
                              {validAnswers}/{totalAnswers} Valid
                            </Badge>
                          </div>
                        </TableCell>
                      </TableRow>
                      
                      {isExpanded && (
                        <TableRow>
                          <TableCell colSpan={5} className="p-0">
                            <div className="bg-muted/30 p-4 border-t">
                              <div className="grid gap-4">
                                <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                                  <div className="flex items-center space-x-2">
                                    <Mail className="h-4 w-4 text-muted-foreground" />
                                    <span className="text-sm">{response.respondentEmail}</span>
                                  </div>
                                  <div className="flex items-center space-x-2">
                                    <Phone className="h-4 w-4 text-muted-foreground" />
                                    <span className="text-sm">{response.respondentPhone || "Not provided"}</span>
                                  </div>
                                  <div className="flex items-center space-x-2">
                                    <Building className="h-4 w-4 text-muted-foreground" />
                                    <span className="text-sm">{response.respondentCompany || "Not provided"}</span>
                                  </div>
                                </div>
                                
                                <div className="mt-4">
                                  <h4 className="text-sm font-medium mb-2">Answers</h4>
                                  <div className="border rounded-md">
                                    <div className="divide-y">
                                      {response.answers.map((answer, i) => (
                                        <div key={i} className="p-3 flex justify-between items-start">
                                          <div>
                                            <div className="font-medium text-sm">{answer.questionTitle}</div>
                                            <div className="text-sm mt-1">
                                              {Array.isArray(answer.value) 
                                                ? answer.value.join(", ") 
                                                : answer.value}
                                            </div>
                                          </div>
                                          <div>
                                            {answer.isValid 
                                              ? <CheckCircle className="h-5 w-5 text-green-500" /> 
                                              : <XCircle className="h-5 w-5 text-red-500" />
                                            }
                                          </div>
                                        </div>
                                      ))}
                                    </div>
                                  </div>
                                </div>
                              </div>
                            </div>
                          </TableCell>
                        </TableRow>
                      )}
                    </>
                  );
                })}
              </TableBody>
            </Table>
          </div>
        )}
      </CardContent>
    </Card>
  );
}
