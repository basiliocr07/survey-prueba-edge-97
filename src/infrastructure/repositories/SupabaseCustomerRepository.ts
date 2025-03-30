
import { supabase } from '@/integrations/supabase/client';
import { Customer } from '@/domain/models/Customer';
import { Service } from '@/domain/models/Service';
import { CustomerRepository } from '@/domain/repositories/CustomerRepository';

export class SupabaseCustomerRepository implements CustomerRepository {
  async getAllCustomers(): Promise<Customer[]> {
    // Obtener todos los clientes
    const { data: customers, error: customersError } = await supabase
      .from('customers')
      .select('*')
      .order('created_at', { ascending: false });
    
    if (customersError) {
      console.error('Error fetching customers:', customersError);
      throw new Error('Failed to fetch customers');
    }
    
    // Para cada cliente, obtener sus servicios
    const result = await Promise.all(customers.map(async (customer) => {
      const { data: customerServices, error: servicesError } = await supabase
        .from('customer_services')
        .select('services(name)')
        .eq('customer_id', customer.id);
      
      if (servicesError) {
        console.error('Error fetching customer services:', servicesError);
        return {
          id: customer.id,
          brand_name: customer.brand_name,
          contact_name: customer.contact_name,
          contact_email: customer.contact_email,
          contact_phone: customer.contact_phone,
          acquired_services: [],
          created_at: customer.created_at,
          updated_at: customer.updated_at
        };
      }
      
      // Extraer los nombres de los servicios
      const serviceNames = customerServices
        .map(cs => cs.services?.name)
        .filter(name => typeof name === 'string') as string[];
      
      return {
        id: customer.id,
        brand_name: customer.brand_name,
        contact_name: customer.contact_name,
        contact_email: customer.contact_email,
        contact_phone: customer.contact_phone,
        acquired_services: serviceNames,
        created_at: customer.created_at,
        updated_at: customer.updated_at
      };
    }));
    
    return result;
  }

  async getAllServices(): Promise<Service[]> {
    const { data, error } = await supabase
      .from('services')
      .select('*')
      .order('name');
    
    if (error) {
      console.error('Error fetching services:', error);
      throw new Error('Failed to fetch services');
    }
    
    return data;
  }

  async addCustomerService(customerId: string, serviceId: string): Promise<void> {
    const { error } = await supabase
      .from('customer_services')
      .insert({ customer_id: customerId, service_id: serviceId });
    
    if (error) {
      console.error('Error adding customer service:', error);
      throw new Error('Failed to add customer service');
    }
  }
}
