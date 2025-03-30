
import { supabase } from '@/integrations/supabase/client';
import { Customer } from '@/domain/models/Customer';
import { CustomerRepository } from '@/domain/repositories/CustomerRepository';

export class SupabaseCustomerRepository implements CustomerRepository {
  async getAllCustomers(): Promise<Customer[]> {
    const { data, error } = await supabase
      .from('customers')
      .select('*')
      .order('created_at', { ascending: false });
    
    if (error) {
      console.error('Error fetching customers:', error);
      throw new Error('Failed to fetch customers');
    }
    
    return data.map(customer => {
      // Ensure acquired_services is always an array of strings
      let services: string[] = [];
      
      if (customer.acquired_services) {
        if (Array.isArray(customer.acquired_services)) {
          services = customer.acquired_services.map(service => String(service));
        } else if (typeof customer.acquired_services === 'string') {
          services = [customer.acquired_services];
        }
        // Ignore other types that cannot be reasonably converted to string[]
      }
      
      return {
        id: customer.id,
        brand_name: customer.brand_name,
        contact_name: customer.contact_name,
        contact_email: customer.contact_email,
        contact_phone: customer.contact_phone,
        acquired_services: services,
        created_at: customer.created_at,
        updated_at: customer.updated_at
      };
    });
  }
}
