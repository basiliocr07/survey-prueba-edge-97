
import { Service } from '@/domain/models/Service';
import { CustomerRepository } from '@/domain/repositories/CustomerRepository';

export class GetAllServices {
  constructor(private customerRepository: CustomerRepository) {}
  
  async execute(): Promise<Service[]> {
    return this.customerRepository.getAllServices();
  }
}
