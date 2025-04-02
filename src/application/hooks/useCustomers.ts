import { useQuery } from '@tanstack/react-query';
import { GetAllCustomers } from '../useCases/customer/GetAllCustomers';
import { GetAllServices } from '../useCases/customer/GetAllServices';
import { GetCustomerEmails } from '../useCases/customer/GetCustomerEmails';
import { SupabaseCustomerRepository } from '@/infrastructure/repositories/SupabaseCustomerRepository';
import { ServiceUsageData } from '@/domain/models/Customer';
import { Service } from '@/domain/models/Service';
import { format, subMonths, isWithinInterval, startOfMonth, endOfMonth } from 'date-fns';

const customerRepository = new SupabaseCustomerRepository();
const getAllCustomers = new GetAllCustomers(customerRepository);
const getAllServices = new GetAllServices(customerRepository);
const getCustomerEmails = new GetCustomerEmails(customerRepository);

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
    queryFn: () => getCustomerEmails.execute(),
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

  const calculateBrandGrowth = (timeRange: TimeRange = 'all') => {
    if (!customers.length) return [];
    
    const now = new Date();
    let startDate = new Date(0); // Fecha muy antigua para incluir todo
    
    if (timeRange !== 'all') {
      const months = parseInt(timeRange);
      startDate = subMonths(now, months);
    }
    
    const brandGroups: Record<string, { total: number, recent: number }> = {};
    
    customers.forEach(customer => {
      const brandName = customer.brand_name;
      const createdDate = new Date(customer.created_at);
      
      if (!brandGroups[brandName]) {
        brandGroups[brandName] = { total: 0, recent: 0 };
      }
      
      brandGroups[brandName].total += 1;
      
      if (createdDate >= startDate) {
        brandGroups[brandName].recent += 1;
      }
    });
    
    return Object.entries(brandGroups)
      .map(([name, data]) => ({
        name,
        total: data.total,
        recent: data.recent
      }))
      .sort((a, b) => b.total - a.total);
  };

  const calculateMonthlyGrowth = (months: number = 3) => {
    if (!customers.length) return [];
    
    const now = new Date();
    const monthsData: { name: string, new: number }[] = [];
    
    for (let i = 0; i < months; i++) {
      const targetMonth = subMonths(now, i);
      const monthStart = startOfMonth(targetMonth);
      const monthEnd = endOfMonth(targetMonth);
      
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

  const calculateMonthlyGrowthByBrand = (months: number = 3): { months: string[]; brands: { name: string; data: number[] }[] } => {
    if (!customers.length) return { months: [], brands: [] };
    
    const now = new Date();
    const monthNames: string[] = [];
    const brandGrowthData: Record<string, { name: string, data: number[] }> = {};
    
    const brandNames = Array.from(new Set(customers.map(customer => customer.brand_name)));
    
    brandNames.forEach(brand => {
      brandGrowthData[brand] = {
        name: brand,
        data: new Array(months).fill(0)
      };
    });
    
    for (let i = 0; i < months; i++) {
      const targetMonth = subMonths(now, i);
      const monthStart = startOfMonth(targetMonth);
      const monthEnd = endOfMonth(targetMonth);
      const monthName = format(targetMonth, 'MMM yyyy');
      
      monthNames.unshift(monthName);
      
      brandNames.forEach(brand => {
        const brandCustomersInMonth = customers.filter(customer => {
          const createdDate = new Date(customer.created_at);
          return customer.brand_name === brand && 
                 isWithinInterval(createdDate, { start: monthStart, end: monthEnd });
        }).length;
        
        const idx = months - 1 - i;
        if (idx >= 0 && idx < months) {
          brandGrowthData[brand].data[idx] = brandCustomersInMonth;
        }
      });
    }
    
    return {
      months: monthNames,
      brands: Object.values(brandGrowthData)
        .filter(brand => brand.data.some(count => count > 0))
        .sort((a, b) => {
          const totalA = a.data.reduce((sum, count) => sum + count, 0);
          const totalB = b.data.reduce((sum, count) => sum + count, 0);
          return totalB - totalA;
        })
    };
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
    calculateMonthlyGrowth,
    calculateMonthlyGrowthByBrand
  };
}
