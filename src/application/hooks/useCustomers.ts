
import { useQuery } from '@tanstack/react-query';
import { GetAllCustomers } from '../useCases/customer/GetAllCustomers';
import { SupabaseCustomerRepository } from '@/infrastructure/repositories/SupabaseCustomerRepository';
import { ServiceUsageData } from '@/domain/models/Customer';

const customerRepository = new SupabaseCustomerRepository();
const getAllCustomers = new GetAllCustomers(customerRepository);

export function useCustomers() {
  const { data: customers = [], isLoading, error } = useQuery({
    queryKey: ['customers'],
    queryFn: () => getAllCustomers.execute(),
  });

  // Calculate service usage for the chart
  const calculateServiceUsage = (): ServiceUsageData[] => {
    if (!customers.length) return [];
    
    // Get all unique services
    const allServices = new Set<string>();
    customers.forEach(customer => {
      if (customer.acquired_services && Array.isArray(customer.acquired_services)) {
        customer.acquired_services.forEach(service => allServices.add(service));
      }
    });
    
    // Count usage for each service
    const serviceUsageData = Array.from(allServices).map(service => {
      const count = customers.filter(customer => 
        customer.acquired_services && 
        Array.isArray(customer.acquired_services) && 
        customer.acquired_services.includes(service)
      ).length;
      
      return {
        name: service,
        count
      };
    });
    
    // Sort by count descending
    return serviceUsageData.sort((a, b) => b.count - a.count);
  };

  return {
    customers,
    isLoading,
    error,
    serviceUsageData: calculateServiceUsage()
  };
}
