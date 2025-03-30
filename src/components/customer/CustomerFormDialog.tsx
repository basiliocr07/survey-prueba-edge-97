
import { useState } from 'react';
import { Button } from '@/components/ui/button';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from '@/components/ui/dialog';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { Customer } from '@/domain/models/Customer';
import { v4 as uuidv4 } from 'uuid';
import { PlusCircle } from 'lucide-react';
import { Checkbox } from '@/components/ui/checkbox';

interface CustomerFormDialogProps {
  availableServices: string[];
  onAddCustomer: (customer: Customer) => void;
}

export default function CustomerFormDialog({ availableServices, onAddCustomer }: CustomerFormDialogProps) {
  const [open, setOpen] = useState(false);
  const [formData, setFormData] = useState<Partial<Customer>>({
    brand_name: '',
    contact_name: '',
    contact_email: '',
    contact_phone: '',
    acquired_services: []
  });
  
  const handleInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    setFormData(prev => ({ ...prev, [name]: value }));
  };

  const toggleService = (service: string) => {
    setFormData(prev => {
      const currentServices = prev.acquired_services || [];
      if (currentServices.includes(service)) {
        return {
          ...prev,
          acquired_services: currentServices.filter(s => s !== service)
        };
      } else {
        return {
          ...prev,
          acquired_services: [...currentServices, service]
        };
      }
    });
  };

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    
    // Create new customer object
    const newCustomer: Customer = {
      id: uuidv4(),
      brand_name: formData.brand_name || '',
      contact_name: formData.contact_name || '',
      contact_email: formData.contact_email || '',
      contact_phone: formData.contact_phone,
      acquired_services: formData.acquired_services || [],
      created_at: new Date().toISOString(),
      updated_at: new Date().toISOString()
    };
    
    onAddCustomer(newCustomer);
    
    // Reset form and close dialog
    setFormData({
      brand_name: '',
      contact_name: '',
      contact_email: '',
      contact_phone: '',
      acquired_services: []
    });
    setOpen(false);
  };

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        <Button className="gap-2">
          <PlusCircle className="h-4 w-4" />
          Add Customer
        </Button>
      </DialogTrigger>
      <DialogContent className="sm:max-w-[425px]">
        <form onSubmit={handleSubmit}>
          <DialogHeader>
            <DialogTitle>Add New Customer</DialogTitle>
            <DialogDescription>
              Enter the customer details and the services they've acquired.
            </DialogDescription>
          </DialogHeader>
          <div className="grid gap-4 py-4">
            <div className="grid gap-2">
              <Label htmlFor="brand_name">Brand Name</Label>
              <Input
                id="brand_name"
                name="brand_name"
                value={formData.brand_name}
                onChange={handleInputChange}
                required
              />
            </div>
            <div className="grid gap-2">
              <Label htmlFor="contact_name">Contact Person</Label>
              <Input
                id="contact_name"
                name="contact_name"
                value={formData.contact_name}
                onChange={handleInputChange}
                required
              />
            </div>
            <div className="grid gap-2">
              <Label htmlFor="contact_email">Contact Email</Label>
              <Input
                id="contact_email"
                name="contact_email"
                type="email"
                value={formData.contact_email}
                onChange={handleInputChange}
                required
              />
            </div>
            <div className="grid gap-2">
              <Label htmlFor="contact_phone">Contact Phone</Label>
              <Input
                id="contact_phone"
                name="contact_phone"
                value={formData.contact_phone}
                onChange={handleInputChange}
              />
            </div>
            <div className="grid gap-2">
              <Label>Acquired Services</Label>
              <div className="grid grid-cols-2 gap-2">
                {availableServices.map(service => (
                  <div key={service} className="flex items-center space-x-2">
                    <Checkbox 
                      id={`service-${service}`}
                      checked={(formData.acquired_services || []).includes(service)}
                      onCheckedChange={() => toggleService(service)}
                    />
                    <Label htmlFor={`service-${service}`} className="text-sm">
                      {service}
                    </Label>
                  </div>
                ))}
              </div>
            </div>
          </div>
          <DialogFooter>
            <Button type="submit">Add Customer</Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
