
import { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { Tabs, TabsList, TabsTrigger, TabsContent } from "@/components/ui/tabs";
import { 
  BarChart, Bar, 
  PieChart, Pie, Cell, 
  LineChart, Line, 
  ResponsiveContainer, 
  XAxis, YAxis, 
  Tooltip, Legend, CartesianGrid
} from "recharts";
import { Survey, SurveyResponse, QuestionResponse } from "@/types/surveyTypes";

interface DataVisualizationsProps {
  survey: Survey;
  responses: SurveyResponse[];
}

export default function DataVisualizations({ survey, responses }: DataVisualizationsProps) {
  const [selectedQuestionId, setSelectedQuestionId] = useState<string>(
    survey.questions.length > 0 ? survey.questions[0].id : ''
  );

  const getAnswersForQuestion = (questionId: string): QuestionResponse[] => {
    return responses
      .map(response => response.answers.find(answer => answer.questionId === questionId))
      .filter((answer): answer is QuestionResponse => answer !== undefined);
  };

  const getSelectedQuestion = () => {
    return survey.questions.find(q => q.id === selectedQuestionId);
  };

  const formatDataForVisualization = (questionType: string, answers: QuestionResponse[]) => {
    if (questionType === 'text') {
      return answers.map(answer => ({
        response: answer.value as string,
      }));
    }

    if (questionType === 'multiple-choice') {
      const counts: Record<string, number> = {};
      
      answers.forEach(answer => {
        const selectedOptions = Array.isArray(answer.value) ? answer.value : [answer.value];
        selectedOptions.forEach(option => {
          if (counts[option as string] !== undefined) {
            counts[option as string]++;
          } else {
            counts[option as string] = 1;
          }
        });
      });

      return Object.entries(counts).map(([name, value]) => ({ name, value }));
    }

    if (['single-choice', 'dropdown'].includes(questionType)) {
      const counts: Record<string, number> = {};
      
      answers.forEach(answer => {
        const selected = answer.value as string;
        if (counts[selected] !== undefined) {
          counts[selected]++;
        } else {
          counts[selected] = 1;
        }
      });

      return Object.entries(counts).map(([name, value]) => ({ name, value }));
    }

    if (['rating', 'nps'].includes(questionType)) {
      const min = questionType === 'nps' ? 0 : 1;
      const max = questionType === 'nps' ? 10 : 5;
      const counts: Record<string, number> = {};
      
      for (let i = min; i <= max; i++) {
        counts[i.toString()] = 0;
      }
      
      answers.forEach(answer => {
        const rating = typeof answer.value === 'string' ? answer.value : answer.value[0];
        if (counts[rating] !== undefined) {
          counts[rating]++;
        }
      });

      return Object.entries(counts).map(([name, value]) => ({ name, value }));
    }

    return [];
  };

  const selectedQuestion = getSelectedQuestion();
  const questionAnswers = selectedQuestion ? getAnswersForQuestion(selectedQuestion.id) : [];
  const visualizationData = selectedQuestion 
    ? formatDataForVisualization(selectedQuestion.type, questionAnswers) 
    : [];

  // Colores para gráficos
  const COLORS = ['#3b82f6', '#64748b', '#0ea5e9', '#84cc16', '#8b5cf6', '#f97316'];

  const renderRespondentInfo = () => {
    if (questionAnswers.length === 0) return null;
    
    return (
      <div className="mb-6 border rounded-md p-4">
        <h3 className="text-sm font-medium mb-2">Respondent Information</h3>
        <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-4 gap-4">
          <div>
            <p className="text-xs text-muted-foreground">Total Responses</p>
            <p className="font-medium">{responses.length}</p>
          </div>
          <div>
            <p className="text-xs text-muted-foreground">Valid Responses</p>
            <p className="font-medium">{questionAnswers.filter(a => a.isValid).length}</p>
          </div>
          <div>
            <p className="text-xs text-muted-foreground">Latest Response</p>
            <p className="font-medium">
              {responses.length ? new Date(responses[0].submittedAt).toLocaleDateString() : 'N/A'}
            </p>
          </div>
          <div>
            <p className="text-xs text-muted-foreground">Invalid Responses</p>
            <p className="font-medium">{questionAnswers.filter(a => !a.isValid).length}</p>
          </div>
        </div>
      </div>
    );
  };

  const renderTextResponses = () => {
    return (
      <div className="space-y-3">
        <p className="text-sm text-muted-foreground">
          {questionAnswers.length} text responses collected
        </p>
        <div className="max-h-[400px] overflow-y-auto border rounded-md">
          {questionAnswers.length === 0 ? (
            <p className="text-center p-4 text-muted-foreground">No responses yet</p>
          ) : (
            <div className="divide-y">
              {questionAnswers.map((answer, i) => (
                <div key={i} className={`p-3 hover:bg-muted/40 transition-colors ${!answer.isValid ? 'bg-red-50' : ''}`}>
                  <div className="flex justify-between items-start">
                    <p className="text-sm">{answer.value as string}</p>
                    {!answer.isValid && (
                      <span className="px-2 py-0.5 text-xs rounded-full bg-red-100 text-red-800">Invalid</span>
                    )}
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>
    );
  };

  const renderCharts = () => {
    if (!selectedQuestion || questionAnswers.length === 0) {
      return (
        <div className="flex items-center justify-center h-[300px] border rounded-md">
          <p className="text-muted-foreground">No data available for visualization</p>
        </div>
      );
    }

    if (selectedQuestion.type === 'text') {
      return renderTextResponses();
    }

    return (
      <Tabs defaultValue="bar" className="w-full">
        <TabsList className="grid grid-cols-3 mb-4">
          <TabsTrigger value="bar">Bar Chart</TabsTrigger>
          <TabsTrigger value="pie">Pie Chart</TabsTrigger>
          <TabsTrigger value="line">Line Chart</TabsTrigger>
        </TabsList>

        <TabsContent value="bar" className="w-full animate-fade-in">
          <div className="w-full h-[300px]">
            <ResponsiveContainer width="100%" height="100%">
              <BarChart
                data={visualizationData}
                layout="vertical"
                margin={{ top: 5, right: 30, left: 20, bottom: 5 }}
              >
                <CartesianGrid strokeDasharray="3 3" horizontal={false} />
                <XAxis type="number" />
                <YAxis 
                  dataKey="name" 
                  type="category" 
                  width={120}
                  tick={{ fontSize: 12 }}
                />
                <Tooltip 
                  contentStyle={{ 
                    borderRadius: '6px', 
                    border: '1px solid hsl(var(--border))',
                    boxShadow: '0 10px 15px -3px rgba(0, 0, 0, 0.1)' 
                  }} 
                />
                <Bar 
                  dataKey="value" 
                  fill="#3b82f6" 
                  radius={[0, 4, 4, 0]}
                  background={{ fill: 'hsl(var(--muted))' }}
                />
              </BarChart>
            </ResponsiveContainer>
          </div>
        </TabsContent>

        <TabsContent value="pie" className="w-full animate-fade-in">
          <div className="w-full h-[300px]">
            <ResponsiveContainer width="100%" height="100%">
              <PieChart>
                <Pie
                  data={visualizationData}
                  cx="50%"
                  cy="50%"
                  labelLine={true}
                  label={({ name, percent }) => `${name}: ${(percent * 100).toFixed(0)}%`}
                  outerRadius={100}
                  fill="#8884d8"
                  dataKey="value"
                >
                  {visualizationData.map((entry, index) => (
                    <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                  ))}
                </Pie>
                <Tooltip
                  formatter={(value, name, props) => [`${value} responses`, name]}
                  contentStyle={{ 
                    borderRadius: '6px', 
                    border: '1px solid hsl(var(--border))',
                    boxShadow: '0 10px 15px -3px rgba(0, 0, 0, 0.1)'
                  }}
                />
                <Legend />
              </PieChart>
            </ResponsiveContainer>
          </div>
        </TabsContent>

        <TabsContent value="line" className="w-full animate-fade-in">
          <div className="w-full h-[300px]">
            <ResponsiveContainer width="100%" height="100%">
              <LineChart
                data={visualizationData}
                margin={{ top: 5, right: 30, left: 20, bottom: 5 }}
              >
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="name" />
                <YAxis allowDecimals={false} />
                <Tooltip
                  contentStyle={{ 
                    borderRadius: '6px', 
                    border: '1px solid hsl(var(--border))',
                    boxShadow: '0 10px 15px -3px rgba(0, 0, 0, 0.1)'
                  }}
                />
                <Line
                  type="monotone"
                  dataKey="value"
                  stroke="#3b82f6"
                  strokeWidth={2}
                  activeDot={{ r: 6 }}
                />
              </LineChart>
            </ResponsiveContainer>
          </div>
        </TabsContent>
      </Tabs>
    );
  };

  return (
    <Card className="w-full">
      <CardHeader>
        <CardTitle>Question Analysis</CardTitle>
        <CardDescription>Visualize responses for each question</CardDescription>
      </CardHeader>
      <CardContent>
        <div className="space-y-6">
          <div>
            <label htmlFor="question-selector" className="block text-sm font-medium mb-2">
              Select a question to analyze
            </label>
            <select
              id="question-selector"
              value={selectedQuestionId}
              onChange={(e) => setSelectedQuestionId(e.target.value)}
              className="w-full rounded-md border border-input bg-background px-3 py-2 text-sm ring-offset-background focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring"
            >
              {survey.questions.map((question) => (
                <option key={question.id} value={question.id}>
                  {question.title}
                </option>
              ))}
            </select>
          </div>

          {renderRespondentInfo()}

          <div>
            {renderCharts()}
          </div>
        </div>
      </CardContent>
    </Card>
  );
}
