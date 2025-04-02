
import { CustomerRepository } from '@/domain/repositories/CustomerRepository';

export class GetCustomerEmails {
  constructor(private customerRepository: CustomerRepository) {}
  
  async execute(filter?: { customerType?: string }): Promise<string[]> {
    return this.customerRepository.getCustomerEmails(filter);
  }
}
