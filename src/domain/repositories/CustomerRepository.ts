
import { Customer } from '../models/Customer';
import { Service } from '../models/Service';

export interface CustomerRepository {
  getAllCustomers(): Promise<Customer[]>;
  getAllServices(): Promise<Service[]>;
  addCustomerService(customerId: string, serviceId: string): Promise<void>;
}
