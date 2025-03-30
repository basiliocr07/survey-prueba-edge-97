import { useQuery } from '@tanstack/react-query';
import { GetAllCustomers } from '../useCases/customer/GetAllCustomers';
import { GetAllServices } from '../useCases/customer/GetAllServices';
import { SupabaseCustomerRepository } from '@/infrastructure/repositories/SupabaseCustomerRepository';
import { ServiceUsageData } from '@/domain/models/Customer';
import { Service } from '@/domain/models/Service';

const customerRepository = new SupabaseCustomerRepository();
const getAllCustomers = new GetAllCustomers(customerRepository);
const getAllServices = new GetAllServices(customerRepository);

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
    
    return serviceUsageData.sort((a, b) => b.count - a.count);
  };

  const isLoading = isLoadingCustomers || isLoadingServices || isLoadingEmails;
  const error = customersError || servicesError || emailsError;

  return {
    customers,
    services,
    customerEmails,
    isLoading,
    error,
    serviceUsageData: calculateServiceUsage()
  };
}
