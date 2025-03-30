
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

// Colores para cada marca en el gráfico
const BRAND_COLORS = [
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
  const [chartType, setChartType] = useState<'services' | 'servicesPie' | 'brands' | 'monthly' | 'monthlyByBrand' | 'singleBrand'>('services');
  const [selectedBrand, setSelectedBrand] = useState<string>('');
  
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
  
  // Safely get the monthly brand data with proper type checking
  const monthlyBrandData = calculateMonthlyGrowthByBrand ? 
    calculateMonthlyGrowthByBrand(parseInt(timeRange)) : 
    { months: [], brands: [] };

  // Get available brand names
  const availableBrands = brandGrowthData.map(item => item.name);

  // Get individual brand data for the selected brand
  const getSingleBrandData = () => {
    if (!selectedBrand || !monthlyBrandData.months.length) return [];
    
    const brandIndex = monthlyBrandData.brands.findIndex(b => b.name === selectedBrand);
    if (brandIndex === -1) return [];
    
    return monthlyBrandData.months.map((month, index) => ({
      name: month,
      value: brandIndex >= 0 && index < monthlyBrandData.brands[brandIndex].data.length 
        ? monthlyBrandData.brands[brandIndex].data[index] 
        : 0
    }));
  };

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
        <div className="flex items-center justify-between mt-4 flex-wrap gap-2">
          <Tabs value={chartType} onValueChange={(value) => setChartType(value as any)}>
            <TabsList>
              <TabsTrigger value="services">Service Usage (Bar)</TabsTrigger>
              <TabsTrigger value="servicesPie">Service Usage (Pie)</TabsTrigger>
              <TabsTrigger value="brands">Brand Growth</TabsTrigger>
              <TabsTrigger value="monthly">Monthly Growth</TabsTrigger>
              <TabsTrigger value="monthlyByBrand">All Brands</TabsTrigger>
              <TabsTrigger value="singleBrand">Single Brand</TabsTrigger>
            </TabsList>
          </Tabs>
          
          <div className="flex items-center gap-2">
            {chartType === 'singleBrand' && (
              <Select 
                value={selectedBrand} 
                onValueChange={setSelectedBrand}
              >
                <SelectTrigger className="w-[150px]">
                  <SelectValue placeholder="Select Brand" />
                </SelectTrigger>
                <SelectContent>
                  {availableBrands.map(brand => (
                    <SelectItem key={brand} value={brand}>{brand}</SelectItem>
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
        
        {chartType === 'monthlyByBrand' && (
          <div className="mt-4">
            <h4 className="text-sm font-medium mb-2">Brands:</h4>
            <ScrollArea className="h-10 whitespace-nowrap">
              <div className="flex gap-2 pb-2">
                {monthlyBrandData.brands.slice(0, 8).map((brand, index) => (
                  <Badge 
                    key={brand.name} 
                    style={{ backgroundColor: BRAND_COLORS[index % BRAND_COLORS.length] }}
                    className="text-white"
                  >
                    {brand.name}
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
                    <Cell key={`cell-${index}`} fill={BRAND_COLORS[index % BRAND_COLORS.length]} />
                  ))}
                </Pie>
                <Tooltip />
                <Legend />
              </RechartsPieChart>
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
          
          {chartType === 'singleBrand' && selectedBrand && (
            <ResponsiveContainer width="100%" height="100%">
              <RechartsBarChart
                data={getSingleBrandData()}
                margin={{ top: 20, right: 30, left: 20, bottom: 5 }}
              >
                <CartesianGrid strokeDasharray="3 3" />
                <XAxis dataKey="name" />
                <YAxis />
                <Tooltip content={<CustomTooltip />} />
                <Legend />
                <Bar 
                  dataKey="value" 
                  fill={BRAND_COLORS[0]} 
                  name={`New Customers - ${selectedBrand}`} 
                />
              </RechartsBarChart>
            </ResponsiveContainer>
          )}
          
          {chartType === 'singleBrand' && !selectedBrand && (
            <div className="h-full flex items-center justify-center">
              <p className="text-muted-foreground">Select a brand to view its growth data</p>
            </div>
          )}
        </div>
      </CardContent>
    </Card>
  );
}
