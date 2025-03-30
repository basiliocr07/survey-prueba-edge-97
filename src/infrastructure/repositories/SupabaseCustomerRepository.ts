
import { supabase } from '@/integrations/supabase/client';
import { Customer } from '@/domain/models/Customer';
import { CustomerRepository } from '@/domain/repositories/CustomerRepository';
import { Service } from '@/domain/models/Service';

export class SupabaseCustomerRepository implements CustomerRepository {
  async getAllCustomers(): Promise<Customer[]> {
    try {
      const { data: customers, error } = await supabase
        .from('customers')
        .select(`
          *,
          customer_services!inner(
            service_id
          ),
          services:customer_services!inner(
            services(id, name)
          )
        `);

      if (error) {
        console.error('Error fetching customers:', error);
        return [];
      }

      return customers.map(customer => {
        // Extract service names from the nested services data
        const services = customer.services.map((serviceRel: any) => serviceRel.services.name);
        
        // Ensure customer_type is one of the allowed values, default to 'client' if invalid
        const customerType = customer.customer_type === 'admin' ? 'admin' : 'client';

        return {
          id: customer.id,
          brand_name: customer.brand_name,
          contact_name: customer.contact_name,
          contact_email: customer.contact_email,
          contact_phone: customer.contact_phone,
          created_at: customer.created_at,
          updated_at: customer.updated_at,
          acquired_services: services,
          customer_type: customerType
        };
      });
    } catch (error) {
      console.error('Unexpected error fetching customers:', error);
      return [];
    }
  }

  async getAllServices(): Promise<Service[]> {
    try {
      const { data: services, error } = await supabase
        .from('services')
        .select('*')
        .order('name');

      if (error) {
        console.error('Error fetching services:', error);
        return [];
      }

      return services;
    } catch (error) {
      console.error('Unexpected error fetching services:', error);
      return [];
    }
  }

  async addCustomerService(customerId: string, serviceId: string): Promise<void> {
    try {
      const { error } = await supabase
        .from('customer_services')
        .insert({ customer_id: customerId, service_id: serviceId });

      if (error) {
        console.error('Error adding customer service:', error);
        throw new Error('Failed to add customer service');
      }
    } catch (error) {
      console.error('Unexpected error adding customer service:', error);
      throw error;
    }
  }

  async getCustomerEmails(): Promise<string[]> {
    try {
      const { data, error } = await supabase
        .from('customers')
        .select('contact_email')
        .order('contact_email');

      if (error) {
        console.error('Error fetching customer emails:', error);
        return [];
      }

      return data.map(customer => customer.contact_email);
    } catch (error) {
      console.error('Unexpected error fetching customer emails:', error);
      return [];
    }
  }
}
