
export interface Customer {
  id: string;
  brand_name: string;
  contact_name: string;
  contact_email: string;
  contact_phone?: string;
  acquired_services?: string[];
  created_at: string;
  updated_at: string;
  customer_type: 'admin' | 'client';  // Nuevo campo para tipo de cliente
}

export interface ServiceUsageData {
  name: string;
  count: number;
}
