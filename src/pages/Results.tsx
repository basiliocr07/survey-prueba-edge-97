import { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import DataVisualizations from "@/components/results/DataVisualizations";
import { sampleSurveys, sampleResponses, Survey } from "@/utils/sampleData";
import { BarChart, PieChart, Download, Filter, Calendar, Eye } from "lucide-react";
import { Button } from "@/components/ui/button";
import Navbar from '@/components/layout/Navbar';
import Footer from '@/components/layout/Footer';
import { cn } from '@/lib/utils';
import { SurveyResponse } from "@/types/surveyTypes";

export default function Results() {
  const [selectedSurveyId, setSelectedSurveyId] = useState<string>(sampleSurveys[0]?.id || '');
  
  const selectedSurvey = sampleSurveys.find(survey => survey.id === selectedSurveyId) || sampleSurveys[0];
  const surveyResponses = sampleResponses.filter(response => response.surveyId === selectedSurveyId) as SurveyResponse[];

  const getAverageCompletionTime = () => {
    if (surveyResponses.length === 0) return 0;
    const total = surveyResponses.reduce((sum, response) => sum + response.completionTime, 0);
    return Math.round(total / surveyResponses.length);
  };

  const formatTime = (seconds: number) => {
    const minutes = Math.floor(seconds / 60);
    const remainingSeconds = seconds % 60;
    return `${minutes}m ${remainingSeconds}s`;
  };

  return (
    <div className="min-h-screen flex flex-col bg-background">
      <Navbar />
      
      <main className="flex-1 w-full max-w-7xl mx-auto pt-24 px-6 pb-16">
        <div className="mb-8">
          <h1 className="text-3xl font-bold mb-2">Survey Results & Analysis</h1>
          <p className="text-muted-foreground">
            View responses, analyze data, and export insights from your surveys
          </p>
        </div>
        
        <div className="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
          <div className="md:col-span-1">
            <div className="sticky top-24">
              <Card>
                <CardHeader>
                  <CardTitle className="text-lg">My Surveys</CardTitle>
                </CardHeader>
                <CardContent className="p-0">
                  <div className="divide-y">
                    {sampleSurveys.map((survey) => (
                      <button
                        key={survey.id}
                        className={cn(
                          "w-full text-left px-4 py-3 hover:bg-accent transition-colors",
                          selectedSurveyId === survey.id ? "bg-accent" : ""
                        )}
                        onClick={() => setSelectedSurveyId(survey.id)}
                      >
                        <div className="font-medium truncate">{survey.title}</div>
                        <div className="text-sm text-muted-foreground mt-1">
                          {survey.responses} responses
                        </div>
                      </button>
                    ))}
                  </div>
                </CardContent>
              </Card>
            </div>
          </div>
          
          <div className="md:col-span-3 space-y-6">
            {selectedSurvey && (
              <>
                <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 border-b pb-4">
                  <div>
                    <h2 className="text-2xl font-bold">{selectedSurvey.title}</h2>
                    <p className="text-muted-foreground">{selectedSurvey.description}</p>
                  </div>
                  <div className="flex gap-2">
                    <Button variant="outline" size="sm">
                      <Filter className="mr-2 h-4 w-4" /> Filter
                    </Button>
                    <Button variant="outline" size="sm">
                      <Download className="mr-2 h-4 w-4" /> Export
                    </Button>
                    <Button size="sm">
                      <Eye className="mr-2 h-4 w-4" /> View Survey
                    </Button>
                  </div>
                </div>
                
                <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
                  <Card>
                    <CardContent className="pt-6">
                      <div className="flex items-start">
                        <div className="p-2 rounded-md bg-primary/10 text-primary mr-4">
                          <BarChart className="h-6 w-6" />
                        </div>
                        <div>
                          <div className="text-sm text-muted-foreground">Total Responses</div>
                          <div className="text-2xl font-bold">{selectedSurvey.responses}</div>
                        </div>
                      </div>
                    </CardContent>
                  </Card>
                  
                  <Card>
                    <CardContent className="pt-6">
                      <div className="flex items-start">
                        <div className="p-2 rounded-md bg-primary/10 text-primary mr-4">
                          <PieChart className="h-6 w-6" />
                        </div>
                        <div>
                          <div className="text-sm text-muted-foreground">Completion Rate</div>
                          <div className="text-2xl font-bold">{selectedSurvey.completionRate}%</div>
                        </div>
                      </div>
                    </CardContent>
                  </Card>
                  
                  <Card>
                    <CardContent className="pt-6">
                      <div className="flex items-start">
                        <div className="p-2 rounded-md bg-primary/10 text-primary mr-4">
                          <Calendar className="h-6 w-6" />
                        </div>
                        <div>
                          <div className="text-sm text-muted-foreground">Avg. Completion Time</div>
                          <div className="text-2xl font-bold">{formatTime(getAverageCompletionTime())}</div>
                        </div>
                      </div>
                    </CardContent>
                  </Card>
                </div>
                
                <Tabs defaultValue="charts" className="w-full">
                  <TabsList className="grid grid-cols-3 w-full max-w-md mb-6">
                    <TabsTrigger value="charts">Charts</TabsTrigger>
                    <TabsTrigger value="responses">Responses</TabsTrigger>
                    <TabsTrigger value="insights">Insights</TabsTrigger>
                  </TabsList>
                  
                  <TabsContent value="charts" className="space-y-6 animate-fade-in">
                    <DataVisualizations survey={selectedSurvey} responses={surveyResponses} />
                  </TabsContent>
                  
                  <TabsContent value="responses" className="animate-fade-in">
                    <Card>
                      <CardHeader>
                        <CardTitle>Individual Responses</CardTitle>
                      </CardHeader>
                      <CardContent>
                        <div className="space-y-4">
                          {surveyResponses.length === 0 ? (
                            <p className="text-center py-6 text-muted-foreground">
                              No responses yet for this survey
                            </p>
                          ) : (
                            <div className="border rounded-md overflow-hidden">
                              <table className="w-full">
                                <thead>
                                  <tr className="bg-muted/50">
                                    <th className="px-4 py-3 text-left font-medium text-muted-foreground">Response ID</th>
                                    <th className="px-4 py-3 text-left font-medium text-muted-foreground">Submitted</th>
                                    <th className="px-4 py-3 text-left font-medium text-muted-foreground">Duration</th>
                                    <th className="px-4 py-3 text-left font-medium text-muted-foreground"></th>
                                  </tr>
                                </thead>
                                <tbody className="divide-y">
                                  {surveyResponses.map((response) => (
                                    <tr key={response.id} className="hover:bg-muted/30 transition-colors">
                                      <td className="px-4 py-3">{response.id}</td>
                                      <td className="px-4 py-3">
                                        {new Date(response.submittedAt).toLocaleDateString()}
                                      </td>
                                      <td className="px-4 py-3">{formatTime(response.completionTime)}</td>
                                      <td className="px-4 py-3 text-right">
                                        <Button variant="ghost" size="sm">View Details</Button>
                                      </td>
                                    </tr>
                                  ))}
                                </tbody>
                              </table>
                            </div>
                          )}
                        </div>
                      </CardContent>
                    </Card>
                  </TabsContent>
                  
                  <TabsContent value="insights" className="animate-fade-in">
                    <Card>
                      <CardHeader>
                        <CardTitle>Survey Insights</CardTitle>
                      </CardHeader>
                      <CardContent>
                        <div className="space-y-6">
                          <div>
                            <h3 className="text-lg font-medium mb-2">Response Trends</h3>
                            <div className="bg-muted/30 p-4 rounded-md">
                              <p className="text-sm text-muted-foreground">
                                Response trends visualization will appear here once there are enough responses to analyze patterns over time.
                              </p>
                            </div>
                          </div>
                          
                          <div>
                            <h3 className="text-lg font-medium mb-2">Key Findings</h3>
                            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                              <div className="border rounded-md p-4">
                                <div className="text-sm font-medium mb-1">Most Selected Answer</div>
                                <div className="text-muted-foreground">
                                  {surveyResponses.length > 0 ? (
                                    "Very satisfied"
                                  ) : (
                                    "No responses yet"
                                  )}
                                </div>
                              </div>
                              
                              <div className="border rounded-md p-4">
                                <div className="text-sm font-medium mb-1">Least Selected Answer</div>
                                <div className="text-muted-foreground">
                                  {surveyResponses.length > 0 ? (
                                    "Very unsatisfied"
                                  ) : (
                                    "No responses yet"
                                  )}
                                </div>
                              </div>
                              
                              <div className="border rounded-md p-4">
                                <div className="text-sm font-medium mb-1">Average Rating</div>
                                <div className="text-muted-foreground">
                                  {surveyResponses.length > 0 ? (
                                    "7.6 / 10"
                                  ) : (
                                    "No responses yet"
                                  )}
                                </div>
                              </div>
                              
                              <div className="border rounded-md p-4">
                                <div className="text-sm font-medium mb-1">Response Rate</div>
                                <div className="text-muted-foreground">
                                  {surveyResponses.length > 0 ? (
                                    "32%"
                                  ) : (
                                    "No responses yet"
                                  )}
                                </div>
                              </div>
                            </div>
                          </div>
                        </div>
                      </CardContent>
                    </Card>
                  </TabsContent>
                </Tabs>
              </>
            )}
          </div>
        </div>
      </main>
      
      <Footer />
    </div>
  );
}
