
import { Customer } from '../models/Customer';

export interface CustomerRepository {
  getAllCustomers(): Promise<Customer[]>;
}
