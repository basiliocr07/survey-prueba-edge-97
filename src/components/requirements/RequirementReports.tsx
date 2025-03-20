
import { useState } from 'react';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from "@/components/ui/card";
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, PieChart, Pie, Cell, Legend } from 'recharts';
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Requirement } from '@/types/requirements';
import { FileBarChart, PieChart as PieChartIcon, Calendar, Activity } from 'lucide-react';

interface RequirementReportsProps {
  requirements: Requirement[];
}

export default function RequirementReports({ requirements = [] }: RequirementReportsProps) {
  const [timeRange, setTimeRange] = useState('all');
  
  // Ensure requirements is a valid array
  const safeRequirements = Array.isArray(requirements) ? requirements : [];
  
  if (safeRequirements.length === 0) {
    return (
      <Card>
        <CardHeader>
          <CardTitle>Requirement Reports</CardTitle>
          <CardDescription>
            No data available. Submit requirements to view reports.
          </CardDescription>
        </CardHeader>
        <CardContent className="h-80 flex items-center justify-center">
          <p className="text-muted-foreground">No requirement data to display</p>
        </CardContent>
      </Card>
    );
  }

  // Calculate status distribution
  const statusData = [
    { name: 'Proposed', value: safeRequirements.filter(req => req.status === 'proposed').length },
    { name: 'In Progress', value: safeRequirements.filter(req => req.status === 'in-progress').length },
    { name: 'Implemented', value: safeRequirements.filter(req => req.status === 'implemented').length },
    { name: 'Rejected', value: safeRequirements.filter(req => req.status === 'rejected').length },
  ];
  
  // Calculate priority distribution
  const priorityData = [
    { name: 'Critical', value: safeRequirements.filter(req => req.priority === 'critical').length },
    { name: 'High', value: safeRequirements.filter(req => req.priority === 'high').length },
    { name: 'Medium', value: safeRequirements.filter(req => req.priority === 'medium').length },
    { name: 'Low', value: safeRequirements.filter(req => req.priority === 'low').length },
  ];
  
  // Calculate category distribution
  const categories = [...new Set(safeRequirements.map(req => req.category).filter(Boolean))];
  const categoryData = categories.map(category => ({
    name: category,
    value: safeRequirements.filter(req => req.category === category).length
  }));
  
  // Calculate project area distribution
  const projectAreas = [...new Set(safeRequirements.map(req => req.projectArea).filter(Boolean))];
  const projectAreaData = projectAreas.map(area => ({
    name: area,
    value: safeRequirements.filter(req => req.projectArea === area).length
  }));
  
  // Chart colors
  const COLORS = ['#0088FE', '#00C49F', '#FFBB28', '#FF8042', '#8884d8', '#82ca9d'];
  
  return (
    <Card>
      <CardHeader>
        <div className="flex items-center justify-between">
          <CardTitle className="flex items-center gap-2">
            <FileBarChart className="h-5 w-5" />
            Requirement Reports
          </CardTitle>
          <Select value={timeRange} onValueChange={setTimeRange}>
            <SelectTrigger className="w-[180px]">
              <SelectValue placeholder="Time Range" />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="all">All time</SelectItem>
              <SelectItem value="month">Last month</SelectItem>
              <SelectItem value="quarter">Last quarter</SelectItem>
              <SelectItem value="year">Last year</SelectItem>
            </SelectContent>
          </Select>
        </div>
        <CardDescription>
          Visualize requirement statistics and analytics
        </CardDescription>
      </CardHeader>
      
      <CardContent>
        <Tabs defaultValue="status" className="space-y-4">
          <TabsList>
            <TabsTrigger value="status" className="flex items-center gap-1">
              <Activity className="h-4 w-4" />
              Status
            </TabsTrigger>
            <TabsTrigger value="priority" className="flex items-center gap-1">
              <PieChartIcon className="h-4 w-4" />
              Priority
            </TabsTrigger>
            <TabsTrigger value="category" className="flex items-center gap-1">
              <Calendar className="h-4 w-4" />
              Category
            </TabsTrigger>
            <TabsTrigger value="area" className="flex items-center gap-1">
              <FileBarChart className="h-4 w-4" />
              Project Area
            </TabsTrigger>
          </TabsList>
          
          <TabsContent value="status">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <h3 className="text-lg font-medium mb-4">Status Distribution</h3>
                <div className="h-64">
                  <ResponsiveContainer width="100%" height="100%">
                    <PieChart>
                      <Pie
                        data={statusData}
                        cx="50%"
                        cy="50%"
                        labelLine={false}
                        label={({ name, percent }) => `${name}: ${(percent * 100).toFixed(0)}%`}
                        outerRadius={80}
                        fill="#8884d8"
                        dataKey="value"
                      >
                        {statusData.map((entry, index) => (
                          <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                        ))}
                      </Pie>
                      <Tooltip />
                      <Legend />
                    </PieChart>
                  </ResponsiveContainer>
                </div>
              </div>
              
              <div>
                <h3 className="text-lg font-medium mb-4">Requirements by Status</h3>
                <div className="h-64">
                  <ResponsiveContainer width="100%" height="100%">
                    <BarChart data={statusData}>
                      <CartesianGrid strokeDasharray="3 3" />
                      <XAxis dataKey="name" />
                      <YAxis />
                      <Tooltip />
                      <Bar dataKey="value" fill="#8884d8" />
                    </BarChart>
                  </ResponsiveContainer>
                </div>
              </div>
            </div>
          </TabsContent>
          
          <TabsContent value="priority">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <h3 className="text-lg font-medium mb-4">Priority Distribution</h3>
                <div className="h-64">
                  <ResponsiveContainer width="100%" height="100%">
                    <PieChart>
                      <Pie
                        data={priorityData}
                        cx="50%"
                        cy="50%"
                        labelLine={false}
                        label={({ name, percent }) => `${name}: ${(percent * 100).toFixed(0)}%`}
                        outerRadius={80}
                        fill="#8884d8"
                        dataKey="value"
                      >
                        {priorityData.map((entry, index) => (
                          <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                        ))}
                      </Pie>
                      <Tooltip />
                      <Legend />
                    </PieChart>
                  </ResponsiveContainer>
                </div>
              </div>
              
              <div>
                <h3 className="text-lg font-medium mb-4">Requirements by Priority</h3>
                <div className="h-64">
                  <ResponsiveContainer width="100%" height="100%">
                    <BarChart data={priorityData}>
                      <CartesianGrid strokeDasharray="3 3" />
                      <XAxis dataKey="name" />
                      <YAxis />
                      <Tooltip />
                      <Bar dataKey="value" fill="#82ca9d" />
                    </BarChart>
                  </ResponsiveContainer>
                </div>
              </div>
            </div>
          </TabsContent>
          
          <TabsContent value="category">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <h3 className="text-lg font-medium mb-4">Category Distribution</h3>
                <div className="h-64">
                  <ResponsiveContainer width="100%" height="100%">
                    <PieChart>
                      <Pie
                        data={categoryData}
                        cx="50%"
                        cy="50%"
                        labelLine={false}
                        label={({ name, percent }) => `${name}: ${(percent * 100).toFixed(0)}%`}
                        outerRadius={80}
                        fill="#8884d8"
                        dataKey="value"
                      >
                        {categoryData.map((entry, index) => (
                          <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                        ))}
                      </Pie>
                      <Tooltip />
                      <Legend />
                    </PieChart>
                  </ResponsiveContainer>
                </div>
              </div>
              
              <div>
                <h3 className="text-lg font-medium mb-4">Requirements by Category</h3>
                <div className="h-64">
                  <ResponsiveContainer width="100%" height="100%">
                    <BarChart data={categoryData}>
                      <CartesianGrid strokeDasharray="3 3" />
                      <XAxis dataKey="name" />
                      <YAxis />
                      <Tooltip />
                      <Bar dataKey="value" fill="#FFBB28" />
                    </BarChart>
                  </ResponsiveContainer>
                </div>
              </div>
            </div>
          </TabsContent>
          
          <TabsContent value="area">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
              <div>
                <h3 className="text-lg font-medium mb-4">Project Area Distribution</h3>
                <div className="h-64">
                  <ResponsiveContainer width="100%" height="100%">
                    <PieChart>
                      <Pie
                        data={projectAreaData}
                        cx="50%"
                        cy="50%"
                        labelLine={false}
                        label={({ name, percent }) => `${name}: ${(percent * 100).toFixed(0)}%`}
                        outerRadius={80}
                        fill="#8884d8"
                        dataKey="value"
                      >
                        {projectAreaData.map((entry, index) => (
                          <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                        ))}
                      </Pie>
                      <Tooltip />
                      <Legend />
                    </PieChart>
                  </ResponsiveContainer>
                </div>
              </div>
              
              <div>
                <h3 className="text-lg font-medium mb-4">Requirements by Project Area</h3>
                <div className="h-64">
                  <ResponsiveContainer width="100%" height="100%">
                    <BarChart data={projectAreaData}>
                      <CartesianGrid strokeDasharray="3 3" />
                      <XAxis dataKey="name" />
                      <YAxis />
                      <Tooltip />
                      <Bar dataKey="value" fill="#0088FE" />
                    </BarChart>
                  </ResponsiveContainer>
                </div>
              </div>
            </div>
          </TabsContent>
        </Tabs>
      </CardContent>
    </Card>
  );
}
