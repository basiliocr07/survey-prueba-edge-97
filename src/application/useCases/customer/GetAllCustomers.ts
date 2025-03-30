
import { Customer } from '@/domain/models/Customer';
import { CustomerRepository } from '@/domain/repositories/CustomerRepository';

export class GetAllCustomers {
  constructor(private customerRepository: CustomerRepository) {}
  
  async execute(): Promise<Customer[]> {
    return this.customerRepository.getAllCustomers();
  }
}
