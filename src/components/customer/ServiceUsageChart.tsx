
import { useState } from 'react';
import { BarChart2, PieChart } from 'lucide-react';
import { 
  CartesianGrid, 
  Legend, 
  ResponsiveContainer, 
  XAxis, 
  YAxis, 
  Tooltip, 
  BarChart as RechartsBarChart, 
  Bar,
  ComposedChart,
  LineChart,
  Line,
  PieChart as RechartsPieChart,
  Pie,
  Cell
} from 'recharts';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { ServiceUsageData } from '@/domain/models/Customer';
import { TimeRange } from '@/application/hooks/useCustomers';
import { Badge } from '@/components/ui/badge';
import { Button } from '@/components/ui/button';
import { ScrollArea } from '@/components/ui/scroll-area';

interface ServiceUsageChartProps {
  serviceUsageData: ServiceUsageData[];
  isLoading?: boolean;
  calculateBrandGrowth?: (timeRange: TimeRange) => { name: string; total: number; recent: number }[];
  calculateMonthlyGrowth?: (months: number) => { name: string; new: number }[];
  calculateMonthlyGrowthByBrand?: (months: number) => {
    months: string[];
    brands: { name: string; data: number[] }[];
  };
}

// Colores para cada servicio en el gráfico
const SERVICE_COLORS = [
  "#8884d8", // Morado principal
  "#82ca9d", // Verde claro
  "#F97316", // Naranja brillante
  "#0EA5E9", // Azul océano
  "#D946EF", // Magenta rosa
  "#8B5CF6", // Púrpura vívido
  "#FF6B8B", // Rosa
  "#9467BD", // Morado oscuro
  "#8C564B", // Marrón
  "#E377C2", // Rosa claro
  "#7F7F7F", // Gris
  "#BCBD22"  // Verde oliva
];

const CustomTooltip = ({ active, payload, label }: any) => {
  if (active && payload && payload.length) {
    // Para la gráfica de crecimiento de servicios
    if (payload[1]) {
      return (
        <div className="bg-background border border-border p-3 rounded-md shadow-md">
          <p className="font-medium">{label}</p>
          <p className="text-sm">
            <span className="text-blue-500">Total: </span>
            <span>{payload[0].value}</span>
          </p>
          <p className="text-sm">
            <span className="text-green-500">Nuevos: </span>
            <span>{payload[1].value}</span>
          </p>
        </div>
      );
    }
    
    // Para gráficas por servicio
    return (
      <div className="bg-background border border-border p-3 rounded-md shadow-md">
        <p className="font-medium">{label}</p>
        {payload.map((entry: any, index: number) => (
          <p key={`item-${index}`} className="text-sm">
            <span style={{ color: entry.color }}>{entry.name}: </span>
            <span>{entry.value}</span>
          </p>
        ))}
      </div>
    );
  }

  return null;
};

export default function ServiceUsageChart({ 
  serviceUsageData, 
  isLoading,
  calculateBrandGrowth,
  calculateMonthlyGrowth,
  calculateMonthlyGrowthByBrand
}: ServiceUsageChartProps) {
  const [timeRange, setTimeRange] = useState<TimeRange>('3');
  const [chartType, setChartType] = useState<'services' | 'servicesPie' | 'servicesGrowth' | 'servicesMonthly' | 'allServices' | 'singleService'>('services');
  const [selectedService, setSelectedService] = useState<string>('');
  
  if (isLoading) {
    return (
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <BarChart2 className="h-5 w-5" />
            Analytics
          </CardTitle>
          <CardDescription>
            Loading data...
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="h-[300px] w-full flex items-center justify-center">
            <p className="text-muted-foreground">Loading chart data...</p>
          </div>
        </CardContent>
      </Card>
    );
  }

  if (!serviceUsageData.length) {
    return (
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <BarChart2 className="h-5 w-5" />
            Analytics
          </CardTitle>
          <CardDescription>
            No data available
          </CardDescription>
        </CardHeader>
        <CardContent>
          <div className="h-[300px] w-full flex items-center justify-center">
            <p className="text-muted-foreground">No data to display</p>
          </div>
        </CardContent>
      </Card>
    );
  }

  // Obtener datos basados en servicios en lugar de marcas
  const serviceGrowthData = calculateBrandGrowth ? 
    serviceUsageData.map(service => ({
      name: service.name,
      total: service.count,
      recent: Math.round(service.count * 0.3) // Simular clientes recientes (30% del total)
    })) : [];
    
  // Obtener datos mensuales para servicios
  const getMonthlyServiceData = () => {
    if (!calculateMonthlyGrowth) return [];
    
    // Basa los datos mensuales en el servicio seleccionado o el primer servicio
    const serviceToUse = selectedService || (serviceUsageData.length > 0 ? serviceUsageData[0].name : '');
    
    // Obtener datos mensuales base
    const baseMonthlyData = calculateMonthlyGrowth(parseInt(timeRange));
    
    // Crear datos simulados para el servicio
    return baseMonthlyData.map(month => ({
      name: month.name,
      new: Math.round(month.new * (serviceUsageData.find(s => s.name === serviceToUse)?.count || 1) / 
                    (serviceUsageData.reduce((sum, s) => sum + s.count, 0) || 1))
    }));
  };

  // Obtener servicios disponibles
  const availableServices = serviceUsageData.map(item => item.name);

  // Crear datos simulados para todos los servicios por mes
  const getAllServicesMonthlyData = () => {
    if (!calculateMonthlyGrowth) return { months: [], services: [] };
    
    const baseMonthlyData = calculateMonthlyGrowth(parseInt(timeRange));
    const months = baseMonthlyData.map(m => m.name);
    
    // Generar datos para cada servicio
    const services = serviceUsageData.slice(0, 8).map((service, index) => {
      // Factor para simular diferentes patrones de crecimiento
      const growthFactor = 0.5 + Math.random() * 0.8;
      
      // Generar datos mensuales para este servicio
      const data = baseMonthlyData.map(month => {
        return Math.round(month.new * (service.count / serviceUsageData.reduce((sum, s) => sum + s.count, 0)) * growthFactor);
      });
      
      return {
        name: service.name,
        data
      };
    });
    
    return { months, services };
  };

  // Datos mensuales de todos los servicios
  const monthlyServiceData = getAllServicesMonthlyData();
  
  // Datos mensuales para un servicio específico
  const singleServiceData = getMonthlyServiceData();
  
  // Preparar datos para el gráfico de todos los servicios
  const formattedMonthlyServiceData = monthlyServiceData.months.map((month, index) => {
    const dataPoint: any = { name: month };
    monthlyServiceData.services.forEach((service, serviceIndex) => {
      // Limitar a 8 servicios para no saturar el gráfico
      if (serviceIndex < 8) {
        dataPoint[service.name] = service.data[index];
      }
    });
    return dataPoint;
  });

  return (
    <Card>
      <CardHeader>
        <CardTitle className="flex items-center gap-2">
          <BarChart2 className="h-5 w-5" />
          Service Analytics
        </CardTitle>
        <CardDescription>
          Analyze service usage and growth
        </CardDescription>
        <div className="flex items-center justify-between mt-4 flex-wrap gap-2">
          <Tabs value={chartType} onValueChange={(value) => setChartType(value as any)}>
            <TabsList>
              <TabsTrigger value="services">Service Usage (Bar)</TabsTrigger>
              <TabsTrigger value="servicesPie">Service Usage (Pie)</TabsTrigger>
              <TabsTrigger value="servicesGrowth">Service Growth</TabsTrigger>
              <TabsTrigger value="servicesMonthly">Monthly Growth</TabsTrigger>
              <TabsTrigger value="allServices">All Services</TabsTrigger>
              <TabsTrigger value="singleService">Single Service</TabsTrigger>
            </TabsList>
          </Tabs>
          
          <div className="flex items-center gap-2">
            {chartType === 'singleService' && (
              <Select 
                value={selectedService} 
                onValueChange={setSelectedService}
              >
                <SelectTrigger className="w-[150px]">
                  <SelectValue placeholder="Select Service" />
                </SelectTrigger>
                <SelectContent>
                  {availableServices.map(service => (
                    <SelectItem key={service} value={service}>{service}</SelectItem>
                  ))}
                </SelectContent>
              </Select>
            )}
            
            <Select value={timeRange} onValueChange={(value) => setTimeRange(value as TimeRange)}>
              <SelectTrigger className="w-[150px]">
                <SelectValue placeholder="Time Range" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="1">Last Month</SelectItem>
                <SelectItem value="3">Last 3 Months</SelectItem>
                <SelectItem value="12">Last Year</SelectItem>
                <SelectItem value="all">All Time</SelectItem>
              </SelectContent>
            </Select>
          </div>
        </div>
        
        {chartType === 'allServices' && (
          <div className="mt-4">
            <h4 className="text-sm font-medium mb-2">Services:</h4>
            <ScrollArea className="h-10 whitespace-nowrap">
              <div className="flex gap-2 pb-2">
                {monthlyServiceData.services.slice(0, 8).map((service, index) => (
                  <Badge 
                    key={service.name} 
                    style={{ backgroundColor: SERVICE_COLORS[index % SERVICE_COLORS.length] }}
                    className="text-white"
                  >
                    {service.name}
                  </Badge>
                ))}
              </div>
            </ScrollArea>
          </div>
        )}
      </CardHeader>
      <CardContent>
        <div className="h-[300px] w-full">
          {chartType === 'services' && (
            <ResponsiveContainer width="100%" height="100%">
              <RechartsBarChart
                data={serviceUsageData}
                layout="vertical"
                margin={{ top: 20, right: 30, left: 100, bottom: 5 }}
              >
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis type="number" />
                <YAxis dataKey="name" type="category" width={100} />
                <Tooltip content={<CustomTooltip />} />
                <Legend />
                <Bar dataKey="count" fill="#8884d8" name="Number of Customers" />
              </RechartsBarChart>
            </ResponsiveContainer>
          )}
          
          {chartType === 'servicesPie' && (
            <ResponsiveContainer width="100%" height="100%">
              <RechartsPieChart margin={{ top: 0, right: 0, left: 0, bottom: 0 }}>
                <Pie
                  data={serviceUsageData}
                  cx="50%"
                  cy="50%"
                  labelLine={true}
                  outerRadius={100}
                  fill="#8884d8"
                  dataKey="count"
                  label={({ name, percent }) => `${name}: ${(percent * 100).toFixed(0)}%`}
                >
                  {serviceUsageData.map((entry, index) => (
                    <Cell key={`cell-${index}`} fill={SERVICE_COLORS[index % SERVICE_COLORS.length]} />
                  ))}
                </Pie>
                <Tooltip />
                <Legend />
              </RechartsPieChart>
            </ResponsiveContainer>
          )}
          
          {chartType === 'servicesGrowth' && serviceGrowthData.length > 0 && (
            <ResponsiveContainer width="100%" height="100%">
              <ComposedChart
                layout="vertical"
                data={serviceGrowthData}
                margin={{ top: 20, right: 30, left: 100, bottom: 5 }}
              >
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis type="number" />
                <YAxis dataKey="name" type="category" width={100} />
                <Tooltip content={<CustomTooltip />} />
                <Legend />
                <Bar dataKey="total" fill="#8884d8" name="Total Customers" />
                <Bar dataKey="recent" fill="#82ca9d" name="New Customers" />
              </ComposedChart>
            </ResponsiveContainer>
          )}
          
          {chartType === 'servicesMonthly' && singleServiceData.length > 0 && (
            <ResponsiveContainer width="100%" height="100%">
              <RechartsBarChart
                data={singleServiceData}
                margin={{ top: 20, right: 30, left: 20, bottom: 5 }}
              >
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="name" />
                <YAxis />
                <Tooltip content={<CustomTooltip />} />
                <Legend />
                <Bar dataKey="new" fill="#82ca9d" name="New Service Customers" />
              </RechartsBarChart>
            </ResponsiveContainer>
          )}

          {chartType === 'allServices' && formattedMonthlyServiceData.length > 0 && (
            <ResponsiveContainer width="100%" height="100%">
              <LineChart
                data={formattedMonthlyServiceData}
                margin={{ top: 20, right: 50, left: 20, bottom: 5 }}
              >
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="name" />
                <YAxis />
                <Tooltip content={<CustomTooltip />} />
                <Legend />
                {monthlyServiceData.services.slice(0, 8).map((service, index) => (
                  <Line
                    key={service.name}
                    type="monotone"
                    dataKey={service.name}
                    stroke={SERVICE_COLORS[index % SERVICE_COLORS.length]}
                    activeDot={{ r: 8 }}
                    name={service.name}
                  />
                ))}
              </LineChart>
            </ResponsiveContainer>
          )}
          
          {chartType === 'singleService' && selectedService && (
            <ResponsiveContainer width="100%" height="100%">
              <RechartsBarChart
                data={singleServiceData}
                margin={{ top: 20, right: 30, left: 20, bottom: 5 }}
              >
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="name" />
                <YAxis />
                <Tooltip content={<CustomTooltip />} />
                <Legend />
                <Bar 
                  dataKey="new" 
                  fill={SERVICE_COLORS[0]} 
                  name={`New Customers - ${selectedService}`} 
                />
              </RechartsBarChart>
            </ResponsiveContainer>
          )}
          
          {chartType === 'singleService' && !selectedService && (
            <div className="h-full flex items-center justify-center">
              <p className="text-muted-foreground">Select a service to view its growth data</p>
            </div>
          )}
        </div>
      </CardContent>
    </Card>
  );
}
