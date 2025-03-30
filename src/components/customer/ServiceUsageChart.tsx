
import { useState } from 'react';
import { BarChart2 } from 'lucide-react';
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
  Line
} from 'recharts';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { ServiceUsageData } from '@/domain/models/Customer';
import { TimeRange } from '@/application/hooks/useCustomers';

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

// Colores para cada marca en el gráfico
const BRAND_COLORS = [
  "#8884d8", // Morado principal
  "#82ca9d", // Verde claro
  "#ffc658", // Amarillo
  "#0088FE", // Azul
  "#00C49F", // Verde azulado
  "#FF8042", // Naranja
  "#FF6B8B", // Rosa
  "#9467BD", // Morado oscuro
  "#8C564B", // Marrón
  "#E377C2", // Rosa claro
  "#7F7F7F", // Gris
  "#BCBD22"  // Verde oliva
];

const CustomTooltip = ({ active, payload, label }: any) => {
  if (active && payload && payload.length) {
    // Para la gráfica de crecimiento de marcas
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
    
    // Para gráficas por marca
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
  const [chartType, setChartType] = useState<'services' | 'brands' | 'monthly' | 'monthlyByBrand'>('services');

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

  // Obtener datos de crecimiento según el rango de tiempo seleccionado
  const brandGrowthData = calculateBrandGrowth ? calculateBrandGrowth(timeRange) : [];
  const monthlyGrowthData = calculateMonthlyGrowth ? calculateMonthlyGrowth(parseInt(timeRange)) : [];
  const monthlyBrandData = calculateMonthlyGrowthByBrand ? calculateMonthlyGrowthByBrand(parseInt(timeRange)) : { months: [], brands: [] };

  // Preparar datos para el gráfico de crecimiento mensual por marca
  const formattedMonthlyBrandData = monthlyBrandData.months.map((month, index) => {
    const dataPoint: any = { name: month };
    monthlyBrandData.brands.forEach((brand, brandIndex) => {
      // Limitar a 8 marcas para no saturar el gráfico
      if (brandIndex < 8) {
        dataPoint[brand.name] = brand.data[index];
      }
    });
    return dataPoint;
  });

  return (
    <Card>
      <CardHeader>
        <CardTitle className="flex items-center gap-2">
          <BarChart2 className="h-5 w-5" />
          Customer Analytics
        </CardTitle>
        <CardDescription>
          Analyze service usage and brand growth
        </CardDescription>
        <div className="flex items-center justify-between mt-4">
          <Tabs value={chartType} onValueChange={(value) => setChartType(value as any)}>
            <TabsList>
              <TabsTrigger value="services">Service Usage</TabsTrigger>
              <TabsTrigger value="brands">Brand Growth</TabsTrigger>
              <TabsTrigger value="monthly">Monthly Growth</TabsTrigger>
              <TabsTrigger value="monthlyByBrand">Growth by Brand</TabsTrigger>
            </TabsList>
          </Tabs>
          
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
          
          {chartType === 'brands' && brandGrowthData.length > 0 && (
            <ResponsiveContainer width="100%" height="100%">
              <ComposedChart
                layout="vertical"
                data={brandGrowthData}
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
          
          {chartType === 'monthly' && monthlyGrowthData.length > 0 && (
            <ResponsiveContainer width="100%" height="100%">
              <RechartsBarChart
                data={monthlyGrowthData}
                margin={{ top: 20, right: 30, left: 20, bottom: 5 }}
              >
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="name" />
                <YAxis />
                <Tooltip content={<CustomTooltip />} />
                <Legend />
                <Bar dataKey="new" fill="#82ca9d" name="New Customers" />
              </RechartsBarChart>
            </ResponsiveContainer>
          )}

          {chartType === 'monthlyByBrand' && formattedMonthlyBrandData.length > 0 && (
            <ResponsiveContainer width="100%" height="100%">
              <LineChart
                data={formattedMonthlyBrandData}
                margin={{ top: 20, right: 50, left: 20, bottom: 5 }}
              >
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="name" />
                <YAxis />
                <Tooltip content={<CustomTooltip />} />
                <Legend />
                {monthlyBrandData.brands.slice(0, 8).map((brand, index) => (
                  <Line
                    key={brand.name}
                    type="monotone"
                    dataKey={brand.name}
                    stroke={BRAND_COLORS[index % BRAND_COLORS.length]}
                    activeDot={{ r: 8 }}
                    name={brand.name}
                  />
                ))}
              </LineChart>
            </ResponsiveContainer>
          )}
        </div>
      </CardContent>
    </Card>
  );
}
