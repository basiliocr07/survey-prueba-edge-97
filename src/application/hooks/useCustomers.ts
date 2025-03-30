
import { useQuery } from '@tanstack/react-query';
import { GetAllCustomers } from '../useCases/customer/GetAllCustomers';
import { GetAllServices } from '../useCases/customer/GetAllServices';
import { SupabaseCustomerRepository } from '@/infrastructure/repositories/SupabaseCustomerRepository';
import { ServiceUsageData } from '@/domain/models/Customer';
import { Service } from '@/domain/models/Service';
import { format, subMonths, isWithinInterval, startOfMonth, endOfMonth } from 'date-fns';

const customerRepository = new SupabaseCustomerRepository();
const getAllCustomers = new GetAllCustomers(customerRepository);
const getAllServices = new GetAllServices(customerRepository);

export type TimeRange = '1' | '3' | '12' | 'all';

export function useCustomers() {
  const { 
    data: customers = [], 
    isLoading: isLoadingCustomers, 
    error: customersError 
  } = useQuery({
    queryKey: ['customers'],
    queryFn: () => getAllCustomers.execute(),
  });

  const {
    data: services = [],
    isLoading: isLoadingServices,
    error: servicesError
  } = useQuery({
    queryKey: ['services'],
    queryFn: () => getAllServices.execute(),
  });

  const {
    data: customerEmails = [],
    isLoading: isLoadingEmails,
    error: emailsError
  } = useQuery({
    queryKey: ['customer-emails'],
    queryFn: () => customerRepository.getCustomerEmails(),
  });

  const calculateServiceUsage = (): ServiceUsageData[] => {
    if (!customers.length) return [];
    
    const allServices = new Set<string>();
    customers.forEach(customer => {
      if (customer.acquired_services && Array.isArray(customer.acquired_services)) {
        customer.acquired_services.forEach(service => allServices.add(service));
      }
    });
    
    return Array.from(allServices).map(service => {
      const count = customers.filter(customer => 
        customer.acquired_services && 
        Array.isArray(customer.acquired_services) && 
        customer.acquired_services.includes(service)
      ).length;
      
      return {
        name: service,
        count
      };
    }).sort((a, b) => b.count - a.count);
  };

  // Nueva función para calcular el crecimiento por marca en períodos de tiempo
  const calculateBrandGrowth = (timeRange: TimeRange = 'all') => {
    if (!customers.length) return [];
    
    const now = new Date();
    let startDate = new Date(0); // Fecha muy antigua para incluir todo
    
    // Determinar la fecha de inicio según el rango de tiempo seleccionado
    if (timeRange !== 'all') {
      const months = parseInt(timeRange);
      startDate = subMonths(now, months);
    }
    
    // Agrupar marcas y contar clientes
    const brandGroups: Record<string, { total: number, recent: number }> = {};
    
    customers.forEach(customer => {
      const brandName = customer.brand_name;
      const createdDate = new Date(customer.created_at);
      
      // Inicializar si no existe
      if (!brandGroups[brandName]) {
        brandGroups[brandName] = { total: 0, recent: 0 };
      }
      
      // Aumentar contador total
      brandGroups[brandName].total += 1;
      
      // Aumentar contador reciente si se creó dentro del rango de tiempo
      if (createdDate >= startDate) {
        brandGroups[brandName].recent += 1;
      }
    });
    
    // Convertir a array y ordenar por total
    return Object.entries(brandGroups)
      .map(([name, data]) => ({
        name,
        total: data.total,
        recent: data.recent
      }))
      .sort((a, b) => b.total - a.total);
  };

  // Nueva función para generar datos de crecimiento mensual
  const calculateMonthlyGrowth = (months: number = 3) => {
    if (!customers.length) return [];
    
    const now = new Date();
    const monthsData: { name: string, new: number }[] = [];
    
    // Generar últimos N meses
    for (let i = 0; i < months; i++) {
      const targetMonth = subMonths(now, i);
      const monthStart = startOfMonth(targetMonth);
      const monthEnd = endOfMonth(targetMonth);
      
      // Contar clientes creados en este mes
      const newCustomers = customers.filter(customer => {
        const createdDate = new Date(customer.created_at);
        return isWithinInterval(createdDate, { start: monthStart, end: monthEnd });
      }).length;
      
      monthsData.unshift({
        name: format(targetMonth, 'MMM yyyy'),
        new: newCustomers
      });
    }
    
    return monthsData;
  };

  const isLoading = isLoadingCustomers || isLoadingServices || isLoadingEmails;
  const error = customersError || servicesError || emailsError;

  return {
    customers,
    services,
    customerEmails,
    isLoading,
    error,
    serviceUsageData: calculateServiceUsage(),
    calculateBrandGrowth,
    calculateMonthlyGrowth
  };
}
